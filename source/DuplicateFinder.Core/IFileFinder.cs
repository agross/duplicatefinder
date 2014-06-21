using System.Collections.Generic;

namespace DuplicateFinder.Core
{
  public interface IFileFinder
  {
    IEnumerable<string> GetFiles();
  }
}
