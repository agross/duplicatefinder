using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

using DuplicateFinder.Core.HashCodeHistory;

namespace DuplicateFinder.Core
{
	internal class DuplicateFinder : IDuplicateFinder
	{
		readonly IOutput _output;

		public DuplicateFinder(IEnumerable<IFileFinder> fileFinders,
		                       IEnumerable<IHashCodeProvider> hashCodeProviders,
		                       IOutput output,
		                       IRememberHashCodes history)
		{
			_output = output;
			FileFinders = fileFinders;
			HashCodeProviders = hashCodeProviders;
			History = history;
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

		public IRememberHashCodes History
		{
			get;
			private set;
		}

		public FindResult FindDuplicates()
		{
			var fileList = FileFinders
				.SelectMany(x => x.GetFiles())
				.RemoveDuplicateEntries();

			var hashesAndFiles = fileList
				.Select(x => new { File = x, Hashes = HashesOf(x) })
				.Where(x => x.Hashes.Any())
				.Select(x => new { x.File, Hash = Aggregate(x.Hashes) })
				.GroupBy(x => x.Hash, x => x.File)
				.ToList();

			var resurrectedHashes = History.Snapshot(hashesAndFiles.Select(x => x.Key));

			return new FindResult
			       {
			       	Duplicates = hashesAndFiles.Where(x => x.Count() > 1),
			       	Resurrected = hashesAndFiles.Where(x => resurrectedHashes.Any(y => y == x.Key))
			       };
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