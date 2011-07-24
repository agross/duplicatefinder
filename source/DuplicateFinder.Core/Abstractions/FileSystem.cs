using System.Collections.Generic;
using System.IO;
using System.Linq;

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

		public IEnumerable<string> ReadAllLines(string path)
		{
			if (File.Exists(path))
			{
				return File.ReadAllLines(path);
			}

			return Enumerable.Empty<string>();
		}

		public void WriteAllLines(string path, IEnumerable<string> lines)
		{
			File.WriteAllLines(path, lines);
		}

		public bool Exists(string path)
		{
			return File.Exists(path);
		}
	}
}