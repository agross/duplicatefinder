using System;
using System.Collections;
using System.Linq;

using DuplicateFinder.Core.Commands;
using DuplicateFinder.Core.Deletion;
using DuplicateFinder.Core.HashCodeProviders;
using DuplicateFinder.Core.Streams;

using Machine.Specifications;
using Machine.Specifications.Utility;

namespace DuplicateFinder.Core
{
	[Subject(typeof(CommandLineParser))]
	public class When_help_is_requested
	{
		static ICommand Command;
		static CommandLineParser CommandLine;

		Establish context = () => { CommandLine = new CommandLineParser(); };

		Because of = () => { Command = CommandLine.Parse(@"--help".Args()); };

		It should_be_able_to_parse_the_command_line =
			() => Command.ShouldNotBeNull();

		It should_create_the_show_help_command =
			() => Command.ShouldBeOfType<ShowHelpCommand>();
	}

	[Subject(typeof(CommandLineParser))]
	public class When_finding_duplicates_in_a_dry_run
	{
		static ICommand Command;
		static CommandLineParser CommandLine;
		static FindDuplicatesCommand FindCommand;
		static DuplicateFinder DuplicateFinder;

		Establish context = () => { CommandLine = new CommandLineParser(); };

		Because of = () =>
			{
				Command = CommandLine.Parse(@"--name --whatif c:\1 c:\2".Args());
				FindCommand = Command as FindDuplicatesCommand;
				DuplicateFinder = (DuplicateFinder)FindCommand.DuplicateFinder;
			};

		It should_be_able_to_parse_the_command_line =
			() => Command.ShouldNotBeNull();

		It should_create_the_find_command =
			() => Command.ShouldBeOfType<FindDuplicatesCommand>();

		It should_parse_the_directories_to_scan =
			() => DuplicateFinder.FileFinders.Each(x => x.ShouldBeOfType<RecursiveFileFinder>());

		It should_parse_the_first_directory_to_scan =
			() => DuplicateFinder.FileFinders.OfType<RecursiveFileFinder>()
			      	.First()
			      	.BaseDirectory
			      	.ShouldEqual(@"c:\1");

		It should_parse_the_second_directory_to_scan =
			() => DuplicateFinder.FileFinders.OfType<RecursiveFileFinder>()
			      	.Skip(1)
			      	.First()
			      	.BaseDirectory.ShouldEqual(@"c:\2");

		It should_not_delete_files =
			() => FindCommand.FileDeleter.ShouldBeOfType<WhatIfFileDeleter>();
	}

	[Subject(typeof(CommandLineParser))]
	public class When_options_are_missing
	{
		static ICommand Command;
		static CommandLineParser CommandLine;
		static ShowHelpCommand ShowHelpCommand;

		Establish context = () => { CommandLine = new CommandLineParser(); };

		Because of = () =>
			{
				Command = CommandLine.Parse(new string[] { });
				ShowHelpCommand = Command as ShowHelpCommand;
			};

		It should_be_able_to_parse_the_command_line =
			() => Command.ShouldNotBeNull();

		It should_create_the_show_help_command =
			() => Command.ShouldBeOfType<ShowHelpCommand>();

		It should_create_a_message_for_the_missing_comparison_option =
			() => ShowHelpCommand.Messages.ShouldContain("The comparison type is missing");

		It should_create_a_message_for_the_missing_directories =
			() => ShowHelpCommand.Messages.ShouldContain("No directories to compare");
	}

	[Subject(typeof(CommandLineParser))]
	public class When_finding_duplicates_by_name_size_and_contents
	{
		static ICommand Command;
		static CommandLineParser CommandLine;
		static DuplicateFinder DuplicateFinder;

		Establish context = () => { CommandLine = new CommandLineParser(); };

		Because of = () =>
			{
				Command = CommandLine.Parse(@"--name --size --content c:\1 c:\2".Args());
				DuplicateFinder = (DuplicateFinder)(Command as FindDuplicatesCommand).DuplicateFinder;
			};

		It should_be_able_to_parse_the_command_line =
			() => Command.ShouldNotBeNull();

		It should_create_the_find_command =
			() => Command.ShouldBeOfType<FindDuplicatesCommand>();

		It should_parse_the_directories_to_scan =
			() => DuplicateFinder.FileFinders.Each(x => x.ShouldBeOfType<RecursiveFileFinder>());

