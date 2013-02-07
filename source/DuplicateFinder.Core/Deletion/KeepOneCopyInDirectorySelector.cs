using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace DuplicateFinder.Core.Deletion
{
  class KeepOneCopyInDirectorySelector : ISelectFilesToDelete
  {
    readonly string _keepDirectory;

    public KeepOneCopyInDirectorySelector(string keepFilesInDirectory)
    {
      _keepDirectory = Path.GetFullPath(keepFilesInDirectory);
    }

    public IEnumerable<string> FilesToDelete(IEnumerable<string> duplicates)
    {
      var keep = duplicates
                   .FirstOrDefault(x => InPath(_keepDirectory, x)) ??
                 duplicates
                   .OrderBy(NumberOfPathSeparators)
                   .ThenBy(Path.GetFileName)
                   .First();

      return duplicates.Except(new[] { keep });
    }

    static int NumberOfPathSeparators(string file)
    {
      return file.Count(ch => ch == Path.DirectorySeparatorChar);
    }

    static bool InPath(string keepDirectory, string file)
    {
      return Path.GetFullPath(file).StartsWith(keepDirectory, StringComparison.OrdinalIgnoreCase);
    }
  }
}