using System.Collections.Generic;

using Machine.Specifications;

namespace DuplicateFinder.Core.HashCodeHistory
{
	[Subject(typeof(DatabaseHistory))]
	public class When_no_history_is_available
	{
		static DatabaseHistory History;
		static IEnumerable<string> Resurrected;

		Establish context = () =>
			{
				History = new DatabaseHistory(null);
				History.Snapshot(new[] { "1", "2", "3" });
			};

		Because of = () => { Resurrected = History.Resurrected(); };

		It should_have_no_resurrected_files =
			() => Resurrected.ShouldBeEmpty();
	}

	[Subject(typeof(DatabaseHistory))]
	public class When_no_files_have_been_resurrected_but_history_exists
	{
		static DatabaseHistory History;
		static IEnumerable<string> Resurrected;

		Establish context = () =>
			{
				History = new DatabaseHistory(null);
				History.Snapshot(new[] { "1", "2", "3" });
				History.Snapshot(new[] { "1", "2" });
			};

		Because of = () => { Resurrected = History.Resurrected(); };

		It should_have_no_resurrected_files =
			() => Resurrected.ShouldBeEmpty();
	}

	[Subject(typeof(DatabaseHistory))]
	public class When_a_file_has_been_resurrected
	{
		static DatabaseHistory History;
		static IEnumerable<string> Resurrected;

		Establish context = () =>
			{
				History = new DatabaseHistory(null);
				History.Snapshot(new[] { "1", "2" });
				History.Snapshot(new[] { "1", "3" });
				History.Snapshot(new[] { "1", "2" });
			};

		Because of = () => { Resurrected = History.Resurrected(); };

		It should_identify_the_file_that_was_resurrected =
			() => Resurrected.ShouldContainOnly("2");
	}
	
	[Subject(typeof(DatabaseHistory))]
	public class When_files_have_been_resurrected
	{
		static DatabaseHistory History;
		static IEnumerable<string> Resurrected;

		Establish context = () =>
			{
				History = new DatabaseHistory(null);
				History.Snapshot(new[] { "1", "2" });
				History.Snapshot(new[] { "3" });
				History.Snapshot(new[] { "1", "2" });
			};

		Because of = () => { Resurrected = History.Resurrected(); };

		It should_identify_the_files_that_was_resurrected =
			() => Resurrected.ShouldContainOnly("1", "2");
	}
}