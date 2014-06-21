using System;
using System.Collections.Generic;
using System.Linq;

namespace DuplicateFinder.Core.HashCodeHistory
{
    public class ReadOnlyHistory : IRememberHashCodes
    {
        readonly IRememberHashCodes _inner;
        readonly IOutput _output;

        public ReadOnlyHistory(IRememberHashCodes inner, IOutput output)
        {
            _inner = inner;
            _output = output;
        }

        public IEnumerable<string> Snapshot(IEnumerable<string> hashes)
        {
            return _inner.Snapshot(hashes);
        }

        public void Forget(IEnumerable<string> hashes)
        {
            _output.WriteLine(String.Format("WHATIF: Would forget {0} hashes.", hashes.Count()));
        }
    }
}
