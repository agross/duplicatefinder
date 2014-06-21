using System.Collections.Generic;
using System.Linq;

namespace DuplicateFinder.Core.Deletion
{
  class AllButFirstDuplicateSelector : ISelectFilesToDelete
  {
    public IEnumerable<string> FilesToDelete(IEnumerable<string> duplicates)
    {
      return duplicates.Skip(1);
    }
  }
}
