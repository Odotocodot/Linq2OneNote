# Changelog

All notable changes to this project will be documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.1.0/),
and this project adheres to [Semantic Versioning](https://semver.org/spec/v2.0.0.html).

## [1.1.0] - 2024-06-
### Added
- Exposed OneNote COM object to allow for more advanced operations if needed.
- Added and refactored parser tests.
- Exposed UpdatePageContent method.
- LinqPad samples
- Added FindByID method to find a hierarchy item by its ID (Currently slow).

### Changed
- Updated logo!
- OneNoteNotebook.Notebook returns itself rather than null.
- Updated documentation to include examples and more information on the library.
- The methods that create hierarchy items e.g. CreatePage, CreateSection, CreateSectionGroup, CreateNotebook now return the ID of the created item. Can be used with the new FindByID.

### Deprecated
### Removed
### Fixed

## [1.0.0] - 2023-10-16