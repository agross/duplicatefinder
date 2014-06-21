using System;
using System.Collections.Generic;

using DuplicateFinder.Core.HashCodeHistory.Scopes;

namespace DuplicateFinder.Core.HashCodeHistory
{
	public class NullHistory : IRememberHashCodes
	{
		public Func<IScope> ScopeFactory { get; set; }

		public IEnumerable<string> Snapshot(IEnumerable<string> hashes)
		{
			yield break;
		}

		public void Forget(IEnumerable<string> hashes)
		{
		}
	}
}
