using System.IO;
using System.Linq;
using DuplicateFinder.Core.CommandLine;
using Machine.Specifications;
using Machine.Specifications.Utility;

namespace DuplicateFinder.Core.Integration.Tests
{
	[Ignore("not implemented")]
	public class When_all_seen_files_are_pruned : HistorySpecs
	{
		static ICommand Run3;

		Establish context = () =>
		{
			var parser = new CommandLineParser();

			var run1 = parser.Parse((@"--delete --content --history " + History + @" HashCodeHistory_Prune\Run1_FileExists").Args());
			run1.Execute();

			var run2 = parser.Parse((@"--delete --content --history " + History + @" HashCodeHistory_Prune\Run2_FileWasDeleted").Args());
			run2.Execute();

			Run3 = parser.Parse((@"--prune --content --history " + History + @" HashCodeHistory_Prune").Args());
		};

		Because of = () => Run3.Execute();

		Cleanup after = () => HistoryFiles.Each(File.Delete);

		It should_remove_all_hash_codes_from_the_history =
			() => HistoryFiles.Each(f => File.ReadLines(f).Each(l => l.ShouldBeEmpty()));
	}
	
	[Ignore("not implemented")]
	public class When_a_subset_of_seen_files_are_pruned : HistorySpecs
	{
		static ICommand Run3;

		Establish context = () =>
		{
			var parser = new CommandLineParser();

			var run1 = parser.Parse((@"--delete --content --history " + History + @" HashCodeHistory_Prune\Run1_FileExists").Args());
			run1.Execute();

			var run2 = parser.Parse((@"--delete --content --history " + History + @" HashCodeHistory_Prune\Run2_FileWasDeleted").Args());
			run2.Execute();

			Run3 = parser.Parse((@"--prune --content --history " + History + @" HashCodeHistory_Prune\Run2_FileWasDeleted").Args());
		};

		Because of = () => Run3.Execute();

		Cleanup after = () => HistoryFiles.Each(File.Delete);

		It should_keep_unpruned_hash_codes_in_the_history =
			() => HistoryFiles.Each(f => File.ReadLines(f).Count().ShouldEqual(1));
	}
}