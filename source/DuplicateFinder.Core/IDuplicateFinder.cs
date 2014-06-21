using System.Collections.Generic;
using System.Linq;

namespace DuplicateFinder.Core
{
	public interface IDuplicateFinder
	{
		FindResult FindDuplicates();
		IEnumerable<IGrouping<string, string>> CalculateHashes();
		IEnumerable<IHashCodeProvider> HashCodeProviders { get; }
	}

	public class FindResult
	{
		public FindResult()
		{
			Duplicates = Enumerable.Empty<IEnumerable<string>>();
			Resurrected = Enumerable.Empty<IEnumerable<string>>();
		}

		public IEnumerable<IEnumerable<string>> Duplicates
		{
			get;
			set;
		}
		
		public IEnumerable<IEnumerable<string>> Resurrected
		{
			get;
			set;
		}
	}
}