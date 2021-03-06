using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

using DuplicateFinder.Core.HashCodeHistory;

namespace DuplicateFinder.Core
{
  class DuplicateFinder : IDuplicateFinder
  {
    readonly IOutput _output;
    readonly IDisplayProgress _progress;

    public DuplicateFinder(IEnumerable<IFileFinder> fileFinders,
                           IEnumerable<IHashCodeProvider> hashCodeProviders,
                           IOutput output,
                           IRememberHashCodes history,
                           IDisplayProgress progress)
    {
      _output = output;
      _progress = progress;
      FileFinders = fileFinders;
      HashCodeProviders = hashCodeProviders.OrderBy(x => x.GetType().AssemblyQualifiedName);
      History = history;
    }

    public IEnumerable<IFileFinder> FileFinders { get; private set; }

    public IRememberHashCodes History { get; private set; }
    public IEnumerable<IHashCodeProvider> HashCodeProviders { get; private set; }

    public IEnumerable<IGrouping<string, string>> CalculateHashes()
    {
      try
      {
        _progress.Intermediate();

        var fileList = FileFinders
          .SelectMany(x => x.GetFiles())
          .RemoveDuplicateEntries()
          .ToList();

        var currentFile = 0;
        var totalFiles = fileList.Count;

        var filesByDrive = new FilesByDrive(fileList);
        return filesByDrive
          .AsParallel()
          .Select(x =>
          {
            _progress.Percent(++currentFile, totalFiles);
            return new { File = x, Hashes = HashesOf(x) };
          })
          .Where(x => x.Hashes.Any())
          .Select(x => new { x.File, Hash = Aggregate(x.Hashes) })
          .GroupBy(x => x.Hash, x => x.File)
          .ToList();
      }
      finally
      {
        _progress.Stop();
      }
    }

    public FindResult FindDuplicates()
    {
      var hashesAndFiles = CalculateHashes();

      var resurrectedHashes = History.Snapshot(hashesAndFiles.Select(x => x.Key), HashCodeProviders);

      return new FindResult
             {
               Duplicates = hashesAndFiles.Where(x => x.Count() > 1),
               Resurrected = hashesAndFiles.Where(x => resurrectedHashes.Any(y => y == x.Key))
             };
    }

    IEnumerable<string> HashesOf(string path)
    {
      _output.WriteLine("Hashing {0}", path);

      try
      {
        return HashCodeProviders
          .SelectMany(hcp => hcp.CalculateHashCode(path))
          .ToArray();
      }
      catch (IOException)
      {
        _output.WriteLine("File is inaccessible or has been deleted: {0}", path);
        return Enumerable.Empty<string>();
      }
    }

    static string Aggregate(IEnumerable<string> hashes)
    {
      return hashes
        .Aggregate(new StringBuilder(), (h1, h2) => h1.Append(h2))
        .ToString();
    }

    internal class FilesByDrive : Partitioner<string>
    {
      readonly IEnumerable<string> _files;

      public FilesByDrive(IEnumerable<string> files)
      {
        _files = files;
      }

      public override bool SupportsDynamicPartitions
      {
        get
        {
          return false;
        }
      }

      public override IList<IEnumerator<string>> GetPartitions(int partitionCount)
      {
        var drives = _files
          .GroupBy(Path.GetPathRoot)
          .OrderBy(x => x.Key)
          .Select(grouping => grouping.AsEnumerable())
          .ToList();

        IEnumerable<IEnumerable<string>> result = drives;

        if (partitionCount > drives.Count)
        {
          var fillUpToPartitionCount = Enumerable.Repeat(Enumerable.Empty<string>(),
                                                         partitionCount - drives.Count);

          result = drives.Concat(fillUpToPartitionCount);
        }

        if (partitionCount < drives.Count)
        {
          var matchedPartitions = drives.Take(partitionCount);
          var unmatchedPartitions = drives
            .Skip(partitionCount)
            .SelectMany(x => x)
            .Split(partitionCount);

          result = matchedPartitions
            .Select((partition, index) =>
            {
              var rest = unmatchedPartitions.Skip(index).First();
              return partition.Concat(rest).ToList();
            });
        }

        return result
          .Select(byDrive => byDrive.GetEnumerator())
          .ToList();
      }
    }
  }

  static class EnumerableExtensions
  {
    public static IEnumerable<IEnumerable<T>> Split<T>(this IEnumerable<T> source, int partitions)
    {
      return source
        .Select((value, index) => new { x = value, index })
        .GroupBy(x => x.index % partitions)
        .Select(x => x.Select(group => group.x).ToList())
        .ToList();
    }

    public static IEnumerable<T> RemoveDuplicateEntries<T>(this IEnumerable<T> instance)
    {
      return instance
        .GroupBy(entry => entry, entry => entry, (entry, entries) => entries)
        .Where(entries => entries.Count() == 1)
        .SelectMany(entries => entries);
    }
  }
}
