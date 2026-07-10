#!/usr/bin/env bash
# Bumps the Spelllang .NET library version, updates the <Version> in the
# csproj, commits the change, and creates the matching git tag.
#
# What it does:
#   1. Reads the current version from src/Spelllang/Spelllang.csproj (<Version>).
#   2. Determines the next version by bumping semver:
#        - bump patch (default): 0.1.0 -> 0.1.1
#        - --minor:               0.1.0 -> 0.2.0
#        - --major:               1.2.3 -> 2.0.0
#        - --version X.Y.Z:       explicit version (no 'v' prefix needed)
#   3. Updates <Version>X.Y.Z</Version> in src/Spelllang/Spelllang.csproj.
#   4. Commits as "release vX.Y.Z".
#   5. Tags the commit as vX.Y.Z.
#   6. Prints next steps (push commit + tag to origin).
#
# The script does NOT push by default. Use --push to push both the commit and
# the tag (this triggers the .github/workflows/release.yml workflow).
#
# Usage:
#   ./scripts/release.sh                  # bump patch from current csproj version
#   ./scripts/release.sh --minor          # bump minor
#   ./scripts/release.sh --major          # bump major
#   ./scripts/release.sh --version 1.2.3  # explicit version
#   ./scripts/release.sh --push           # also push to origin (combinable)
#   ./scripts/release.sh --dry-run        # print what would happen, change nothing

set -euo pipefail

REPO_ROOT="$(cd "$(dirname "${BASH_SOURCE[0]}")/.." && pwd)"
CSPROJ="$REPO_ROOT/src/Spelllang/Spelllang.csproj"
cd "$REPO_ROOT"

# --- helpers -----------------------------------------------------------------

die() { echo "error: $*" >&2; exit 1; }

bump_patch() { echo "${1}.${2}.$((10#$3 + 1))"; }
bump_minor() { echo "${1}.$((10#$2 + 1)).0"; }
bump_major() { echo "$((10#$1 + 1)).0.0"; }

# --- parse args --------------------------------------------------------------

BUMP="patch"
EXPLICIT_VERSION=""
PUSH=false
DRY_RUN=false

while [[ $# -gt 0 ]]; do
  case "$1" in
    --patch)  BUMP="patch"; shift ;;
    --minor)  BUMP="minor"; shift ;;
    --major)  BUMP="major"; shift ;;
    --version)
      [[ $# -ge 2 ]] || die "--version requires an argument"
      EXPLICIT_VERSION="$2"
      shift 2
      ;;
    --push)   PUSH=true; shift ;;
    --dry-run) DRY_RUN=true; shift ;;
    -h|--help)
      sed -n '2,28p' "$0" | sed 's/^# \{0,1\}//'
      exit 0
      ;;
    *) die "unknown argument: $1 (see --help)" ;;
  esac
done

# --- pre-flight checks -------------------------------------------------------

[[ -f "$CSPROJ" ]] || die "csproj not found: $CSPROJ"
[[ -n "$(git tag --list 'v[0-9]*' --sort=-v:refname)" ]] \
  || echo "warning: no existing v* tags found (this will be the first release)."

# --- read current version from csproj ----------------------------------------

current_version=$(grep -oP '<Version>\K[^<]+' "$CSPROJ" || true)
if [[ -z "$current_version" ]]; then
  die "no <Version>...</Version> element found in $CSPROJ"
fi
[[ "$current_version" =~ ^[0-9]+\.[0-9]+\.[0-9]+$ ]] \
  || die "csproj <Version> is not valid semver (got: $current_version)"

IFS='.' read -r cur_major cur_minor cur_patch <<< "$current_version"

# --- resolve next version ----------------------------------------------------

if [[ -n "$EXPLICIT_VERSION" ]]; then
  EXPLICIT_VERSION="${EXPLICIT_VERSION#v}"
  [[ "$EXPLICIT_VERSION" =~ ^[0-9]+\.[0-9]+\.[0-9]+$ ]] \
    || die "explicit version must match X.Y.Z (got: $EXPLICIT_VERSION)"
  new_version="$EXPLICIT_VERSION"
else
  case "$BUMP" in
    patch) new_version=$(bump_patch "$cur_major" "$cur_minor" "$cur_patch") ;;
    minor) new_version=$(bump_minor "$cur_major" "$cur_minor") ;;
    major) new_version=$(bump_major "$cur_major") ;;
  esac
fi

if [[ "$new_version" == "$current_version" ]]; then
  die "new version ($new_version) is the same as current — nothing to do."
fi

new_tag="v$new_version"

echo "Current csproj version: $current_version"
echo "Next release version:   $new_version"
echo "Tag to create:          $new_tag"
echo

if [[ "$DRY_RUN" == true ]]; then
  echo "--dry-run set: no changes will be made."
  exit 0
fi

# --- working tree check ------------------------------------------------------

if [[ -n "$(git status --porcelain)" ]]; then
  echo "warning: working tree has uncommitted changes:"
  git status --short
  echo
  read -r -p "Continue anyway? [y/N] " reply
  [[ "$reply" =~ ^[Yy]$ ]] || die "aborted by user"
fi

# --- check tag doesn't already exist -----------------------------------------

if git rev-parse -q --verify "refs/tags/$new_tag" >/dev/null; then
  die "tag $new_tag already exists. Delete it first or choose a different version."
fi

# --- update csproj -----------------------------------------------------------

echo "==> updating $CSPROJ to $new_version"
# In-place replace of the <Version> element. Portable across GNU/BSD sed via perl.
if perl -e 'exit(!(defined $ARGV[0] && -f $ARGV[0]))' "$CSPROJ" 2>/dev/null; then
  perl -i -pe "s{(<Version>)[^<]*(</Version>)}{\${1}$new_version\${2}}" "$CSPROJ"
else
  die "perl is required to update the csproj but was not found."
fi

# verify the write took
written_version=$(grep -oP '<Version>\K[^<]+' "$CSPROJ")
[[ "$written_version" == "$new_version" ]] \
  || die "failed to update csproj (expected $new_version, got $written_version)"

# --- stage, commit, tag ------------------------------------------------------

git add "$CSPROJ"

echo "==> committing release $new_tag"
git commit -m "release $new_tag"

echo "==> tagging $new_tag"
git tag "$new_tag"

echo
echo "==> done: $new_tag created"
echo
echo "Next steps:"
echo "  git push origin main"
echo "  git push origin $new_tag"
if [[ "$PUSH" == true ]]; then
  echo
  echo "--push set: pushing now"
  git push origin main
  git push origin "$new_tag"
fi