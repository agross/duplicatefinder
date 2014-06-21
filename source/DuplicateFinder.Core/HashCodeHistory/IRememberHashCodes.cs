using System;
using System.Collections.Generic;

using DuplicateFinder.Core.HashCodeHistory.Scopes;

namespace DuplicateFinder.Core.HashCodeHistory
{
	public interface IRememberHashCodes
	{
		Func<IScope> ScopeFactory { get; set; }
		IEnumerable<string> Snapshot(IEnumerable<string> hashes);
		void Forget(IEnumerable<string> hashes);
	}
}
