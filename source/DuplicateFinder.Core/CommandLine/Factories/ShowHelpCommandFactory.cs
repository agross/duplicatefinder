using DuplicateFinder.Core.Commands;

namespace DuplicateFinder.Core.CommandLine.Factories
{
  class ShowHelpCommandFactory : ICommandFactory
  {
    readonly ModifyableOptionSet _options;
    readonly IOutput _output;

    public ShowHelpCommandFactory(IOutput output, ModifyableOptionSet options)
    {
      _output = output;
      _options = options;
    }

    public bool CanHandle(string[] args)
    {
      return true;
    }

    public ICommand CreateCommand(string[] args)
    {
      return new ShowHelpCommand(_options, _output, new string[] { });
    }
  }
}
