using System;
using System.Collections.Generic;
using System.IO;

using DuplicateFinder.Core.Abstractions;

using Machine.Specifications;
using Machine.Specifications.Utility;

namespace DuplicateFinder.Core.HashCodeHistory
{
	public abstract class DatabaseHistorySpecs
	{
		protected DatabaseHistorySpecs()
		{
			FileName = Guid.NewGuid().ToString();
		}

		protected static string FileName
		{
			get;
			private set;
		}

		protected static IEnumerable<string> HistoryFiles
		{
			get { return Directory.EnumerateFiles(".", FileName + "*"); }
		}
	}

	[Subject(typeof(DatabaseHistory))]
	public class When_the_first_snapshot_is_created : DatabaseHistorySpecs
	{
		static DatabaseHistory History;
		static IEnumerable<string> Resurrected;

		Establish context = () =>
		{
			History = new DatabaseHistory(FileName, new FileSystem());
		};

		Because of = () => { Resurrected = History.Snapshot(new[] { "1", "2", "3" }); };
		
		Cleanup after = () => HistoryFiles.Each(File.Delete);

		It should_have_no_resurrected_files =
			() => Resurrected.ShouldBeEmpty();
	}

	[Subject(typeof(DatabaseHistory))]
	public class When_no_files_have_been_resurrected_and_history_exists : DatabaseHistorySpecs
	{
		static DatabaseHistory History;
		static IEnumerable<string> Resurrected;

		Establish context = () =>
		{
			History = new DatabaseHistory(FileName, new FileSystem());
			History.Snapshot(new[] { "1", "2", "3" });
		};

		Because of = () => { Resurrected = History.Snapshot(new[] { "1", "2" }); };

		Cleanup after = () => HistoryFiles.Each(File.Delete);

		It should_have_no_resurrected_files =
			() => Resurrected.ShouldBeEmpty();
	}

	[Subject(typeof(DatabaseHistory))]
	public class When_a_file_has_been_resurrected : DatabaseHistorySpecs
	{
		static DatabaseHistory History;
		static IEnumerable<string> Resurrected;

		Establish context = () =>
		{
			History = new DatabaseHistory(FileName, new FileSystem());
			History.Snapshot(new[] { "1", "2" });
			History.Snapshot(new[] { "1" });
		};

		Because of = () => { Resurrected = History.Snapshot(new[] { "1", "2" }); };

		Cleanup after = () => HistoryFiles.Each(File.Delete);

		It should_identify_the_file_that_was_resurrected =
			() => Resurrected.ShouldContainOnly("2");
	}

	[Subject(typeof(DatabaseHistory))]
	public class When_files_have_been_resurrected : DatabaseHistorySpecs
	{
		static DatabaseHistory History;
		static IEnumerable<string> Resurrected;

		Establish context = () =>
		{
			History = new DatabaseHistory(FileName, new FileSystem());
			History.Snapshot(new[] { "1", "2" });
			History.Snapshot(new string[] { });
		};

		Because of = () => { Resurrected = History.Snapshot(new[] { "1", "2" }); };

		Cleanup after = () => HistoryFiles.Each(File.Delete);

		It should_identify_the_files_that_were_resurrected =
			() => Resurrected.ShouldContainOnly("1", "2");
	}
}