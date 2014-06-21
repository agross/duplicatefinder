namespace DuplicateFinder.Core.CommandLine
{
  interface ICommandFactory
  {
    bool CanHandle(string[] args);
    ICommand CreateCommand(string[] args);
  }
}
