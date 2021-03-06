﻿using System.Collections.Generic;
using System.Globalization;

namespace DuplicateFinder.Core.HashCodeProviders
{
  class FileSizeHashCodeProvider : IHashCodeProvider
  {
    readonly IFileSystem _fileSystem;

    public FileSizeHashCodeProvider(IFileSystem fileSystem)
    {
      _fileSystem = fileSystem;
    }

    public IEnumerable<string> CalculateHashCode(string path)
    {
      yield return _fileSystem.GetSize(path).ToString(CultureInfo.InvariantCulture);
    }

    public override string ToString()
    {
      return "size";
    }
  }
}
