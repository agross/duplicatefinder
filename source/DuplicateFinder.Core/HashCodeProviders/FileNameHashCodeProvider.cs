using System.Collections.Generic;
using System.IO;

namespace DuplicateFinder.Core.HashCodeProviders
{
  class FileNameHashCodeProvider : IHashCodeProvider
  {
    public IEnumerable<string> CalculateHashCode(string path)
    {
      yield return Path.GetFileName(path);
    }

    public override string ToString()
    {
      return "name";
    }
  }
}
