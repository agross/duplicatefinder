using System;
using System.Collections.Generic;
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
			var removeDuplicateEntries = FileFinders
				.SelectMany(x => x.GetFiles())
				.RemoveDuplicateEntries();
			return removeDuplicateEntries
				.Select(x => new { File = x, Hash = HashOf(x) })
				.GroupBy(x => x.Hash, x => x.File, (hash, files) => files)
				.Where(x => x.Count() > 1);
		}

		string HashOf(string path)
		{
			_output.WriteLine(String.Format("Hashing {0}", path));

			return HashCodeProviders
				.SelectMany(hcp => hcp.CalculateHashCode(path))
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