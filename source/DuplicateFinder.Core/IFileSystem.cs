using System.Collections.Generic;
using System.IO;

namespace DuplicateFinder.Core
{
	public interface IFileSystem
	{
		IEnumerable<string> AllFilesWithin(string path);
		Stream CreateStreamFrom(string path);
		long GetSize(string path);
		void Delete(string path);
	}
}