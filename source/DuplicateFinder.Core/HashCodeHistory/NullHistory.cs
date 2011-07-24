using System.Collections.Generic;

namespace DuplicateFinder.Core.HashCodeHistory
{
	public class NullHistory : IRememberHashCodes
	{
		public void Snapshot(IEnumerable<string> hashes)
		{
		}

		public IEnumerable<string> Resurrected()
		{
			yield break;
		}
	}
}