using DuplicateFinder.Core.Commands;

namespace DuplicateFinder.Core.CommandLine.Factories
{
	class ShowHelpCommandFactory : ICommandFactory
	{
		readonly IOutput _output;
		readonly ModifyableOptionSet _options;

		public ShowHelpCommandFactory(IOutput output, ModifyableOptionSet options)
		{
			_output = output;
			_options = options;
		}

		public bool CanHandle(string[] args)
		{
			var showHelp = false;
			_options.Update<string>(Args.Help, v => showHelp = v != null);
			_options.Parse(args);
			return showHelp;
		}

		public ICommand CreateCommand(string[] args)
		{
			return new ShowHelpCommand(_options, _output, new string[] {});
		}
	}
}