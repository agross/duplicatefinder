using System;
using System.IO;

namespace DuplicateFinder.Core.Streams
{
  class HeadStreamDecorator : IStreamDecorator
  {
    readonly long _length;

    public HeadStreamDecorator(long length)
    {
      _length = length;
    }

    public Stream GetStream(Stream stream)
    {
      return new HeadStream(stream, _length);
    }

    public override string ToString()
    {
      return String.Format("head({0})", _length);
    }
  }
}
