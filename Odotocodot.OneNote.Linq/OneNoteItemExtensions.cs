﻿using System;
using System.Collections.Generic;
using System.Linq;

namespace Odotocodot.OneNote.Linq
{
    /// <summary>
    /// A static class containing extension methods for the <see cref="IOneNoteItem"/> object.
    /// </summary>
    public static class OneNoteItemExtensions
    {
        /// <summary>
        /// Returns a flattened collection of OneNote items, that contains the children of every OneNote item from the <paramref name="source"/>.
        /// </summary>
        /// <param name="source">The source OneNote item.</param>
        /// <returns>An <see cref="IEnumerable{T}">IEnumerable</see>&lt;<see cref="IOneNoteItem"/>&gt; containing the 
        /// child items of the <paramref name="source"/>.</returns>
        /// <remarks>This method uses a non-recursive depth first traversal algorithm.</remarks>
        public static IEnumerable<IOneNoteItem> Traverse(this IOneNoteItem source)
        {
            var stack = new Stack<IOneNoteItem>();
            stack.Push(source);
            while (stack.Count > 0)
            {
                var current = stack.Pop();

                yield return current;

                foreach (var child in current.Children)
                    stack.Push(child);
            }
        }

        /// <summary>
        /// Returns a filtered flattened collection of OneNote items, that contains the children of every OneNote item 
        /// from the <paramref name="source"/>.<br/>
        /// Only items that successfully pass the <paramref name="predicate"/> are returned.
        /// </summary>
        /// <param name="source"><inheritdoc cref="Traverse(IOneNoteItem)" path="/param[@name='source']"/></param>
        /// <param name="predicate">A function to test each item for a condition.</param>
        /// <returns>An <see cref="IEnumerable{T}">IEnumerable</see>&lt;<see cref="IOneNoteItem"/>&gt; containing the 
        /// child items of the <paramref name="source"/> that pass the <paramref name="predicate"/>.</returns>
        /// <remarks><inheritdoc cref="Traverse(IOneNoteItem)" path="/remarks"/></remarks>
        public static IEnumerable<IOneNoteItem> Traverse(this IOneNoteItem source, Func<IOneNoteItem, bool> predicate)
        {
            var stack = new Stack<IOneNoteItem>();
            stack.Push(source);
            while (stack.Count > 0)
            {
                var current = stack.Pop();

                if (predicate(current))
                    yield return current;

                foreach (var child in current.Children)
                    stack.Push(child);
            }
        }

        /// <inheritdoc cref="Traverse(IOneNoteItem)"/>
        public static IEnumerable<IOneNoteItem> Traverse(this IEnumerable<IOneNoteItem> source)
            => source.SelectMany(item => item.Traverse());

        /// <inheritdoc cref="Traverse(IOneNoteItem,Func{IOneNoteItem, bool})"/>
        public static IEnumerable<IOneNoteItem> Traverse(this IEnumerable<IOneNoteItem> source, Func<IOneNoteItem, bool> predicate)
            => source.SelectMany(item => item.Traverse(predicate));

        /// <inheritdoc cref="OneNoteApplication.OpenInOneNote(IOneNoteItem)"/>
        public static void OpenInOneNote(this IOneNoteItem item) => OneNoteApplication.OpenInOneNote(item);

        /// <inheritdoc cref="OneNoteApplication.SyncItem(IOneNoteItem)"/>
        public static void Sync(this IOneNoteItem item) => OneNoteApplication.SyncItem(item);

        /// <inheritdoc cref="OneNoteApplication.GetPageContent(OneNotePage)"/>
        public static string GetPageContent(this OneNotePage page) => OneNoteApplication.GetPageContent(page);

