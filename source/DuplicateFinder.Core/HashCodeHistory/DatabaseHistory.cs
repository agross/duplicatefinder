using System.Collections.Generic;
using System.Linq;

namespace DuplicateFinder.Core.HashCodeHistory
{
	class DatabaseHistory : IRememberHashCodes
	{
		readonly string _databaseFile;
		static IEnumerable<string> AllSeen = new List<string>();
		static IEnumerable<string> Gone = new List<string>();
		static IEnumerable<string> LastSnapshot = new List<string>();
		static IEnumerable<string> CurrentSnapshot = new List<string>();

		public DatabaseHistory(string databaseFile)
		{
			_databaseFile = databaseFile;
		}

		public string DatabaseFile
		{
			get 
			{
				return _databaseFile;
			}
		}

		public  void Snapshot(IEnumerable<string> hashes)
		{
			AllSeen = AllSeen.Union(hashes);

			LastSnapshot = CurrentSnapshot;
			CurrentSnapshot = hashes;

			Gone = LastSnapshot.Except(CurrentSnapshot).Union(Gone).ToList();

		}

		public IEnumerable<string> Resurrected()
		{
			return Gone.Intersect(CurrentSnapshot);
		}
	}
}