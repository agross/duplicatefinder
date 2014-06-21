using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;

namespace DuplicateFinder.Core.HashCodeProviders
{
  class FileContentHashCodeProvider : IHashCodeProvider
  {
    readonly IFileSystem _fileSystem;
    readonly SHA1CryptoServiceProvider _sha;

    public FileContentHashCodeProvider(IFileSystem fileSystem, params IStreamDecorator[] streamDecorators)
    {
      _sha = new SHA1CryptoServiceProvider();
      _fileSystem = fileSystem;
      StreamDecorators = streamDecorators;
    }

    public IStreamDecorator[] StreamDecorators { get; private set; }

    public IEnumerable<string> CalculateHashCode(string path)
    {
      foreach (var stream in StreamsFor(path))
      {
        using (stream)
        {
          var hash = _sha.ComputeHash(stream);
          yield return Convert.ToBase64String(hash);
        }
      }
    }

    IEnumerable<Stream> StreamsFor(string path)
    {
      if (!StreamDecorators.Any())
      {
        yield return _fileSystem.CreateStreamFrom(path);
        yield break;
      }

      foreach (var decorator in StreamDecorators)
      {
        yield return decorator.GetStream(_fileSystem.CreateStreamFrom(path));
      }
    }

    public override string ToString()
    {
      return String.Format("content " + String.Join(" ", StreamDecorators.Select(x => x.ToString())));
    }
  }
}
