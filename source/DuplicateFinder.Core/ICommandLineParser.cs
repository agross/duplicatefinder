namespace DuplicateFinder.Core
{
	public interface ICommandLineParser
	{
		ICommand Parse(string[] args);
	}
}