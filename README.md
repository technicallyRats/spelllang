# Spelllang

Spelllang is a small, interpreted programming language implemented in C#, with a hand-written lexer, parser, AST, and tree-walking interpreter. It targets `netstandard2.1`, so the resulting `Spelllang.dll` runs on any .NET Core/5+ runtime across Linux, Windows, and macOS from a single build.

## Release process

Releases are driven by git tags and automated through GitHub Actions. The workflow lives at [`.github/workflows/release.yml`](.github/workflows/release.yml) and runs whenever a `v*` tag is pushed.

### One command

```sh
./scripts/release.sh            # bump patch (default), update csproj, commit, tag
./scripts/release.sh --minor    # bump minor version
./scripts/release.sh --major    # bump major version
./scripts/release.sh --version 1.2.3   # explicit version
./scripts/release.sh --push     # also push commit + tag to origin
./scripts/release.sh --dry-run  # preview without changing anything
```

The script:

1. Reads the current `<Version>` from `src/Spelllang/Spelllang.csproj`.
2. Bumps it according to the chosen mode (semver: `MAJOR.MINOR.PATCH`).
3. Rewrites `<Version>` in the csproj and commits the change as `release vX.Y.Z`.
4. Creates the `vX.Y.Z` tag.

It does **not** push unless you pass `--push`. Pushing the tag is what triggers the release workflow.

### What the workflow does

The workflow is split into three jobs that run in sequence:

1. **`verify`** — confirms the tag (minus the leading `v`) matches `<Version>` in the csproj. Fails fast if they disagree, so no release is built from a mismatched version.
2. **`build`** — sets up .NET 8 and builds `src/Spelllang/Spelllang.csproj` in `Release` mode. Only the main library is built (no Sample or Tests). The resulting `Spelllang.dll` and `Spelllang.xml` docs are uploaded as an artifact.
3. **`release`** — downloads the artifact and creates a GitHub Release on the tag with `Spelllang.dll` and `Spelllang.xml` attached, using auto-generated release notes.

Because the project targets `netstandard2.1`, a single build produces a DLL that runs on Linux, Windows, and macOS — no per-OS matrix is needed.

### Manual alternative

If you prefer to do the steps by hand:

```sh
# 1. Update <Version> in src/Spelllang/Spelllang.csproj
# 2. Commit and tag
git add src/Spelllang/Spelllang.csproj
git commit -m "release v0.0.5"
git tag v0.0.5
# 3. Push to trigger the workflow
git push origin main
git push origin v0.0.5
```

The `verify` job will reject the run if the tag and the csproj version don't match, so keep them in sync.
