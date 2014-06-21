using System.Collections.Generic;

namespace DuplicateFinder.Core
{
  public interface ISelectFilesToDelete
  {
    IEnumerable<string> FilesToDelete(IEnumerable<string> duplicates);
  }
}
