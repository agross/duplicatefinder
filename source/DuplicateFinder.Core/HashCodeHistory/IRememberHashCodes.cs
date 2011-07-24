using System.Collections.Generic;

namespace DuplicateFinder.Core.HashCodeHistory
{
	public interface IRememberHashCodes
	{
		IEnumerable<string> Snapshot(IEnumerable<string> hashes);
	}
}