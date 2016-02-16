using System;

namespace DuplicateFinder.Core.Deletion
{
  class FileDeleter : IFileDeleter
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

        _output.WriteLine("Deleted {0}", path);
        return bytesDeleted;
      }
      catch (Exception ex)
      {
        _output.WriteLine("ERROR: Could not delete {0}: {1}", path, ex.Message);
      }

      return 0;
    }
  }
}
