﻿using System;
using System.IO;

namespace DuplicateFinder.Core.Streams
{
  class TailStreamDecorator : IStreamDecorator
  {
    readonly long _length;

    public TailStreamDecorator(long length)
    {
      _length = length;
    }

    public Stream GetStream(Stream stream)
    {
      return new TailStream(stream, _length);
    }

    public override string ToString()
    {
      return String.Format("tail({0})", _length);
    }
  }
}
