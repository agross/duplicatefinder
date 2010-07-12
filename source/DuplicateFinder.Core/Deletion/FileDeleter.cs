using System;

namespace DuplicateFinder.Core.Deletion
{
	internal class FileDeleter : IFileDeleter
	{
		readonly IFileSystem _fileSystem;
		readonly IOutput _output;

		public FileDeleter(IFileSystem fileSystem, IOutput output)
		{
			_fileSystem = fileSystem;
			_output = output;
		}

		public long Delete(string path)
		{
			try
			{
				var bytesDeleted = _fileSystem.GetSize(path);
				_fileSystem.Delete(path);

				_output.WriteLine(String.Format("Deleted {0}", path));
				return bytesDeleted;
			}
			catch (Exception ex)
			{
				_output.WriteLine(String.Format("ERROR: Could not delete {0}: {1}", path, ex.Message));
			}

			return 0;
		}
	}
}