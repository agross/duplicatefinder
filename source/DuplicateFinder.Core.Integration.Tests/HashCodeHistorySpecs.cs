using System;
using System.Collections.Generic;
using System.IO;

using Machine.Specifications;
using Machine.Specifications.Utility;

namespace DuplicateFinder.Core.Integration.Tests
{
	public abstract class HistorySpecs
	{
		protected HistorySpecs()
		{
			History = Guid.NewGuid().ToString();
		}

		protected static string History
		{
			get;
			private set;
		}

		protected static IEnumerable<string> HistoryFiles
		{
			get { return Directory.EnumerateFiles(".", History + "*"); }
		}
	}

	public class When_deleted_files_are_resurrected : HistorySpecs
	{
		static ICommand Run3;

		Establish context = () =>
		{
			var parser = new CommandLineParser();

			var run1 = parser.Parse((@"--content --history " + History + @" HashCodeHistory\Run1_FileExists").Args());
			run1.Execute();

			var run2 = parser.Parse((@"--content --history " + History + @" HashCodeHistory\Run2_FileWasDeleted").Args());
			run2.Execute();

			Run3 = parser.Parse((@"--content --history " + History + @" HashCodeHistory\Run3_FileIsRecreated").Args());
		};

		Because of = () => Run3.Execute();

		Cleanup after = () => HistoryFiles.Each(File.Delete);

		It should_delete_the_file_that_was_recreated =
			() => File.Exists(@"HashCodeHistory\Run3_FileIsRecreated\match.txt").ShouldBeFalse();
	}

	public class When_deleted_files_are_resurrected_multiple_times : HistorySpecs
	{
		static ICommand Run3;

		Establish context = () =>
		{
			var parser = new CommandLineParser();

			var run1 = parser.Parse((@"--content --history " + History + @" HashCodeHistory_with_Duplicate\Run1_FileExists").Args());
			run1.Execute();

			var run2 = parser.Parse((@"--content --history " + History + @" HashCodeHistory_with_Duplicate\Run2_FileWasDeleted").Args());
			run2.Execute();

			Run3 = parser.Parse((@"--content --history " + History + @" HashCodeHistory_with_Duplicate\Run3_FileIsRecreatedAsDuplicate").Args());
		};

		Because of = () => Run3.Execute();

		Cleanup after = () => Directory.EnumerateFiles(".", History + "*").Each(File.Delete);

		It should_delete_the_recreated_first_duplicate =
			() => File.Exists(@"HashCodeHistory\Run3_FileIsRecreatedAndDuplicate\match-dup-1.txt").ShouldBeFalse();

		It should_delete_the_recreated_second_duplicate =
			() => File.Exists(@"HashCodeHistory\Run3_FileIsRecreatedAndDuplicate\match-dup-2.txt").ShouldBeFalse();
	}
}