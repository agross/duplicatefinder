using System;
using System.IO;

namespace DuplicateFinder.Core.Abstractions
{
  class ConsoleOutput : IOutput
  {
    public void WriteLine(string format, params object[] args)
    {
      Console.WriteLine(format, args);
    }

    public TextWriter GetTextWriter()
    {
      return Console.Out;
    }
  }
}
