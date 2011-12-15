using System.Collections.Generic;

namespace DuplicateFinder.Core.HashCodeHistory
{
	public class NullHistory : IRememberHashCodes
	{
		public IEnumerable<string> Snapshot(IEnumerable<string> hashes)
		{
			yield break;
		}

		public void Forget(IEnumerable<string> hashes)
		{
		}
	}
}