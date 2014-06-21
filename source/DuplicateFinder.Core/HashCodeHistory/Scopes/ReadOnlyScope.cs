using System.Collections.Generic;

namespace DuplicateFinder.Core.HashCodeHistory.Scopes
{
  class ReadOnlyScope : IScope
  {
    readonly IScope _inner;

    public ReadOnlyScope(IScope inner)
    {
      _inner = inner;
    }

    public void AddSnapshot(IEnumerable<string> snapshot)
    {
      _inner.AddSnapshot(snapshot);
    }

    public void Remove(IEnumerable<string> hashes)
    {
      _inner.Remove(hashes);
    }

    public IEnumerable<string> Resurrected()
    {
      return _inner.Resurrected();
    }

    public void EnsureCompatibilityWith(IEnumerable<IHashCodeProvider> hashCodeProviders)
    {
      _inner.EnsureCompatibilityWith(hashCodeProviders);
    }

    public void Dispose()
    {
    }
  }
}
