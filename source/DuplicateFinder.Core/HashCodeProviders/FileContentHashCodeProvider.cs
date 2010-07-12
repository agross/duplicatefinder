using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;

namespace DuplicateFinder.Core.HashCodeProviders
{
	public class FileContentHashCodeProvider : IHashCodeProvider
	{
		static readonly SHA1CryptoServiceProvider Sha;
		readonly IFileSystem _fileSystem;
		readonly Func<Stream, Stream>[] _streamProcessors;

		static FileContentHashCodeProvider()
		{
			Sha = new SHA1CryptoServiceProvider();
		}

		// TODO: Replace Func with one param (that might be null -> no transform)
		public FileContentHashCodeProvider(IFileSystem fileSystem, params Func<Stream, Stream>[] streamProcessors)
		{
			_fileSystem = fileSystem;
			_streamProcessors = streamProcessors.Any() ? streamProcessors : DoNotTransformStream();
		}

		public IEnumerable<string> CalculateHashCode(string path)
		{
			foreach (var processor in _streamProcessors)
			{
				using (var stream = processor(_fileSystem.CreateStreamFrom(path)))
				{
					var hash = Sha.ComputeHash(stream);
					yield return Convert.ToBase64String(hash);
				}
			}
		}

		static Func<Stream, Stream>[] DoNotTransformStream()
		{
			Func<Stream, Stream> passThrough = x => x;
			return new[] { passThrough };
		}
	}
}