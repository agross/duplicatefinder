using System.Collections.Generic;
using System.IO;

namespace DuplicateFinder.Core.Abstractions
{
	internal class FileSystem : IFileSystem
	{
		public IEnumerable<string> AllFilesWithin(string path)
		{
			return Directory.GetFiles(path, "*.*", SearchOption.AllDirectories);
		}

		public Stream CreateStreamFrom(string path)
		{
			return new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
		}

		public long GetSize(string path)
		{
			return new FileInfo(path).Length;
		}

		public void Delete(string path)
		{
			File.SetAttributes(path, FileAttributes.Normal);
			File.Delete(path);
		}
	}
}