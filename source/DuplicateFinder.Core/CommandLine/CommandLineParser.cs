using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DuplicateFinder.Core.Abstractions;
using DuplicateFinder.Core.Commands;
using DuplicateFinder.Core.Deletion;
using DuplicateFinder.Core.HashCodeHistory;
using DuplicateFinder.Core.HashCodeProviders;
using DuplicateFinder.Core.Streams;
using NDesk.Options;

namespace DuplicateFinder.Core.CommandLine
{
	public class CommandLineParser : ICommandLineParser
	{
		readonly FileSystem _fileSystem;
		readonly ConsoleOutput _output;

		public CommandLineParser()
		{
			_fileSystem = new FileSystem();
			_output = new ConsoleOutput();
		}

		public ICommand Parse(string[] args)
		{
			var showHelp = false;
			var hashes = new List<Func<IHashCodeProvider>>();
			var decorators = new List<IStreamDecorator>();
			var deletionSelector = (ISelectFilesToDelete) new AllButFirstDuplicateSelector();
			var deleter = (IFileDeleter) new FileDeleter(_fileSystem, _output);
			var history = (IRememberHashCodes) new NullHistory();

			var options = new OptionSet
			              {
			              	{
			              		"n|name", "Compare files by name",
			              		v => hashes.Add(() => new FileNameHashCodeProvider())
			              		},
			              	{
			              		"s|size", "Compare files by file size",
			              		v => hashes.Add(() => new FileSizeHashCodeProvider(_fileSystem))
			              		},
			              	{
			              		"c|content", "Compare files by content",
			              		v => hashes.Add(() => new FileContentHashCodeProvider(_fileSystem, decorators.ToArray()))
			              		},
			              	{
			              		"h|head|first=", "Compare files by head content (first {N} bytes, requires --content)",
			              		(long v) => decorators.Add(new HeadStreamDecorator(v))
			              		},
			              	{
			              		"t|tail|last=", "Compare files by tail content (last {N} bytes, requires --content)",
			              		(long v) => decorators.Add(new TailStreamDecorator(v))
			              		},
			              	{
			              		"k|keep=",
			              		"Keeps the first duplicate encountered under {DIRECTORY}, and deletes duplicates from other directories. If not specified, all but the first duplicate encountered are deleted.",
			              		v => deletionSelector = new KeepOneCopyInDirectorySelector(v)
			              		},
							{
			              		"history=", "Keep a list of seen hashes in {FILE}, deletes files with hashes that reappear after not being seen at least once",
			              		v => history = new DatabaseHistory(v, _fileSystem)
			              		},
			              	{
			              		"whatif|dry-run", "Do not delete anything",
			              		v => deleter = new WhatIfFileDeleter(_output)
			              		},
			              	{
			              		"?|help", "Show this message and exit",
			              		v => showHelp = v != null
			              		}
			              };

			var directories = options.Parse(args);

			var messages = Missing(hashes, "The comparison type is missing")
				.Union(Missing(directories, "No directories to compare"));

			if (showHelp || messages.Any())
			{
				return new ShowHelpCommand(options, _output, messages.ToArray());
			}

			var finder = new DuplicateFinder(directories.Select(x => (IFileFinder) new RecursiveFileFinder(_fileSystem, x)),
			                                 hashes.Select(x => x()),
			                                 _output,
											 history);

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