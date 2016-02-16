using System.IO;

using JetBrains.Annotations;

namespace DuplicateFinder.Core
{
  public interface IOutput
  {
    [StringFormatMethod("format")]
    void WriteLine(string format, params object[] args);
    TextWriter GetTextWriter();
  }
}
