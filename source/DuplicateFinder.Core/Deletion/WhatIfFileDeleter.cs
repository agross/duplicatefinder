using System;

namespace DuplicateFinder.Core.Deletion
{
  class WhatIfFileDeleter : IFileDeleter
  {
    readonly IOutput _output;

    public WhatIfFileDeleter(IOutput output)
    {
      _output = output;
    }

    public long Delete(string path)
    {
      _output.WriteLine(String.Format("WHATIF: Would delete {0}", path));
      return 0;
    }
  }
}
