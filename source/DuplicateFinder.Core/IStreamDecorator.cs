using System.IO;

namespace DuplicateFinder.Core
{
  public interface IStreamDecorator
  {
    Stream GetStream(Stream stream);
  }
}
