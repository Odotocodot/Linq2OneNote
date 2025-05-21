# Changelog

## [1.2.0] - 2025-05-2X

### What’s Changed
- Refactored code base
  - Added abstractions that allow for a reduction in duplicated code, e.g. there is no overloads for the method `CreateSection`. It now accepts both a `OneNoteNotebook` and `OneNoteSectionGroup` as the parent.
  - Refactored tests.

## [1.1.0] - 2024-06-04

### Added
- Exposed OneNote COM object to allow for more advanced operations if needed.
- Added and refactored parser tests.
- Exposed UpdatePageContent method.
- LinqPad samples
- Added FindByID method to find a hierarchy item by its ID (Currently slow).

### Changed
- Updated logo!
- Renamed IOneNoteItemExtensions to OneNoteItemExtensions.
- OneNoteNotebook.Notebook returns itself rather than null.
- Updated documentation to include examples and more information on the library.
- The methods that create hierarchy items e.g. `CreatePage`, `CreateSection`, `CreateSectionGroup`, `CreateNotebook` now return the ID of the created item. Can be used with the new `FindByID`.

## [1.0.0] - 2023-10-16