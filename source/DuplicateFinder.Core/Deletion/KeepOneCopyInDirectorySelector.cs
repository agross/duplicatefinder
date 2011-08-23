using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace DuplicateFinder.Core.Deletion
{
	internal class KeepOneCopyInDirectorySelector : ISelectFilesToDelete
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
			           duplicates.Last();

			return duplicates.Except(new[] { keep });
		}

		static bool InPath(string keepDirectory, string file)
		{
			return Path.GetFullPath(file).StartsWith(keepDirectory, StringComparison.OrdinalIgnoreCase);
		}
	}
}