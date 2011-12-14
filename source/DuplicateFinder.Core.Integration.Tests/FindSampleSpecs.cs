using System;
using System.Collections.Generic;
using System.Linq;
using DuplicateFinder.Core.CommandLine;
using DuplicateFinder.Core.Commands;

using Machine.Specifications;

namespace DuplicateFinder.Core.Integration.Tests
{
	public class When_duplicates_are_searched_by_size
	{
		static ICommand Command;

		Establish context =
			() => { Command = new CommandLineParser().Parse(@"--size --whatif Samples\First Samples\Second".Args()); };

		Because of = () => Command.Execute();

		It should_find_files_of_the_same_size =
			() => Command.AllDuplicates().ShouldContainOnly(@"Samples\First\content-full-first.txt",
															@"Samples\Second\content-full-second.txt",
															@"Samples\First\size-first.txt",
															@"Samples\Second\size-second.txt");
	}

	public class When_duplicates_are_searched_by_name
	{
		static ICommand Command;

		Establish context =
			() => { Command = new CommandLineParser().Parse(@"--name --whatif Samples\First Samples\Second".Args()); };

		Because of = () => Command.Execute();

		It should_find_files_with_the_same_name =
			() => Command.AllDuplicates().ShouldContainOnly(@"Samples\First\name.txt",
															@"Samples\Second\name.txt");
	}

	public class When_duplicates_are_searched_by_contents
	{
		static ICommand Command;

		Establish context =
			() => { Command = new CommandLineParser().Parse(@"--content --whatif Samples\First Samples\Second".Args()); };

		Because of = () => Command.Execute();

		It should_find_files_with_the_same_contents =
			() => Command.AllDuplicates().ShouldContainOnly(@"Samples\First\content-full-first.txt",
															@"Samples\Second\content-full-second.txt");
	}

	public class When_duplicates_are_searched_by_head_contents
	{
		static ICommand Command;

		Establish context =
			() => { Command = new CommandLineParser().Parse(@"--content --first 5 --whatif Samples\First Samples\Second".Args()); };

		Because of = () => Command.Execute();

		It should_find_files_with_the_same_contents_in_the_first_five_bytes= 
			() => Command.AllDuplicates().ShouldContainOnly(@"Samples\First\content-full-first.txt",
															@"Samples\Second\content-full-second.txt",
															@"Samples\First\content-head-first.txt",
															@"Samples\Second\content-head-second.txt");
	}

	public class When_duplicates_are_searched_by_tail_contents
	{
		static ICommand Command;

		Establish context =
			() => { Command = new CommandLineParser().Parse(@"--content --last 5 --whatif Samples\First Samples\Second".Args()); };

		Because of = () => Command.Execute();

		It should_find_files_with_the_same_contents_in_the_first_five_bytes =
			() => Command.AllDuplicates().ShouldContainOnly(@"Samples\First\content-full-first.txt",
															@"Samples\Second\content-full-second.txt",
															@"Samples\First\content-tail-first.txt",
															@"Samples\Second\content-tail-second.txt");
	}

	public class When_duplicates_are_searched_by_head_and_tail_contents
	{
		static ICommand Command;

		Establish context =
			() => { Command = new CommandLineParser().Parse(@"--content --first 5 --last 5 --whatif Samples\First Samples\Second".Args()); };

		Because of = () => Command.Execute();

		It should_find_files_with_the_same_contents_in_the_first_and_last_five_bytes =
			() => Command.AllDuplicates().ShouldContainOnly(@"Samples\First\content-full-first.txt",
															@"Samples\Second\content-full-second.txt");
	}

	internal static class Extensions
	{
		public static string[] Args(this string args)
		{
			return args.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
		}

		static T As<T>(this object instance)
		{
			return (T) instance;
		}

		public static IEnumerable<string> AllDuplicates(this object instance)
		{
			return instance.As<FindDuplicatesCommand>().Results.Duplicates.SelectMany(x => x);
		}
	}
}