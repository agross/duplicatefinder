using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using DuplicateFinder.Core.Commands;
using DuplicateFinder.Core.Deletion;
using DuplicateFinder.Core.FileFinders;
using DuplicateFinder.Core.HashCodeHistory;
using DuplicateFinder.Core.HashCodeProviders;
using DuplicateFinder.Core.Streams;

namespace DuplicateFinder.Core.CommandLine.Factories
{
  class FindDuplicatesCommandFactory : ICommandFactory
  {
    readonly IFileSystem _fileSystem;
    readonly ModifyableOptionSet _options;
    readonly IOutput _output;

    public FindDuplicatesCommandFactory(IOutput output, IFileSystem fileSystem, ModifyableOptionSet options)
    {
      _output = output;
      _fileSystem = fileSystem;
      _options = options;
    }

    public bool CanHandle(string[] args)
    {
      return !_options.With(args).Has(Args.PruneHistory);
    }

    public ICommand CreateCommand(string[] args)
    {
      var hashes = new List<Func<IHashCodeProvider>>();
      var decorators = new List<IStreamDecorator>();
      var deletionSelector = (ISelectFilesToDelete) new AllButFirstDuplicateSelector();
      var deleter = (IFileDeleter) new FileDeleter(_fileSystem, _output);
      var history = (IRememberHashCodes) new NullHistory();
      var applyWhatIf = new Action(() => { });
      Func<IFileSystem, string, IFileFinder> fileFinder = (fs, path) => new RecursiveFileFinder(fs, path);

      _options.Update<string>(Args.Name, v => hashes.Add(() => new FileNameHashCodeProvider()));
      _options.Update<string>(Args.Size, v => hashes.Add(() => new FileSizeHashCodeProvider(_fileSystem)));
      _options.Update<string>(Args.Content,
                              v => hashes.Add(() => new FileContentHashCodeProvider(_fileSystem, decorators.ToArray())));
      _options.Update(Args.Head, (long v) => decorators.Add(new HeadStreamDecorator(v)));
      _options.Update(Args.Tail, (long v) => decorators.Add(new TailStreamDecorator(v)));
      _options.Update<string>(Args.Keep, v => deletionSelector = new KeepOneCopyInDirectorySelector(v));
      _options.Update<string>(Args.History, v => history = new DatabaseHistory(v, _fileSystem));
      _options.Update<string>(Args.DoNotRecurseIntoSubdirectories, v => fileFinder = (fs, path) => new NoRecursionFileFinder(fs, path));
      _options.Update<string>(Args.WhatIf,
                              _ =>
                              {
                                applyWhatIf = () =>
                                {
                                  deleter = new WhatIfFileDeleter(_output);
                                  if (history != null)
                                  {
                                    history = new ReadOnlyHistory(history);
                                  }
                                };
                              });

      var directories = _options.Parse(args);
      applyWhatIf();

      var messages = Missing(hashes, "The comparison type is missing")
        .Union(Missing(directories, "No directories to compare"));

      if (messages.Any())
      {
        throw new CommandLineParserException(messages);
      }

      var finder = new DuplicateFinder(directories.Select(x => fileFinder(_fileSystem, x)),
                                       hashes.Select(x => x()),
                                       _output,
                                       history,
                                       new TaskbarProgress());

      return new FindDuplicatesCommand(_output,
                                       finder,
                                       deletionSelector,
                                       deleter);
    }

    static IEnumerable<string> Missing(IEnumerable hashes, string message)
    {
      if (hashes.Cast<object>().Any())
      {
        yield break;
      }

      yield return message;
    }
  }
}
