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
      _inner.ScopeFactory = p => new ReadOnlyScope(scopeFactory(p));
    }

    public Func<IEnumerable<IHashCodeProvider>, IScope> ScopeFactory { get; set; }

    public IEnumerable<string> Snapshot(IEnumerable<string> hashes, IEnumerable<IHashCodeProvider> hashCodeProviders)
    {
      return _inner.Snapshot(hashes, hashCodeProviders);
    }

    public void Forget(IEnumerable<string> hashes, IEnumerable<IHashCodeProvider> hashCodeProviders)
    {
      _inner.Forget(hashes, hashCodeProviders);
    }
  }
}
