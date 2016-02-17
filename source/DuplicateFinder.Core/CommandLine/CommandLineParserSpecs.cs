using System.Collections;
using System.Linq;

using DuplicateFinder.Core.CommandLine.TestHelpers;
using DuplicateFinder.Core.Commands;
using DuplicateFinder.Core.Deletion;
using DuplicateFinder.Core.HashCodeHistory;
using DuplicateFinder.Core.HashCodeProviders;
using DuplicateFinder.Core.Streams;

using Machine.Specifications;
using Machine.Specifications.Utility;

namespace DuplicateFinder.Core.CommandLine
{
  [Subject(typeof(CommandLineParser))]
  public class When_options_are_missing
  {
    static ICommand Command;
    static CommandLineParser CommandLine;

    Establish context = () => CommandLine = new CommandLineParser();

    Because of = () => Command = CommandLine.Parse(new string[] { });

    It should_be_able_to_parse_the_command_line =
      () => Command.ShouldNotBeNull();

    It should_create_the_show_help_command =
      () => Command.ShouldBeOfExactType<ShowHelpCommand>();
  }

  [Subject(typeof(CommandLineParser))]
  public class When_help_is_requested
  {
    static ICommand Command;
    static CommandLineParser CommandLine;

    Establish context = () => CommandLine = new CommandLineParser();

    Because of = () => Command = CommandLine.Parse(@"--help".Args());

    It should_be_able_to_parse_the_command_line =
      () => Command.ShouldNotBeNull();

    It should_create_the_show_help_command =
      () => Command.ShouldBeOfExactType<ShowHelpCommand>();
  }

  [Subject(typeof(CommandLineParser))]
  public class When_deleting_duplicates_in_a_dry_run
  {
    static ICommand Command;
    static CommandLineParser CommandLine;

    Establish context = () => CommandLine = new CommandLineParser();

    Because of = () => Command = CommandLine.Parse(@"--name --whatif c:\1 c:\2".Args());

    It should_be_able_to_parse_the_command_line =
      () => Command.ShouldNotBeNull();

    It should_create_the_find_command =
      () => Command.ShouldBeOfExactType<FindDuplicatesCommand>();

    It should_parse_the_directories_to_scan =
      () => Command.As<FindDuplicatesCommand>()
                   .DuplicateFinder.As<DuplicateFinder>()
                   .FileFinders.Each(x => x.ShouldBeOfExactType<RecursiveFileFinder>());

    It should_parse_the_first_directory_to_scan =
      () => Command.As<FindDuplicatesCommand>()
                   .DuplicateFinder.As<DuplicateFinder>()
                   .FileFinders.OfType<RecursiveFileFinder>()
                   .First()
                   .BaseDirectory
                   .ShouldEqual(@"c:\1");

    It should_parse_the_second_directory_to_scan =
      () => Command.As<FindDuplicatesCommand>()
                   .DuplicateFinder.As<DuplicateFinder>()
                   .FileFinders.OfType<RecursiveFileFinder>()
                   .Skip(1)
                   .First()
                   .BaseDirectory.ShouldEqual(@"c:\2");

    It should_not_delete_files =
      () => Command.As<FindDuplicatesCommand>()
                   .FileDeleter.ShouldBeOfExactType<WhatIfFileDeleter>();
  }

  [Subject(typeof(CommandLineParser))]
  public class When_deleting_duplicates_by_name_size_and_contents
  {
    static ICommand Command;
    static CommandLineParser CommandLine;

    Establish context = () => CommandLine = new CommandLineParser();

    Because of = () => Command = CommandLine.Parse(@"--name --size --content c:\1 c:\2".Args());

    It should_be_able_to_parse_the_command_line =
      () => Command.ShouldNotBeNull();

    It should_create_the_find_command =
      () => Command.ShouldBeOfExactType<FindDuplicatesCommand>();

    It should_parse_the_directories_to_scan =
      () => Command.As<FindDuplicatesCommand>()
                   .DuplicateFinder.As<DuplicateFinder>()
                   .FileFinders.Each(x => x.ShouldBeOfExactType<RecursiveFileFinder>());

    It should_parse_the_name_hash_code_provider =
      () => Command.As<FindDuplicatesCommand>()
                   .DuplicateFinder.As<DuplicateFinder>()
                   .HashCodeProviders.ShouldContainInstanceOf<FileNameHashCodeProvider>();