        /// <summary>
        /// Returns a value that indicates whether the <paramref name="item"/> is in or is a recycle bin.
        /// </summary>
        /// <param name="item">The OneNote item to check.</param>
        /// <returns><see langword="true"/> if the <paramref name="item"/> is in or is a recycle bin; otherwise, <see langword="false"/>.</returns>
        /// <remarks>Checks whether the <paramref name="item"/> is a recycle bin <see cref="OneNoteSectionGroup">section group</see>,
        /// a deleted <see cref="OneNotePage">page</see>, a deleted <see cref="OneNoteSection">section</see>, or the deleted pages 
        /// <see cref="OneNoteSection">section</see> within a recycle bin.</remarks>
        /// <seealso cref="OneNoteSectionGroup.IsRecycleBin"/>
        /// <seealso cref="OneNoteSection.IsInRecycleBin"/>
        /// <seealso cref="OneNoteSection.IsDeletedPages"/>
        /// <seealso cref="OneNotePage.IsInRecycleBin"/>
        public static bool IsInRecycleBin(this IOneNoteItem item)
        {
            switch (item)
            {
                case OneNoteSectionGroup sectionGroup:
                    return sectionGroup.IsRecycleBin;
                case OneNoteSection section:
                    return section.IsInRecycleBin || section.IsDeletedPages; //If IsDeletedPages is true IsInRecycleBin is always true
                case OneNotePage page:
                    return page.IsInRecycleBin;
                default:
                    return false;
            }
        }

        /// <summary>
        /// Get the recycle bin <see cref="OneNoteSectionGroup">section group</see> for the specified <paramref name="notebook"/> if it exists.
        /// </summary>
        /// <param name="notebook">The notebook to get the recycle bin of.</param>
        /// <param name="sectionGroup">When this method returns, <paramref name="sectionGroup"/> contains the recycle bin of 
        /// the <paramref name="notebook"/> if it was found; 
        /// otherwise, <see langword="null"/>.</param>
        /// <returns><see langword="true"/> if the <paramref name="notebook"/> contains a recycle bin; otherwise, <see langword="false"/>.</returns>
        public static bool GetRecycleBin(this OneNoteNotebook notebook, out OneNoteSectionGroup sectionGroup)
        {
            sectionGroup = notebook.SectionGroups.FirstOrDefault(sg => sg.IsRecycleBin);
            return sectionGroup != null;
        }

        /// <summary>
        /// Returns a flattened collection of all the <see cref="OneNotePage">pages</see> present in the <paramref name="source"/>.
        /// </summary>
        /// <param name="source"><inheritdoc cref="Traverse(IOneNoteItem)" path="/param[@name='source']"/></param>
        /// <returns>An <see cref="IEnumerable{T}">IEnumerable</see>&lt;<see cref="OneNotePage"/>&gt; containing all the 
        /// <see cref="OneNotePage">pages</see> present in the <paramref name="source"/>.</returns>
        public static IEnumerable<OneNotePage> GetPages(this IOneNoteItem source)
            => source.Traverse(i => i is OneNotePage).Cast<OneNotePage>();

        /// <inheritdoc cref="GetPages(IOneNoteItem)"/>
        public static IEnumerable<OneNotePage> GetPages(this IEnumerable<IOneNoteItem> source)
            => source.Traverse(i => i is OneNotePage).Cast<OneNotePage>();


        /// <summary>
        /// Finds the <see cref="IOneNoteItem"/> with the corresponding <paramref name="id"/>.
        /// </summary>
        /// <param name="id">The <see cref="IOneNoteItem.ID"/> of the OneNote hierarchy <see cref="IOneNoteItem">item</see> to find.</param>
        /// <returns>The <see cref="IOneNoteItem"></see> with the specified ID if found; otherwise, <see langword="null"/>.</returns>
        /// <remarks>
        /// This method currently uses <see cref="OneNoteApplication.GetNotebooks()"/> which returns the whole hierarchy to find the ID. So be weary of performance.
        /// </remarks>
        public static IOneNoteItem FindByID(string id) =>
            OneNoteApplication.GetNotebooks().Traverse(i => i.ID == id).FirstOrDefault();

        /// <summary>
        /// Checks if two <see cref="IOneNoteItem"/>s are equal in OneNote.<br/>
        /// Shorthand for comparing the <see cref="IOneNoteItem.ID">ID</see> of OneNote hierarchy items. E.g.
        /// <code>
        /// if(left.ID == right.ID)
        /// {
        ///     Console.WriteLine("Equal")
        /// }
        /// </code>
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <returns></returns>
        public static bool ItemEquals<T>(this T left, T right) where T : IOneNoteItem
        {
            return left.ID == right.ID;
        }
    }
}
