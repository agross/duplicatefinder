using System.Collections.Generic;

namespace DuplicateFinder.Core
{
	public interface IHashCodeProvider
	{
		IEnumerable<string> CalculateHashCode(string path);
	}
}