    It should_parse_the_size_hash_code_provider =
      () => Command.As<FindDuplicatesCommand>()
                   .DuplicateFinder.As<DuplicateFinder>()
                   .HashCodeProviders.ShouldContainInstanceOf<FileSizeHashCodeProvider>();

    It should_parse_the_content_hash_code_provider =
      () => Command.As<FindDuplicatesCommand>()
                   .DuplicateFinder.As<DuplicateFinder>()
                   .HashCodeProviders.ShouldContainInstanceOf<FileContentHashCodeProvider>();
  }

  [Subject(typeof(CommandLineParser))]
  public class When_deleting_duplicates_by_name
  {
    static ICommand Command;
    static CommandLineParser CommandLine;

    Establish context = () => CommandLine = new CommandLineParser();

    Because of = () => Command = CommandLine.Parse(@"--name c:\1 c:\2".Args());

    It should_be_able_to_parse_the_command_line =
      () => Command.ShouldNotBeNull();

    It should_create_the_find_command =
      () => Command.ShouldBeOfExactType<FindDuplicatesCommand>();

    It should_parse_the_name_hash_code_provider =
      () => Command.As<FindDuplicatesCommand>()
                   .DuplicateFinder.As<DuplicateFinder>()
                   .HashCodeProviders.ShouldContainInstanceOf<FileNameHashCodeProvider>();

    It should_parse_no_other_hash_code_provider =
      () => Command.As<FindDuplicatesCommand>()
                   .DuplicateFinder.As<DuplicateFinder>()
                   .HashCodeProviders.Count().ShouldEqual(1);
  }

  [Subject(typeof(CommandLineParser))]
  public class When_deleting_duplicates_by_size
  {
    static ICommand Command;
    static CommandLineParser CommandLine;

    Establish context = () => CommandLine = new CommandLineParser();

    Because of = () => Command = CommandLine.Parse(@"--size c:\1 c:\2".Args());

    It should_be_able_to_parse_the_command_line =
      () => Command.ShouldNotBeNull();

    It should_create_the_find_command =
      () => Command.ShouldBeOfExactType<FindDuplicatesCommand>();

    It should_parse_the_size_hash_code_provider =
      () => Command.As<FindDuplicatesCommand>()
                   .DuplicateFinder.As<DuplicateFinder>()
                   .HashCodeProviders.ShouldContainInstanceOf<FileSizeHashCodeProvider>();

    It should_parse_no_other_hash_code_provider =
      () => Command.As<FindDuplicatesCommand>()
                   .DuplicateFinder.As<DuplicateFinder>()
                   .HashCodeProviders.Count().ShouldEqual(1);
  }

  [Subject(typeof(CommandLineParser))]
  public class When_deleting_duplicates_by_content
  {
    static ICommand Command;
    static CommandLineParser CommandLine;

    Establish context = () => CommandLine = new CommandLineParser();

    Because of = () => Command = CommandLine.Parse(@"--content c:\1 c:\2".Args());

    It should_be_able_to_parse_the_command_line =
      () => Command.ShouldNotBeNull();

    It should_create_the_find_command =
      () => Command.ShouldBeOfExactType<FindDuplicatesCommand>();

    It should_parse_the_content_hash_code_provider =
      () => Command.As<FindDuplicatesCommand>()
                   .DuplicateFinder.As<DuplicateFinder>()
                   .HashCodeProviders.ShouldContainInstanceOf<FileContentHashCodeProvider>();

    It should_use_the_whole_file_for_hashing =
      () => Command.As<FindDuplicatesCommand>()
                   .DuplicateFinder.As<DuplicateFinder>()
                   .HashCodeProviders
                   .OfType<FileContentHashCodeProvider>()
                   .First()
                   .StreamDecorators.ShouldBeEmpty();

    It should_parse_no_other_hash_code_provider =
      () => Command.As<FindDuplicatesCommand>()
                   .DuplicateFinder.As<DuplicateFinder>()
                   .HashCodeProviders.Count().ShouldEqual(1);
  }

  [Subject(typeof(CommandLineParser))]
  public class When_deleting_duplicates_by_head_content
  {
    static ICommand Command;
    static CommandLineParser CommandLine;

