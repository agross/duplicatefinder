using System;
using System.Collections.Generic;

using DuplicateFinder.Core.HashCodeHistory.Scopes;

namespace DuplicateFinder.Core.HashCodeHistory
{
  public class NullHistory : IRememberHashCodes
  {
    public Func<IEnumerable<IHashCodeProvider>, IScope> ScopeFactory { get; set; }

    public IEnumerable<string> Snapshot(IEnumerable<string> hashes, IEnumerable<IHashCodeProvider> hashCodeProviders)
    {
      yield break;
    }

    public void Forget(IEnumerable<string> hashes, IEnumerable<IHashCodeProvider> hashCodeProviders)
    {
    }
  }
}
