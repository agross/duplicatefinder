using System;
using System.Collections.Generic;

using DuplicateFinder.Core.HashCodeHistory.Scopes;

namespace DuplicateFinder.Core.HashCodeHistory
{
	class ReadOnlyHistory : IRememberHashCodes
	{
		readonly IRememberHashCodes _inner;

		public ReadOnlyHistory(IRememberHashCodes inner)
		{
			_inner = inner;

			var scopeFactory = _inner.ScopeFactory;
			_inner.ScopeFactory = () => new ReadOnlyScope(scopeFactory());
		}

		public Func<IScope> ScopeFactory { get; set; }

		IEnumerable<string> IRememberHashCodes.Snapshot(IEnumerable<string> hashes)
		{
			return _inner.Snapshot(hashes);
		}

		public void Forget(IEnumerable<string> hashes)
		{
			_inner.Forget(hashes);
		}
	}
}