    Establish context = () => CommandLine = new CommandLineParser();

    Because of = () => Command = CommandLine.Parse(@"--content --first 10 c:\1 c:\2".Args());

    It should_be_able_to_parse_the_command_line =
      () => Command.ShouldNotBeNull();

    It should_create_the_find_command =
      () => Command.ShouldBeOfExactType<FindDuplicatesCommand>();

    It should_parse_the_content_hash_code_provider =
      () => Command.As<FindDuplicatesCommand>()
                   .DuplicateFinder.As<DuplicateFinder>()
                   .HashCodeProviders.ShouldContainInstanceOf<FileContentHashCodeProvider>();

    It should_parse_no_other_hash_code_provider =
      () => Command.As<FindDuplicatesCommand>()
                   .DuplicateFinder.As<DuplicateFinder>()
                   .HashCodeProviders.Count().ShouldEqual(1);

    It should_use_the_file_s_head_for_hashing =
      () => Command.As<FindDuplicatesCommand>()
                   .DuplicateFinder.As<DuplicateFinder>()
                   .HashCodeProviders
                   .OfType<FileContentHashCodeProvider>()
                   .First()
                   .StreamDecorators.ShouldContainInstanceOf<HeadStreamDecorator>();
  }

  [Subject(typeof(CommandLineParser))]
  public class When_deleting_duplicates_by_tail_content
  {
    static ICommand Command;
    static CommandLineParser CommandLine;

    Establish context = () => CommandLine = new CommandLineParser();

    Because of = () => Command = CommandLine.Parse(@"--content --last 10 c:\1 c:\2".Args());

    It should_be_able_to_parse_the_command_line =
      () => Command.ShouldNotBeNull();

    It should_create_the_find_command =
      () => Command.ShouldBeOfExactType<FindDuplicatesCommand>();

    It should_parse_the_content_hash_code_provider =
      () => Command.As<FindDuplicatesCommand>()
                   .DuplicateFinder.As<DuplicateFinder>()
                   .HashCodeProviders.ShouldContainInstanceOf<FileContentHashCodeProvider>();

    It should_parse_no_other_hash_code_provider =
      () => Command.As<FindDuplicatesCommand>()
                   .DuplicateFinder.As<DuplicateFinder>()
                   .HashCodeProviders.Count().ShouldEqual(1);

    It should_use_the_file_s_tail_for_hashing =
      () => Command.As<FindDuplicatesCommand>()
                   .DuplicateFinder.As<DuplicateFinder>()
                   .HashCodeProviders
                   .OfType<FileContentHashCodeProvider>()
                   .First()
                   .StreamDecorators.ShouldContainInstanceOf<TailStreamDecorator>();
  }

  [Subject(typeof(CommandLineParser))]
  public class When_deleting_duplicates_by_head_and_tail_content
  {
    static ICommand Command;
    static CommandLineParser CommandLine;

    Establish context = () => CommandLine = new CommandLineParser();

    Because of = () => Command = CommandLine.Parse(@"--content --first 10 --last 10 c:\1 c:\2".Args());

    It should_be_able_to_parse_the_command_line =
      () => Command.ShouldNotBeNull();

    It should_create_the_find_command =
      () => Command.ShouldBeOfExactType<FindDuplicatesCommand>();

    It should_parse_the_content_hash_code_provider =
      () => Command.As<FindDuplicatesCommand>()
                   .DuplicateFinder.As<DuplicateFinder>()
                   .HashCodeProviders
                   .ShouldContainInstanceOf<FileContentHashCodeProvider>();

    It should_parse_no_other_hash_code_provider =
      () => Command.As<FindDuplicatesCommand>()
                   .DuplicateFinder.As<DuplicateFinder>()
                   .HashCodeProviders.Count().ShouldEqual(1);

    It should_use_the_file_s_head_for_hashing =
      () => Command.As<FindDuplicatesCommand>()
                   .DuplicateFinder.As<DuplicateFinder>()
                   .HashCodeProviders
                   .OfType<FileContentHashCodeProvider>()
                   .First()
                   .StreamDecorators.ShouldContainInstanceOf<HeadStreamDecorator>();

