using System;
using System.Collections.Generic;

namespace DuplicateFinder.Core.HashCodeHistory.Scopes
{
    public interface IScope : IDisposable
    {
	    void AddSnapshot(IEnumerable<string> snapshot);
	    void Remove(IEnumerable<string> hashes);
	    IEnumerable<string> Resurrected();
    }
}
