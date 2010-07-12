using System.Collections.Generic;
using System.Linq;

using DuplicateFinder.Core.Commands;
using DuplicateFinder.Core.HashCodeProviders;

using Magnum.CommandLineParser;
using Magnum.Monads.Parser;

namespace DuplicateFinder.Core
{
	public class CommandLineParser
	{
		public ICommand Parse(string commandLine)
		{
			return CommandLine.Parse<ICommand>(commandLine, InitializeParser).First();
		}

		static void InitializeParser(ICommandLineElementParser<ICommand> x)
		{
			/*Parser<IEnumerable<ICommandLineElement>, ISwitchElement> switches =
				(from whatIf in x.Switches("whatif") select whatIf);

			Parser<IEnumerable<ICommandLineElement>, IDefinitionElement> contentDefinitions =
				(from first in x.Definition("first") select first)
					.Or(from last in x.Definition("last") select last);

			Parser<IEnumerable<ICommandLineElement>, IDefinitionElement> keepDefinition =
				(from keep in x.Definition("keep") select keep);

			Parser<IEnumerable<ICommandLineElement>, ISwitchElement> comparison =
				(from name in x.Switch("name") select name)
					.Or(from size in x.Switch("size") select size)
					.Or(from content in x.Switch("content") select content);

			x.Add(from find in x.Argument("find")
			      from byName in comparison.Optional("name", false)
			      	.Where(z => z.Value)
			      	.Select(z => new FileNameHashCodeProvider())
			      from bySize in comparison.Optional("size", false)
			      	.Where(z => z.Value)
			      	.Select(z => new FileSizeHashCodeProvider(new FileSystem()))
			      from byContent in comparison.Optional("content", false)
			      	.Where(z => z.Value)
			      	.Select(z => new FileContentHashCodeProvider(new FileSystem()))
			      from dir1 in x.Argument()
			      from dir2 in x.Argument()
			      select (ICommand) new FindDuplicatesCommand(new IHashCodeProvider[] { byName, bySize, byContent }, 
					  dir1.Id, 
					  dir2.Id));

			x.Add(from find in x.Argument("delete")
			      from byName in comparison.Optional("name", false)
			      	.Where(z => z.Value)
			      	.Select(z => new FileNameHashCodeProvider())
			      from bySize in comparison.Optional("size", false)
			      	.Where(z => z.Value)
			      	.Select(z => new FileSizeHashCodeProvider(new FileSystem()))
			      from byContent in comparison.Optional("content", false)
			      	.Where(z => z.Value)
			      	.Select(z => new FileContentHashCodeProvider(new FileSystem()))
			      from keep in keepDefinition.Optional("keep", null)
			      from whatIf in switches.Optional("whatif", false)
			      from dir1 in x.Argument()
			      from dir2 in x.Argument()
			      select (ICommand) new DeleteDuplicatesCommand(new IHashCodeProvider[] { byName, bySize, byContent },
			                                                    dir1.Id,
			                                                    dir2.Id,
			                                                    keep.Value,
			                                                    whatIf.Value));*/
		}
	}
}