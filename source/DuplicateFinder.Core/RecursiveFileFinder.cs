using System.Collections.Generic;

namespace DuplicateFinder.Core
{
	public class RecursiveFileFinder : IFileFinder
	{
		readonly string _directory;
		readonly IFileSystem _fileSystem;

		public RecursiveFileFinder(IFileSystem fileSystem, string directory)
		{
			_fileSystem = fileSystem;
			_directory = directory;
		}

		public IEnumerable<string> GetFiles()
		{
			return _fileSystem.AllFilesWithin(_directory);
		}
	}
}