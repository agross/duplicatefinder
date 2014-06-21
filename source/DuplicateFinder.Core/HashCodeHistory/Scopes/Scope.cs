using System;
using System.Collections.Generic;
using System.Linq;

namespace DuplicateFinder.Core.HashCodeHistory.Scopes
{
  class Scope : IScope
  {
    readonly string _databaseFile;
    readonly IFileSystem _fileSystem;
    IEnumerable<string> _allGone = new List<string>();
    IEnumerable<string> _allSeen = new List<string>();
    IEnumerable<string> _currentSnapshot = new List<string>();
    IEnumerable<string> _gone = new List<string>();
    IEnumerable<string> _parameters = new List<string>();

    public Scope(IFileSystem fileSystem, string databaseFile, IEnumerable<IHashCodeProvider> hashCodeProviders)
    {
      _fileSystem = fileSystem;
      _databaseFile = databaseFile;

      _allSeen = _fileSystem.ReadAllLines(SeenPath);
      _allGone = _fileSystem.ReadAllLines(GonePath);
      _currentSnapshot = _fileSystem.ReadAllLines(CurrentPath);
      _parameters = _fileSystem.ReadAllLines(ParametersPath);

      EnsureCompatibilityWith(hashCodeProviders);
    }

    string GonePath
    {
      get
      {
        return _databaseFile + "-gone";
      }
    }

    string SeenPath
    {
      get
      {
        return _databaseFile + "-seen";
      }
    }

    string CurrentPath
    {
      get
      {
        return _databaseFile + "-current";
      }
    }

    string ParametersPath
    {
      get
      {
        return _databaseFile + "-parameters";
      }
    }

    IEnumerable<string> AllGone
    {
      get
      {
        return _allGone;
      }
    }

    IEnumerable<string> CurrentSnapshot
    {
      get
      {
        return _currentSnapshot;
      }
    }

    public void Dispose()
    {
      _fileSystem.WriteAllLines(SeenPath, _allSeen);
      _fileSystem.WriteAllLines(GonePath, AllGone);
      _fileSystem.WriteAllLines(CurrentPath, CurrentSnapshot);
      _fileSystem.WriteAllLines(ParametersPath, _parameters);
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

    public void EnsureCompatibilityWith(IEnumerable<IHashCodeProvider> hashCodeProviders)
    {
      var expectedInHistory = hashCodeProviders.Select(x => x.ToString()).ToList();

      if (!_parameters.Any())
      {
        _parameters = expectedInHistory;
        // For backwards compatibility, do not throw if we don't have any parameters.
        return;
      }

      var onlyInHistory = _parameters.Except(expectedInHistory);
      var onlyOnCommandLine = expectedInHistory.Except(_parameters);

      if (onlyInHistory.Any() || onlyOnCommandLine.Any())
      {
        var both = _parameters.Where(expectedInHistory.Contains);

        var message =
          String.Format("The history file is not compatible with the parameters passed on the command line. " +
                        "The history was created with {1} and the command line specified {2}.{0}" +
                        "Only in history: {3}{0}" +
                        "Only on command line: {4}{0}" +
                        "Both: {5}",
                        Environment.NewLine,
                        String.Join(", ", _parameters),
                        String.Join(", ", expectedInHistory),
                        String.Join(", ", onlyInHistory),
                        String.Join(", ", onlyOnCommandLine),
                        String.Join(", ", both));

        throw new ArgumentException(message);
      }
    }
  }
}
