using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;

namespace DuplicateFinder.Core.HashCodeProviders
{
	internal class FileContentHashCodeProvider : IHashCodeProvider
	{
		static readonly SHA1CryptoServiceProvider Sha;
		readonly IFileSystem _fileSystem;

		static FileContentHashCodeProvider()
		{
			Sha = new SHA1CryptoServiceProvider();
		}

		public FileContentHashCodeProvider(IFileSystem fileSystem, params IStreamDecorator[] streamDecorators)
		{
			_fileSystem = fileSystem;
			StreamDecorators = streamDecorators;
		}

		public IStreamDecorator[] StreamDecorators
		{
			get;
			private set;
		}

		public IEnumerable<string> CalculateHashCode(string path)
		{
			foreach (var stream in StreamsFor(path))
			{
				using(stream)
				{
					var hash = Sha.ComputeHash(stream);
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
	}
}