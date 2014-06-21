using System;
using System.Collections.Generic;

using DuplicateFinder.Core.HashCodeHistory.Scopes;

namespace DuplicateFinder.Core.HashCodeHistory
{
  public interface IRememberHashCodes
  {
    Func<IEnumerable<IHashCodeProvider>, IScope> ScopeFactory { get; set; }
    IEnumerable<string> Snapshot(IEnumerable<string> hashes, IEnumerable<IHashCodeProvider> hashCodeProviders);
    void Forget(IEnumerable<string> hashes, IEnumerable<IHashCodeProvider> hashCodeProviders);
  }
}
