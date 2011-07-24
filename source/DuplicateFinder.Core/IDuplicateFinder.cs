using System.Collections.Generic;
using System.Linq;

namespace DuplicateFinder.Core
{
	public interface IDuplicateFinder
	{
		FindResult FindDuplicates();
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