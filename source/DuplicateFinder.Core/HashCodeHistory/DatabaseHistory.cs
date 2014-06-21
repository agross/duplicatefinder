using System;
using System.Collections.Generic;
using System.Linq;

using DuplicateFinder.Core.HashCodeHistory.Scopes;

namespace DuplicateFinder.Core.HashCodeHistory
{
	class DatabaseHistory : IRememberHashCodes
	{
		public DatabaseHistory(string databaseFile, IFileSystem fileSystem)
		{
			DatabaseFile = databaseFile;
			ScopeFactory = p => new Scope(fileSystem, DatabaseFile, p);
		}

		public string DatabaseFile { get; private set; }

		public Func<IEnumerable<IHashCodeProvider>, IScope> ScopeFactory { get; set; }

		public IEnumerable<string> Snapshot(IEnumerable<string> hashes, IEnumerable<IHashCodeProvider> hashCodeProviders)
		{
			using (var scope = ScopeFactory(hashCodeProviders))
			{
				var snapshot = hashes.ToList();
				scope.AddSnapshot(snapshot);

				return scope.Resurrected();
			}
		}

		public void Forget(IEnumerable<string> hashes, IEnumerable<IHashCodeProvider> hashCodeProviders)
		{
			using (var scope = ScopeFactory(hashCodeProviders))
			{
				scope.Remove(hashes);
			}
		}
	}
}
