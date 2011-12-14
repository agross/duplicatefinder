using DuplicateFinder.Core;
using DuplicateFinder.Core.CommandLine;

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
