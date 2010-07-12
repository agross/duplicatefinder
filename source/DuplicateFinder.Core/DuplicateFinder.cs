using System;
using System.Collections.Generic;
using System.Linq;

using DuplicateFinder.Core.HashCodeProviders;

namespace DuplicateFinder.Core
{
	public class DuplicateFinder : IDuplicateFinder
	{
		readonly IFileFinder[] _fileFinders;
		readonly IHashCodeProvider[] _hashCodeProviders;

		public DuplicateFinder(IFileFinder[] fileFinders, IHashCodeProvider[] hashCodeProviders)
		{
			_fileFinders = fileFinders;
			_hashCodeProviders = hashCodeProviders;
		}

		public IEnumerable<IEnumerable<string>> FindDuplicates()
		{
			return _fileFinders
				.SelectMany(x => x.GetFiles())
				.RemoveDuplicateEntries()
				.Select(x => new { File = x, Hash = HashesOfFile(x) })
				.GroupBy(x => x.Hash, x => x.File, (hash, files) => files)
				.Where(x => x.Count() > 1);
		}

		string HashesOfFile(string x)
		{
			return _hashCodeProviders
				.SelectMany(hcp => hcp.CalculateHashCode(x))
				.Aggregate(String.Empty, (h1, h2) => h1 + h2);
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