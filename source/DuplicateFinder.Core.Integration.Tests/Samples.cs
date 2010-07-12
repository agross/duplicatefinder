using Machine.Specifications;

namespace DuplicateFinder.Core.Integration.Tests
{
	public class When_duplicates_are_searched_by_size
	{
		static ICommand Command;

		Establish context = () => { Command = new CommandLineParser().Parse(@"find -size Samples\First Samples\Second"); };

		Because of = () => Command.Execute();

		It should_find_files_of_the_same_size;
	}
	
	public class When_duplicates_are_searched_by_name
	{
		static ICommand Command;

		Establish context = () => { Command = new CommandLineParser().Parse(@"find -name Samples\First Samples\Second"); };

		Because of = () => Command.Execute();

		It should_find_files_with_the_same_name;
	}
	
	public class When_duplicates_are_searched_by_contents
	{
		static ICommand Command;

		Establish context = () => { Command = new CommandLineParser().Parse(@"find -content Samples\First Samples\Second"); };

		Because of = () => Command.Execute();

		It should_find_files_with_the_same_contents;
	}
	
	public class When_duplicates_are_searched_by_head_contents
	{
		static ICommand Command;

		Establish context = () => { Command = new CommandLineParser().Parse(@"find -content -first 5 Samples\First Samples\Second"); };

		Because of = () => Command.Execute();

		It should_find_files_with_the_same_contents_in_the_first_five_bytes;
	}
	
	public class When_duplicates_are_searched_by_tail_contents
	{
		static ICommand Command;

		Establish context = () => { Command = new CommandLineParser().Parse(@"find -content -last 5 Samples\First Samples\Second"); };

		Because of = () => Command.Execute();

		It should_find_files_with_the_same_contents_in_the_first_five_bytes;
	}
	
	public class When_duplicates_are_searched_by_head_and_tail_contents
	{
		static ICommand Command;

		Establish context = () => { Command = new CommandLineParser().Parse(@"find -content -first 5 -last 5 Samples\First Samples\Second"); };

		Because of = () => Command.Execute();

		It should_find_files_with_the_same_contents_in_the_first_and_last_five_bytes;
	}
}