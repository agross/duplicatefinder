using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using DuplicateFinder.Core.CommandLine;
using Machine.Specifications;
using Machine.Specifications.Utility;

namespace DuplicateFinder.Core.Integration.Tests
{
	[Tags("integration")]
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
		
		protected static string Gone
		{
			get { return HistoryFiles.First(x => x.EndsWith("-gone")); }
		}
		
		protected static string Current
		{
			get { return HistoryFiles.First(x => x.EndsWith("-current")); }
		}
		
		protected static string Seen
		{
			get { return HistoryFiles.First(x => x.EndsWith("-seen")); }
		}
	}

	[Tags("integration")]
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
    
    [Tags("integration")]
	public class When_a_historized_scan_is_run_with_different_parameter_order : HistorySpecs
	{
		static ICommand Run2;

		Establish context = () =>
		{
			var parser = new CommandLineParser();

			var run1 = parser.Parse((@"--content --size --history " + History + @" HashCodeParamOrder").Args());
			run1.Execute();

            Run2 = parser.Parse((@"--size --content --history " + History + @" HashCodeParamOrder").Args());
		};

		Because of = () => Run2.Execute();

		Cleanup after = () => HistoryFiles.Each(File.Delete);

		It should_have_only_one_hash_in_seen_history =
            () => File.ReadLines(Seen).Count().ShouldEqual(1);
	}

	[Tags("integration")]
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

	[Tags("integration")]
	public class When_files_outside_the_scanned_directories_are_moved_to_the_archive : HistorySpecs
	{
	  static ICommand Run2;

	  Establish context = () =>
	  {
		var parser = new CommandLineParser();

		var run1 = parser.Parse((@"--content --history " + History + @" --keep HashCodeHistory_FileWasMoved\Run1_FileExists\Archive HashCodeHistory_FileWasMoved\Run1_FileExists\Archive HashCodeHistory_FileWasMoved\Run1_FileExists\Scanned").Args());
		run1.Execute();

		Run2 = parser.Parse((@"--content --history " + History + @"  --keep HashCodeHistory_FileWasMoved\Run2_FileWasMoved\Archive HashCodeHistory_FileWasMoved\Run2_FileWasMoved\Archive HashCodeHistory_FileWasMoved\Run2_FileWasMoved\Scanned").Args());
	  };

	  Because of = () => Run2.Execute();

	  Cleanup after = () => Directory.EnumerateFiles(".", History + "*").Each(File.Delete);

	  It should_not_delete_the_moved_file =
		  () => File.Exists(@"HashCodeHistory_FileWasMoved\Run2_FileWasMoved\Archive\match.txt").ShouldBeTrue();
	}
}