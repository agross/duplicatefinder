using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace DuplicateFinder.Core
{
	internal class DuplicateFinder : IDuplicateFinder
	{
		readonly IOutput _output;

		public DuplicateFinder(IEnumerable<IFileFinder> fileFinders,
		                       IEnumerable<IHashCodeProvider> hashCodeProviders,
		                       IOutput output)
		{
			_output = output;
			FileFinders = fileFinders;
			HashCodeProviders = hashCodeProviders;
		}

		public IEnumerable<IFileFinder> FileFinders
		{
			get;
			private set;
		}

		public IEnumerable<IHashCodeProvider> HashCodeProviders
		{
			get;
			private set;
		}

		public IEnumerable<IEnumerable<string>> FindDuplicates()
		{
			var fileList = FileFinders
				.SelectMany(x => x.GetFiles())
				.RemoveDuplicateEntries();

			return fileList
				.Select(x => new { File = x, Hashes = HashesOf(x) })
				.Where(x => x.Hashes.Any())
				.Select(x => new { x.File, Hash = Aggregate(x.Hashes) })
				.GroupBy(x => x.Hash, x => x.File, (hash, files) => files)
				.Where(x => x.Count() > 1);
		}

		IEnumerable<string> HashesOf(string path)
		{
			_output.WriteLine(String.Format("Hashing {0}", path));

			try
			{
				return HashCodeProviders
					.SelectMany(hcp => hcp.CalculateHashCode(path))
					.ToArray();
			}
			catch (IOException)
			{
				_output.WriteLine(String.Format("File is inaccessible or has been deleted: {0}", path));
				return Enumerable.Empty<string>();
			}
		}

		static string Aggregate(IEnumerable<string> hashes)
		{
			return hashes
				.Aggregate(new StringBuilder(), (h1, h2) => h1.Append(h2))
				.ToString();
		}
	}

	internal static partial class EnumerableExtensions
	{
		public static IEnumerable<T> RemoveDuplicateEntries<T>(this IEnumerable<T> instance)
		{
			return instance
				.GroupBy(entry => entry, entry => entry, (entry, entries) => entries)
				.Where(entries => entries.Count() == 1)
				.SelectMany(entries => entries);
		}
	}
}