    It should_use_the_file_s_tail_for_hashing =
      () => Command.As<FindDuplicatesCommand>()
                   .DuplicateFinder.As<DuplicateFinder>()
                   .HashCodeProviders
                   .OfType<FileContentHashCodeProvider>()
                   .First()
                   .StreamDecorators.ShouldContainInstanceOf<TailStreamDecorator>();

    It should_parse_no_other_stream_decorators =
      () => Command.As<FindDuplicatesCommand>()
                   .DuplicateFinder.As<DuplicateFinder>()
                   .HashCodeProviders
                   .OfType<FileContentHashCodeProvider>()
                   .First()
                   .StreamDecorators.Length.ShouldEqual(2);
  }

  [Subject(typeof(CommandLineParser))]
  public class When_not_keeping_a_list_of_seen_hashes
  {
    static ICommand Command;
    static CommandLineParser CommandLine;

    Establish context = () => CommandLine = new CommandLineParser();

    Because of = () => Command = CommandLine.Parse(@"--name c:\1".Args());

    It should_be_able_to_parse_the_command_line =
      () => Command.ShouldNotBeNull();

    It should_create_the_find_command =
      () => Command.ShouldBeOfExactType<FindDuplicatesCommand>();

    It should_not_maintain_history_of_seen_hashes =
      () => Command.As<FindDuplicatesCommand>()
                   .DuplicateFinder.As<DuplicateFinder>()
                   .History.ShouldBeOfExactType<NullHistory>();
  }

  [Subject(typeof(CommandLineParser))]
  public class When_keeping_a_list_of_seen_hashes
  {
    static ICommand Command;
    static CommandLineParser CommandLine;

    Establish context = () => CommandLine = new CommandLineParser();

    Because of = () => Command = CommandLine.Parse(@"--name --history file c:\1".Args());

    It should_be_able_to_parse_the_command_line =
      () => Command.ShouldNotBeNull();

    It should_create_the_find_command =
      () => Command.ShouldBeOfExactType<FindDuplicatesCommand>();

    It should_maintain_a_history_of_seen_hashes =
      () => Command.As<FindDuplicatesCommand>()
                   .DuplicateFinder.As<DuplicateFinder>()
                   .History.ShouldBeOfExactType<DatabaseHistory>();

    It should_maintain_a_history_of_seen_hashes_in_the_file_specified_on_the_command_line =
      () => Command.As<FindDuplicatesCommand>()
                   .DuplicateFinder.As<DuplicateFinder>()
                   .History.As<DatabaseHistory>()
                   .DatabaseFile.ShouldEqual("file");
  }

  [Subject(typeof(CommandLineParser))]
  public class When_pruning_the_list_of_seen_hashes
  {
    static ICommand Command;
    static CommandLineParser CommandLine;

    Establish context = () => CommandLine = new CommandLineParser();

    Because of = () => Command = CommandLine.Parse(@"--prune --name --history file c:\1".Args());

    It should_be_able_to_parse_the_command_line =
      () => Command.ShouldNotBeNull();

    It should_create_the_prune_history_command =
      () => Command.ShouldBeOfExactType<PruneHistoryCommand>();

    It should_not_maintain_a_history_of_seen_hashes =
      () => Command.As<PruneHistoryCommand>()
                   .DuplicateFinder.As<DuplicateFinder>()
                   .History.ShouldBeOfExactType<NullHistory>();

    It should_delete_hashes_from_the_history_file =
      () => Command.As<PruneHistoryCommand>()
                   .History.As<DatabaseHistory>()
                   .DatabaseFile.ShouldEqual("file");
  }

  [Subject(typeof(CommandLineParser))]
  public class When_pruning_the_list_of_seen_hashes_while_omitting_the_history
  {
    static ICommand Command;
    static CommandLineParser CommandLine;

    Establish context = () => CommandLine = new CommandLineParser();

    Because of = () => Command = CommandLine.Parse(@"--prune --name c:\1".Args());

    It should_be_able_to_parse_the_command_line =
      () => Command.ShouldNotBeNull();

    It should_create_the_show_help_command =
      () => Command.ShouldBeOfExactType<ShowHelpCommand>();
  }

  static class EnumerableExtensions
  {
    public static void ShouldContainInstanceOf<TElement>(this IEnumerable instance)
    {
      instance
        .OfType<TElement>()
        .Count()
        .ShouldEqual(1);
    }
  }
}
