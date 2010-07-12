using System.Collections.Generic;
using System.IO;

namespace DuplicateFinder.Core
{
	internal class FileSystem : IFileSystem
	{
		public IEnumerable<string> AllFilesWithin(string path)
		{
			return Directory.GetFiles(path, "*.*", SearchOption.AllDirectories);
		}

		public Stream CreateStreamFrom(string path)
		{
			return File.OpenRead(path);
		}

		public long GetSize(string path)
		{
			return new FileInfo(path).Length;
		}
	}
}