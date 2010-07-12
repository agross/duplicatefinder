using DuplicateFinder.Core;

namespace DuplicateFinder.Console
{
	class Program
	{
		static void Main(string[] args)
		{
			ICommandLineParser parser = new CommandLineParser();
			var command = parser.Parse(args);

			command.Execute();
		}
	}
}
