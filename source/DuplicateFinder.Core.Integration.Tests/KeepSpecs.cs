using System.IO;

using Machine.Specifications;

namespace DuplicateFinder.Core.Integration.Tests
{
	public class When_duplicate_files_have_to_be_kept_in_the_second_directory_of_three
	{
		static ICommand Command;

		Establish context =
			() =>
				{
					Command =
						new CommandLineParser()
						.Parse(@"--size --content --name --keep Keep\Second Keep\First Keep\Second Keep\Third".Args());
				};

		Because of = () => Command.Execute();

		It should_keep_the_file_in_the_second =
			() => File.Exists(@"Keep\Second\match.txt").ShouldBeTrue();

		It should_delete_the_file_in_the_first_directory =
			() => File.Exists(@"Keep\First\match.txt").ShouldBeFalse();

		It should_delete_the_file_in_the_third_directory =
			() => File.Exists(@"Keep\Third\match.txt").ShouldBeFalse();
	}
	
	public class When_duplicate_files_are_found_in_one_directory
	{
		static ICommand Command;

		Establish context =
			() =>
				{
					Command =
						new CommandLineParser()
						.Parse(@"--size --keep Keep\DuplicatesInOne Keep\DuplicatesInOne".Args());
				};

		Because of = () => Command.Execute();

		It should_keep_the_first_match =
			() => File.Exists(@"Keep\DuplicatesInOne\match-1.txt").ShouldBeTrue();

		It should_delete_the_second_match =
			() => File.Exists(@"Keep\DuplicatesInOne\match-2.txt").ShouldBeFalse();
	}
}