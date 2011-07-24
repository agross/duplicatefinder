using System.Collections.Generic;

namespace DuplicateFinder.Core.HashCodeHistory
{
	public interface IRememberHashCodes
	{
		void Snapshot(IEnumerable<string> hashes);
		IEnumerable<string> Resurrected();
	}
}