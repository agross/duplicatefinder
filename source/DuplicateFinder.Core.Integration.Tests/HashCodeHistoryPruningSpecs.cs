using System.IO;
using System.Linq;
using DuplicateFinder.Core.CommandLine;
using Machine.Specifications;
using Machine.Specifications.Utility;

namespace DuplicateFinder.Core.Integration.Tests
{
	[Tags("integration")]
	public class When_all_seen_files_are_pruned : HistorySpecs
	{
		static ICommand Prune;

		Establish context = () =>
		{
			var parser = new CommandLineParser();

			var run1 = parser.Parse((@"--name --history " + History + @" HashCodeHistory_Prune\Run1_FileExists").Args());
			run1.Execute();

			var run2 = parser.Parse((@"--name --history " + History + @" HashCodeHistory_Prune\Run2_FileWasDeleted").Args());
			run2.Execute();

			Prune = parser.Parse((@"--prune --name --history " + History + @" HashCodeHistory_Prune").Args());
		};

		Because of = () => Prune.Execute();

		Cleanup after = () => HistoryFiles.Each(File.Delete);

		It should_remove_all_hash_codes_from_the_history =
			() => HistoryFiles.Each(f => File.ReadLines(f).ShouldBeEmpty());
	}

	[Tags("integration")]
	public class When_all_seen_files_are_pruned_and_later_are_recreated : HistorySpecs
	{
		static ICommand Prune;
		static CommandLineParser Parser;

		Establish context = () =>
		{
			Parser = new CommandLineParser();

			var run1 = Parser.Parse((@"--name --history " + History + @" HashCodeHistory_Prune\Run1_FileExists").Args());
			run1.Execute();

			var run2 = Parser.Parse((@"--name --history " + History + @" HashCodeHistory_Prune\Run2_FileWasDeleted").Args());
			run2.Execute();

			Prune = Parser.Parse((@"--prune --name --history " + History + @" HashCodeHistory_Prune").Args());
		};

		Because of = () =>
		{
			Prune.Execute();
			
			Parser
				.Parse((@"--name --history " + History + @" HashCodeHistory_Prune\Run4_FileIsRecreated").Args())
				.Execute();
		};

		Cleanup after = () => HistoryFiles.Each(File.Delete);

		It should_have_two_seen_files_that_were_recreated =
			() => File.ReadLines(Seen).ShouldContainOnly("match-1.txt", "match-2.txt");

		It should_have_one_current_file =
			() => File.ReadLines(Current).ShouldContainOnly("match-1.txt", "match-2.txt");

		It should_have_no_gone_files =
			() => File.ReadLines(Gone).ShouldBeEmpty();
	}

	[Tags("integration")]
	public class When_a_subset_of_seen_files_is_pruned : HistorySpecs
	{
		static ICommand Prune;

		Establish context = () =>
		{
			var parser = new CommandLineParser();

			var run1 = parser.Parse((@"--name --history " + History + @" HashCodeHistory_PruneSubset\Run1_FileExists").Args());
			run1.Execute();

			var run2 = parser.Parse((@"--name --history " + History + @" HashCodeHistory_PruneSubset\Run2_FileWasDeleted").Args());
			run2.Execute();

			Prune = parser.Parse((@"--prune --name --history " + History + @" HashCodeHistory_PruneSubset\Run1_FileExists").Args());
		};

		Because of = () => Prune.Execute();

		Cleanup after = () => HistoryFiles.Each(File.Delete);

		It should_have_one_seen_file =
			() => File.ReadLines(Seen).ShouldContainOnly("no-match.txt");

		It should_have_one_current_file =
			() => File.ReadLines(Current).ShouldContainOnly("no-match.txt");

		It should_have_no_gone_files =
			() => File.ReadLines(Gone).ShouldBeEmpty();
	}

	[Tags("integration")]
	public class When_a_subset_of_seen_files_is_pruned_and_later_is_recreated : HistorySpecs
	{
		static ICommand Prune;
		static CommandLineParser Parser;

		Establish context = () =>
		{
			Parser = new CommandLineParser();

			var run1 = Parser.Parse((@"--name --history " + History + @" HashCodeHistory_PruneSubset\Run1_FileExists").Args());
			run1.Execute();

			var run2 = Parser.Parse((@"--name --history " + History + @" HashCodeHistory_PruneSubset\Run2_FileWasDeleted").Args());
			run2.Execute();

			Prune = Parser.Parse((@"--prune --name --history " + History + @" HashCodeHistory_PruneSubset\Run1_FileExists").Args());
		};

		Because of = () =>
		{
			Prune.Execute();
			
			Parser
				.Parse((@"--name --history " + History + @" HashCodeHistory_PruneSubset\Run3_FileIsRecreated").Args())
				.Execute();
		};

		Cleanup after = () => HistoryFiles.Each(File.Delete);

		It should_have_two_seen_files =
			() => File.ReadLines(Seen).ShouldContainOnly("match.txt", "no-match.txt");

		It should_have_one_current_file =
			() => File.ReadLines(Current).ShouldContainOnly("match.txt");

		It should_have_one_gone_file =
			() => File.ReadLines(Gone).ShouldContainOnly("no-match.txt");
	}
}