		It should_parse_the_name_hash_code_provider =
			() => DuplicateFinder.HashCodeProviders.ShouldContainInstanceOf<FileNameHashCodeProvider>();

		It should_parse_the_size_hash_code_provider =
			() => DuplicateFinder.HashCodeProviders.ShouldContainInstanceOf<FileSizeHashCodeProvider>();

		It should_parse_the_content_hash_code_provider =
			() => DuplicateFinder.HashCodeProviders.ShouldContainInstanceOf<FileContentHashCodeProvider>();
	}

	[Subject(typeof(CommandLineParser))]
	public class When_finding_duplicates_by_name
	{
		static ICommand Command;
		static CommandLineParser CommandLine;
		static DuplicateFinder DuplicateFinder;

		Establish context = () => { CommandLine = new CommandLineParser(); };

		Because of = () =>
			{
				Command = CommandLine.Parse(@"--name c:\1 c:\2".Args());
				DuplicateFinder = (DuplicateFinder)((FindDuplicatesCommand) Command).DuplicateFinder;
			};

		It should_be_able_to_parse_the_command_line =
			() => Command.ShouldNotBeNull();

		It should_create_the_find_command =
			() => Command.ShouldBeOfType<FindDuplicatesCommand>();

		It should_parse_the_name_hash_code_provider =
			() => DuplicateFinder.HashCodeProviders.ShouldContainInstanceOf<FileNameHashCodeProvider>();

		It should_parse_no_other_hash_code_provider =
			() => DuplicateFinder.HashCodeProviders.Count().ShouldEqual(1);
	}

	[Subject(typeof(CommandLineParser))]
	public class When_finding_duplicates_by_size
	{
		static ICommand Command;
		static CommandLineParser CommandLine;
		static DuplicateFinder DuplicateFinder;

		Establish context = () => { CommandLine = new CommandLineParser(); };

		Because of = () =>
			{
				Command = CommandLine.Parse(@"--size c:\1 c:\2".Args());
				DuplicateFinder = (DuplicateFinder)((FindDuplicatesCommand) Command).DuplicateFinder;
			};

		It should_be_able_to_parse_the_command_line =
			() => Command.ShouldNotBeNull();

		It should_create_the_find_command =
			() => Command.ShouldBeOfType<FindDuplicatesCommand>();

		It should_parse_the_size_hash_code_provider =
			() => DuplicateFinder.HashCodeProviders.ShouldContainInstanceOf<FileSizeHashCodeProvider>();

		It should_parse_no_other_hash_code_provider =
			() => DuplicateFinder.HashCodeProviders.Count().ShouldEqual(1);
	}

	[Subject(typeof(CommandLineParser))]
	public class When_finding_duplicates_by_content
	{
		static ICommand Command;
		static CommandLineParser CommandLine;
		static DuplicateFinder DuplicateFinder;

		Establish context = () => { CommandLine = new CommandLineParser(); };

		Because of = () =>
			{
				Command = CommandLine.Parse(@"--content c:\1 c:\2".Args());
				DuplicateFinder = (DuplicateFinder)((FindDuplicatesCommand) Command).DuplicateFinder;
			};

		It should_be_able_to_parse_the_command_line =
			() => Command.ShouldNotBeNull();

		It should_create_the_find_command =
			() => Command.ShouldBeOfType<FindDuplicatesCommand>();

		It should_parse_the_content_hash_code_provider =
			() => DuplicateFinder.HashCodeProviders.ShouldContainInstanceOf<FileContentHashCodeProvider>();

		It should_use_the_whole_file_for_hashing =
			() => DuplicateFinder.HashCodeProviders
			      	.OfType<FileContentHashCodeProvider>()
			      	.First()
			      	.StreamDecorators.ShouldBeEmpty();

		It should_parse_no_other_hash_code_provider =
			() => DuplicateFinder.HashCodeProviders.Count().ShouldEqual(1);
	}

	[Subject(typeof(CommandLineParser))]
	public class When_finding_duplicates_by_head_content
	{
		static ICommand Command;
		static CommandLineParser CommandLine;
		static DuplicateFinder DuplicateFinder;

		Establish context = () => { CommandLine = new CommandLineParser(); };

