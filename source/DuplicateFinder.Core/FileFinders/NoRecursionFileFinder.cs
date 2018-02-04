using System.Collections.Generic;

namespace DuplicateFinder.Core.FileFinders
{
  public class NoRecursionFileFinder : IFileFinder
  {
    readonly IFileSystem _fileSystem;

    public NoRecursionFileFinder(IFileSystem fileSystem, string directory)
    {
      _fileSystem = fileSystem;
      BaseDirectory = directory;
    }

    public string BaseDirectory { get; private set; }

    public IEnumerable<string> GetFiles()
    {
      return _fileSystem.AllFilesWithin(BaseDirectory, false);
    }
  }
}