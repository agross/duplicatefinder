using System.Collections;
using System.Linq;

using DuplicateFinder.Core.Commands;
using DuplicateFinder.Core.HashCodeProviders;

using Machine.Specifications;

namespace DuplicateFinder.Core
{
/*	[Subject(typeof(CommandLineParser))]
	public class When_finding_duplicates_by_name_size_and_contents
	{
		static ICommand Command;
		static CommandLineParser CommandLine;
		static FindDuplicatesCommand FindCommand;

		Establish context = () => { CommandLine = new CommandLineParser(); };

		Because of = () =>
			{
				Command = CommandLine.Parse(@"find --name --size --content c:\1 c:\2");
				FindCommand = Command as FindDuplicatesCommand;
			};

		It should_be_able_to_parse_the_command_line =
			() => Command.ShouldNotBeNull();

		It should_create_the_find_command =
			() => Command.ShouldBeOfType<FindDuplicatesCommand>();

		It should_parse_the_directories_to_scan =
			() => FindCommand.Directories.ShouldContainOnly(@"c:\1", @"c:\2");

		It should_parse_the_name_hash_code_provider =
			() => FindCommand.HashCodeProviders.ShouldContainInstanceOf<FileNameHashCodeProvider>();

		It should_parse_the_size_hash_code_provider =
			() => FindCommand.HashCodeProviders.ShouldContainInstanceOf<FileSizeHashCodeProvider>();

		It should_parse_the_content_hash_code_provider =
			() => FindCommand.HashCodeProviders.ShouldContainInstanceOf<FileContentHashCodeProvider>();
	}
	
	[Subject(typeof(CommandLineParser))]
	public class When_finding_duplicates_by_name
	{
		static ICommand Command;
		static CommandLineParser CommandLine;
		static FindDuplicatesCommand FindCommand;

		Establish context = () => { CommandLine = new CommandLineParser(); };

		Because of = () =>
			{
				Command = CommandLine.Parse(@"find --name c:\1 c:\2");
				FindCommand = Command as FindDuplicatesCommand;
			};

		It should_be_able_to_parse_the_command_line =
			() => Command.ShouldNotBeNull();

		It should_create_the_find_command =
			() => Command.ShouldBeOfType<FindDuplicatesCommand>();

		It should_parse_the_directories_to_scan =
			() => FindCommand.Directories.ShouldContainOnly(@"c:\1", @"c:\2");

		It should_parse_the_name_hash_code_provider =
			() => FindCommand.HashCodeProviders.ShouldContainInstanceOf<FileNameHashCodeProvider>();

		It should_parse_no_other_hash_code_provider =
			() => FindCommand.HashCodeProviders.Count().ShouldEqual(1);
	}
	
	[Subject(typeof(CommandLineParser))]
	public class When_finding_duplicates_by_size
	{
		static ICommand Command;
		static CommandLineParser CommandLine;
		static FindDuplicatesCommand FindCommand;

		Establish context = () => { CommandLine = new CommandLineParser(); };

		Because of = () =>
			{
				Command = CommandLine.Parse(@"find --size c:\1 c:\2");
				FindCommand = Command as FindDuplicatesCommand;
			};

		It should_be_able_to_parse_the_command_line =
			() => Command.ShouldNotBeNull();

		It should_create_the_find_command =
			() => Command.ShouldBeOfType<FindDuplicatesCommand>();

		It should_parse_the_directories_to_scan =
			() => FindCommand.Directories.ShouldContainOnly(@"c:\1", @"c:\2");

		It should_parse_the_size_hash_code_provider =
			() => FindCommand.HashCodeProviders.ShouldContainInstanceOf<FileSizeHashCodeProvider>();

		It should_parse_no_other_hash_code_provider =
			() => FindCommand.HashCodeProviders.Count().ShouldEqual(1);
	}
	
	[Subject(typeof(CommandLineParser))]
	public class When_finding_duplicates_by_content
	{
		static ICommand Command;
		static CommandLineParser CommandLine;
		static FindDuplicatesCommand FindCommand;

		Establish context = () => { CommandLine = new CommandLineParser(); };

		Because of = () =>
			{
				Command = CommandLine.Parse(@"find --content c:\1 c:\2");
				FindCommand = Command as FindDuplicatesCommand;
			};

		It should_be_able_to_parse_the_command_line =
			() => Command.ShouldNotBeNull();

		It should_create_the_find_command =
			() => Command.ShouldBeOfType<FindDuplicatesCommand>();

		It should_parse_the_directories_to_scan =
			() => FindCommand.Directories.ShouldContainOnly(@"c:\1", @"c:\2");

		It should_parse_the_content_hash_code_provider =
			() => FindCommand.HashCodeProviders.ShouldContainInstanceOf<FileContentHashCodeProvider>();

		It should_parse_no_other_hash_code_provider =
			() => FindCommand.HashCodeProviders.Count().ShouldEqual(1);
	}

	[Subject(typeof(CommandLineParser))]
	public class When_deleting_duplicates
	{
		static ICommand Command;
		static CommandLineParser CommandLine;
		static DeleteDuplicatesCommand DeleteCommand;

		Establish context = () => { CommandLine = new CommandLineParser(); };

		Because of = () =>
			{
				Command = CommandLine.Parse(@"delete --name --size --content -keep=c:\1 c:\2 c:\3 --whatif");
				DeleteCommand = Command as DeleteDuplicatesCommand;
			};

		It should_be_able_to_parse_the_command_line =
			() => Command.ShouldNotBeNull();

		It should_create_the_find_command =
			() => Command.ShouldBeOfType<DeleteDuplicatesCommand>();

		It should_parse_the_directories_to_scan =
			() => DeleteCommand.Scan.ShouldContainOnly(@"c:\2", @"c:\3");

		It should_parse_the_directory_of_files_to_keep =
			() => DeleteCommand.KeepFilesIn.ShouldEqual(@"c:\1");

		It should_parse_the_dry_run_flag =
			() => DeleteCommand.WhatIf.ShouldBeTrue();

		It should_parse_the_name_hash_code_provider =
			() => DeleteCommand.HashCodeProviders.ShouldContainInstanceOf<FileNameHashCodeProvider>();

		It should_parse_the_size_hash_code_provider =
			() => DeleteCommand.HashCodeProviders.ShouldContainInstanceOf<FileSizeHashCodeProvider>();

		It should_parse_the_content_hash_code_provider =
			() => DeleteCommand.HashCodeProviders.ShouldContainInstanceOf<FileContentHashCodeProvider>();
	}

	internal static partial class EnumerableExtensions
	{
		public static void ShouldContainInstanceOf<TElement>(this IEnumerable instance)
		{
			instance
				.OfType<TElement>()
				.Count()
				.ShouldEqual(1);
		}
	}*/
}