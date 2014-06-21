using System.Collections.Generic;

namespace DuplicateFinder.Core
{
  public class RecursiveFileFinder : IFileFinder
  {
    readonly IFileSystem _fileSystem;

    public RecursiveFileFinder(IFileSystem fileSystem, string directory)
    {
      _fileSystem = fileSystem;
      BaseDirectory = directory;
    }

    public string BaseDirectory { get; private set; }

    public IEnumerable<string> GetFiles()
    {
      return _fileSystem.AllFilesWithin(BaseDirectory);
    }
  }
}
