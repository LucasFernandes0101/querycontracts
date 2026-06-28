# Release Checklist

## Pre-release

- [ ] Confirm GitHub repo URL: https://github.com/LucasFernandes0101/querycontracts
- [ ] Confirm NuGet package ID: `QueryContracts`
- [ ] Run `dotnet format`
- [ ] Run `dotnet test`
- [ ] Run `dotnet build -c Release`
- [ ] Run `dotnet pack src/QueryContracts/QueryContracts.csproj -c Release`
- [ ] Verify the `.nupkg` file exists in `src/QueryContracts/bin/Release/`
- [ ] Verify no secrets are present in any file

## Publish

- [ ] Set the NuGet API key as an environment variable (do not commit it)
- [ ] Run `dotnet nuget push` with the API key
- [ ] Verify the package appears on nuget.org

## Tag and release

- [ ] Create git tag `v0.1.0-preview.1`
- [ ] Push the tag to origin
- [ ] Create GitHub release marked as **prerelease**
- [ ] Use CHANGELOG.md content as release notes

## Post-release

- [ ] Post on LinkedIn
- [ ] Update resume/portfolio with the project link
