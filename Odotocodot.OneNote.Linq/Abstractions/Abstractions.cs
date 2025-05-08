using System.Collections.Generic;
using System.Drawing;

namespace Odotocodot.OneNote.Linq.Abstractions
{
    public interface IHasSectionsAndSectionGroups
    {
        IEnumerable<OneNoteSection> Sections { get; }
        IEnumerable<OneNoteSectionGroup> SectionGroups { get; }
    }
    internal interface IWriteSectionsAndSectionGroups : IHasSectionsAndSectionGroups
    {
        IEnumerable<OneNoteSection> Sections { set; }
        IEnumerable<OneNoteSectionGroup> SectionGroups { set; }
    }

    public interface IHasPath
    {
        string Path { get; }
    }

    internal interface IWritePath : IHasPath
    {
        string Path { set; }
    }

    public interface IHasColor
    {
        Color? Color { get; }
    }

    internal interface IWriteColor : IHasColor
    {
        Color? Color { set; }
    }

    public interface IHasIsInRecycleBin
    {
        bool IsInRecycleBin { get; }
    }

    internal interface IWriteIsInRecycleBin : IHasIsInRecycleBin
    {
        bool IsInRecycleBin { set; }
    }
}
