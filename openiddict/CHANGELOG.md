# Changelog

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
