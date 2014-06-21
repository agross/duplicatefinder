using System.Collections.Generic;
using System.Linq;

namespace DuplicateFinder.Core.HashCodeHistory.Scopes
{
    class Scope : IScope
    {
        IEnumerable<string> _allSeen = new List<string>();
        IEnumerable<string> _allGone = new List<string>();
        IEnumerable<string> _gone = new List<string>();
        IEnumerable<string> _currentSnapshot = new List<string>();

        readonly IFileSystem _fileSystem;
        readonly string _databaseFile;

        public Scope(IFileSystem fileSystem, string databaseFile)
        {
            _fileSystem = fileSystem;
            _databaseFile = databaseFile;

            _allSeen = _fileSystem.ReadAllLines(SeenPath);
            _allGone = _fileSystem.ReadAllLines(GonePath);
            _currentSnapshot = _fileSystem.ReadAllLines(CurrentPath);
        }

        public void Dispose()
        {
            _fileSystem.WriteAllLines(SeenPath, _allSeen);
            _fileSystem.WriteAllLines(GonePath, AllGone);
            _fileSystem.WriteAllLines(CurrentPath, CurrentSnapshot);	
        }

        string GonePath
        {
            get { return _databaseFile + "-gone"; }
        }

        string SeenPath
        {
            get { return _databaseFile + "-seen"; }
        }
			
        string CurrentPath
        {
            get { return _databaseFile + "-current"; }
        }

        IEnumerable<string> AllGone
        {
            get { return _allGone; }
        }

        IEnumerable<string> CurrentSnapshot
        {
            get { return _currentSnapshot; }
        }

        public void AddSnapshot(IEnumerable<string> snapshot)
        {
            var hashes = snapshot.ToList();

            _allSeen = _allSeen.Union(hashes).ToList();
				
            var lastSnapshot = _currentSnapshot;
            _currentSnapshot = hashes;

            _gone = lastSnapshot.Except(CurrentSnapshot).ToList();
            _allGone = _allGone.Union(_gone).ToList();
        }
			
        public void Remove(IEnumerable<string> hashes)
        {
            hashes = hashes.ToList();

            _allSeen = _allSeen.Except(hashes).ToList();
            _allGone = _allGone.Except(hashes).ToList();
            _currentSnapshot = _currentSnapshot.Except(hashes).ToList();
        }

        public IEnumerable<string> Resurrected()
        {
            return AllGone.Intersect(CurrentSnapshot).ToList();
        }
    }
}