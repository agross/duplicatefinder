using System.Linq;

using DuplicateFinder.Core.Abstractions;
using DuplicateFinder.Core.CommandLine.Factories;
using DuplicateFinder.Core.Commands;

namespace DuplicateFinder.Core.CommandLine
{
	public class CommandLineParser : ICommandLineParser
	{
		readonly ICommandFactory[] _factories;
		readonly FileSystem _fileSystem;
		readonly ModifyableOptionSet _options;
		readonly IOutput _output;

		public CommandLineParser()
		{
			_fileSystem = new FileSystem();
			_output = new ConsoleOutput();

			_options = new ModifyableOptionSet
			           {
			           	{
			           		Args.Name,
			           		"Compare files by name",
			           		v => { }
			           		},
			           	{
			           		Args.Size,
			           		"Compare files by file size",
			           		v => { }
			           		},
			           	{
			           		Args.Content,
			           		"Compare files by content",
			           		v => { }
			           		},
			           	{
			           		Args.Head,
			           		"Compare files by head content (first {N} bytes, requires --content)",
			           		v => { }
			           		},
			           	{
			           		Args.Tail,
			           		"Compare files by tail content (last {N} bytes, requires --content)",
			           		v => { }
			           		},
			           	{
			           		Args.Keep,
			           		"Keeps the first duplicate encountered under {DIRECTORY}, and deletes duplicates from other" +
			           		" directories. If not specified, all but the first duplicate encountered are deleted.",
			           		v => { }
			           		},
			           	{
			           		Args.History,
			           		"Keep a list of seen hashes in {FILE}, deletes files with hashes that reappear after not" +
			           		" being seen at least once",
			           		v => { }
			           		},
			           	{
			           		Args.PruneHistory,
			           		"Remove hashes from the history, does not delete files",
			           		v => { }
			           		},
			           	{
			           		Args.WhatIf,
			           		"Do not delete files",
			           		v => { }
			           		},
			           	{
			           		Args.Help,
			           		"Show this message and exit",
			           		v => { }
			           		},
			           };

			_factories = new ICommandFactory[]
			             {
			             	new PruneHistoryCommandFactory(_output, _fileSystem, _options), 
			             	new FindDuplicatesCommandFactory(_output, _fileSystem, _options),
			             	new ShowHelpCommandFactory(_output, _options)
			             };
		}

		public ICommand Parse(string[] args)
		{
			try
			{
				return _factories.First(x => x.CanHandle(args)).CreateCommand(args);
			}
			catch (CommandLineParserException ex)
			{
				return new ShowHelpCommand(_options, _output, ex.Messages);
			}
		}
	}
}