		Because of = () =>
			{
				Command = CommandLine.Parse(@"--content --first 10 c:\1 c:\2".Args());
				DuplicateFinder = (DuplicateFinder)((FindDuplicatesCommand) Command).DuplicateFinder;
			};

		It should_be_able_to_parse_the_command_line =
			() => Command.ShouldNotBeNull();

		It should_create_the_find_command =
			() => Command.ShouldBeOfType<FindDuplicatesCommand>();

		It should_parse_the_content_hash_code_provider =
			() => DuplicateFinder.HashCodeProviders.ShouldContainInstanceOf<FileContentHashCodeProvider>();

		It should_parse_no_other_hash_code_provider =
			() => DuplicateFinder.HashCodeProviders.Count().ShouldEqual(1);

		It should_use_the_file_s_head_for_hashing =
			() => DuplicateFinder.HashCodeProviders
			      	.OfType<FileContentHashCodeProvider>()
			      	.First()
			      	.StreamDecorators.ShouldContainInstanceOf<HeadStreamDecorator>();
	}

	[Subject(typeof(CommandLineParser))]
	public class When_finding_duplicates_by_tail_content
	{
		static ICommand Command;
		static CommandLineParser CommandLine;
		static DuplicateFinder DuplicateFinder;

		Establish context = () => { CommandLine = new CommandLineParser(); };

		Because of = () =>
			{
				Command = CommandLine.Parse(@"--content --last 10 c:\1 c:\2".Args());
				DuplicateFinder = (DuplicateFinder)((FindDuplicatesCommand) Command).DuplicateFinder;
			};

		It should_be_able_to_parse_the_command_line =
			() => Command.ShouldNotBeNull();

		It should_create_the_find_command =
			() => Command.ShouldBeOfType<FindDuplicatesCommand>();

		It should_parse_the_content_hash_code_provider =
			() => DuplicateFinder.HashCodeProviders.ShouldContainInstanceOf<FileContentHashCodeProvider>();

		It should_parse_no_other_hash_code_provider =
			() => DuplicateFinder.HashCodeProviders.Count().ShouldEqual(1);

		It should_use_the_file_s_tail_for_hashing =
			() => DuplicateFinder.HashCodeProviders
			      	.OfType<FileContentHashCodeProvider>()
			      	.First()
			      	.StreamDecorators.ShouldContainInstanceOf<TailStreamDecorator>();
	}

	[Subject(typeof(CommandLineParser))]
	public class When_finding_duplicates_by_head_and_tail_content
	{
		static ICommand Command;
		static CommandLineParser CommandLine;
		static DuplicateFinder DuplicateFinder;

		Establish context = () => { CommandLine = new CommandLineParser(); };

		Because of = () =>
			{
				Command = CommandLine.Parse(@"--content --first 10 --last 10 c:\1 c:\2".Args());
				DuplicateFinder = (DuplicateFinder) ((FindDuplicatesCommand) Command).DuplicateFinder;
			};

		It should_be_able_to_parse_the_command_line =
			() => Command.ShouldNotBeNull();

		It should_create_the_find_command =
			() => Command.ShouldBeOfType<FindDuplicatesCommand>();

		It should_parse_the_content_hash_code_provider =
			() => DuplicateFinder.HashCodeProviders
			      	.ShouldContainInstanceOf<FileContentHashCodeProvider>();

		It should_parse_no_other_hash_code_provider =
			() => DuplicateFinder.HashCodeProviders.Count().ShouldEqual(1);

		It should_use_the_file_s_head_for_hashing =
			() => DuplicateFinder.HashCodeProviders
			      	.OfType<FileContentHashCodeProvider>()
			      	.First()
			      	.StreamDecorators.ShouldContainInstanceOf<HeadStreamDecorator>();

		It should_use_the_file_s_tail_for_hashing =
			() => DuplicateFinder.HashCodeProviders
			      	.OfType<FileContentHashCodeProvider>()
			      	.First()
			      	.StreamDecorators.ShouldContainInstanceOf<TailStreamDecorator>();

		It should_parse_no_other_stream_decorators =
			() => DuplicateFinder.HashCodeProviders
			      	.OfType<FileContentHashCodeProvider>()
			      	.First()
			      	.StreamDecorators.Count().ShouldEqual(2);
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

		public static string[] Args(this string args)
		{
			return args.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
		}
	}
}