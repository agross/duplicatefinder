using System.Collections.Generic;

namespace DuplicateFinder.Core
{
	public interface IDuplicateFinder
	{
		IEnumerable<IEnumerable<string>> FindDuplicates();
	}
}