# Changelog

## 5.0.0
### Added
- No new functionality added

### Changed
- **BREAKING**: Upgraded OpenIddict packages from 5.2.0 to 6.4.0
  - Updated endpoint naming: `SetLogoutEndpointUris` → `SetEndSessionEndpointUris`, `SetUserinfoEndpointUris` → `SetUserInfoEndpointUris`
  - Updated prompt API: `Prompts` → `PromptValues`, `HasPrompt()` → `HasPromptValue()`, `GetPrompts()` → `GetPromptValues()`
  - Updated permissions: `Permissions.Endpoints.Logout` → `Permissions.Endpoints.EndSession`
- Upgraded Microsoft.EntityFrameworkCore.SqlServer to minimum required versions (6.0.36 for .NET 6.0, 8.0.17 for .NET 8.0)
- Upgraded Microsoft.Extensions.Hosting.Abstractions to minimum required versions (6.0.1 for .NET 6.0, 8.0.1 for .NET 8.0)
- Upgraded System.Drawing.Common from 4.7.3 to 6.0.0
- See full OpenIddict migration guide [here](https://documentation.openiddict.com/guides/migration/50-to-60)

### Fixed
- No bugs fixed

### Notes
- **No database migration required** - OpenIddict 6.x is fully compatible with 5.x database schema
- All deprecated APIs have been updated to their v6 equivalents
- 100% test pass rate maintained

## 4.1.1
### Added
- No new functionality added

### Changed
- No functionality changed

### Fixed
- Upgraded vulnerable versions ([5a7bbeb](https://github.com/audaciaconsulting/Audacia.Auth/commit/5a7bbebe9caf19ff26e380b740a39f33d681ab5b))

## 4.1.0
### Changed
- Updated TokenIssuedSuccessEvent to include claims.

## 4.0.0 - 2024-04-11
### Added
- Added .Net 6.0 and .Net 8.0 multi-targeting ([1d93a32](https://github.com/audaciaconsulting/Audacia.Auth/pull/5/commits/1d93a327618f9effd1392be864f8aa0bed3f099a))

## 3.0.0 - 2024-02-12
### Added
- No new functionality added

### Changed
- OpenIddict package references updated to 5.0.1 ([90e14cc](https://github.com/audaciaconsulting/Audacia.Auth/pull/3/commits/90e14cc40404674fb65fb01a27e91785774b59d0))
- `Type` renamed to `ClientType`
     - <b>IMPORTANT</b> this will require a database migration as it changes the column name for OpenIddictApplications
- See further changes to openiddict 5 [here](https://documentation.openiddict.com/guides/migration/40-to-50.html)

## 2.0.0 - 2023-12-05
### Added
- No new functionality added

### Changed
- Upgraded to support OpenIddict version 4.6.0 ([e570cc9](https://github.com/audaciaconsulting/Audacia.Auth/pull/2/commits/e570cc9b42315a159eb20d8e9b09bd9b475c5714))
