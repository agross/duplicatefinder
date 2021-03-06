﻿using System.Diagnostics;
using System.Linq;

namespace DuplicateFinder.Core.Commands
{
  class FindDuplicatesCommand : ICommand
  {
    readonly IOutput _output;
    readonly ISelectFilesToDelete _select;

    public FindDuplicatesCommand(IOutput output,
                                 IDuplicateFinder duplicateFinder,
                                 ISelectFilesToDelete deletionSelector,
                                 IFileDeleter fileDeleter)
    {
      DuplicateFinder = duplicateFinder;
      FileDeleter = fileDeleter;
      _output = output;
      _select = deletionSelector;
    }

    public IDuplicateFinder DuplicateFinder { get; private set; }

    public FindResult Results { get; private set; }

    public IFileDeleter FileDeleter { get; private set; }

    public void Execute()
    {
      var timer = new Stopwatch();
      timer.Start();

      try
      {
        Results = DuplicateFinder.FindDuplicates();
        var bytesDeleted = Results.Duplicates
                                  .Select(x =>
                                  {
                                    var delete = _select.FilesToDelete(x);
                                    var keep = x.Except(delete);

                                    return new { Keep = keep, Delete = delete };
                                  })
                                  .SelectMany(x =>
                                  {
                                    foreach (var keep in x.Keep)
                                    {
                                      _output.WriteLine("Keeping {0}", keep);
                                    }

                                    return x.Delete;
                                  })
                                  .Concat(Results.Resurrected.SelectMany(x => x))
                                  .Distinct()
                                  .Select(FileDeleter.Delete)
                                  .Sum();

        _output.WriteLine("{0} deleted.", bytesDeleted.ToFileSize());
      }
      finally
      {
        timer.Stop();
        _output.WriteLine("Took {0}", timer.Elapsed);
      }
    }
  }
}
