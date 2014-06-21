using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using DuplicateFinder.Core.Commands;
using DuplicateFinder.Core.HashCodeHistory;
using DuplicateFinder.Core.HashCodeProviders;
using DuplicateFinder.Core.Streams;

namespace DuplicateFinder.Core.CommandLine.Factories
{
  class PruneHistoryCommandFactory : ICommandFactory
  {
    readonly IFileSystem _fileSystem;
    readonly ModifyableOptionSet _options;
    readonly IOutput _output;

    public PruneHistoryCommandFactory(IOutput output, IFileSystem fileSystem, ModifyableOptionSet options)
    {
      _output = output;
      _fileSystem = fileSystem;
      _options = options;
    }

    public bool CanHandle(string[] args)
    {
      return _options.With(args).Has(Args.PruneHistory);
    }

    public ICommand CreateCommand(string[] args)
    {
      var hashes = new List<Func<IHashCodeProvider>>();
      var decorators = new List<IStreamDecorator>();
      IRememberHashCodes history = null;
      var applyWhatIf = new Action(() => { });

      _options.Update<string>(Args.Name, v => hashes.Add(() => new FileNameHashCodeProvider()));
      _options.Update<string>(Args.Size, v => hashes.Add(() => new FileSizeHashCodeProvider(_fileSystem)));
      _options.Update<string>(Args.Content,
                              v => hashes.Add(() => new FileContentHashCodeProvider(_fileSystem, decorators.ToArray())));
      _options.Update(Args.Head, (long v) => decorators.Add(new HeadStreamDecorator(v)));
      _options.Update(Args.Tail, (long v) => decorators.Add(new TailStreamDecorator(v)));
      _options.Update<string>(Args.History, v => { history = new DatabaseHistory(v, _fileSystem); });
      _options.Update<string>(Args.WhatIf,
                              _ =>
                              {
                                applyWhatIf = () =>
                                {
                                  if (history != null)
                                  {
                                    history = new ReadOnlyHistory(history);
                                  }
                                };
                              });

      var directories = _options.Parse(args);
      applyWhatIf();

      var messages = Missing(hashes, "The comparison type is missing")
        .Union(Missing(directories, "No directories to compare"))
        .Union(Missing(history, "No history file was specified"));

      if (messages.Any())
      {
        throw new CommandLineParserException(messages);
      }

      var finder = new DuplicateFinder(directories.Select(x => (IFileFinder) new RecursiveFileFinder(_fileSystem, x)),
                                       hashes.Select(x => x()),
                                       _output,
                                       new NullHistory(),
                                       new TaskbarProgress());

      return new PruneHistoryCommand(_output,
                                     finder,
                                     history);
    }

    static IEnumerable<string> Missing(object instance, string nullReference)
    {
      return Missing(new[] { instance }, nullReference);
    }

    static IEnumerable<string> Missing(IEnumerable hashes, string message)
    {
      if (hashes.Cast<object>().Any(x => x != null))
      {
        yield break;
      }

      yield return message;
    }
  }
}
