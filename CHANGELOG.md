# Changelog

## [1.1.0] - 2024-06-04

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

## [1.0.0] - 2023-10-16