using System;
using System.Collections.Generic;
using System.Linq;

using DuplicateFinder.Core.HashCodeHistory.Scopes;

namespace DuplicateFinder.Core.HashCodeHistory
{
	class DatabaseHistory : IRememberHashCodes
	{
		readonly string _databaseFile;
		readonly IFileSystem _fileSystem;

		public DatabaseHistory(string databaseFile, IFileSystem fileSystem)
		{
			_databaseFile = databaseFile;
			_fileSystem = fileSystem;
			ScopeFactory = () => new Scope(_fileSystem, DatabaseFile);
		}

		public string DatabaseFile
		{
			get
			{
				return _databaseFile;
			}
		}

		public Func<IScope> ScopeFactory { get; set; }

		public IEnumerable<string> Snapshot(IEnumerable<string> hashes)
		{
			using (var scope = ScopeFactory())
			{
				var snapshot = hashes.ToList();
				scope.AddSnapshot(snapshot);

				return scope.Resurrected();
			}
		}

		public void Forget(IEnumerable<string> hashes)
		{
			using (var scope = ScopeFactory())
			{
				scope.Remove(hashes);
			}
		}
	}
}
