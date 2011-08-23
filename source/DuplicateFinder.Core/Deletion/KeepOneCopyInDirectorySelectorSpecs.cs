using System.Collections.Generic;

using Machine.Specifications;

namespace DuplicateFinder.Core.Deletion
{
	[Subject(typeof(KeepOneCopyInDirectorySelector))]
	public class When_one_copy_in_a_certain_directory_should_be_kept
	{
		static KeepOneCopyInDirectorySelector Selector;
		static IEnumerable<string> ToDelete;

		Establish context = () => { Selector = new KeepOneCopyInDirectorySelector("keep"); };

		Because of = () => { ToDelete = Selector.FilesToDelete(new[] { @"no-keep\1", @"keep\2", @"keep\3", @"no-keep\4" }); };

		It should_select_all_but_the_first_file_in_the_directory_to_keep =
			() => ToDelete.ShouldContainOnly(@"no-keep\1", @"keep\3", @"no-keep\4");
	}

	[Subject(typeof(KeepOneCopyInDirectorySelector))]
	public class When_one_copy_in_a_certain_directory_should_be_kept_and_all_duplicates_are_outside_of_this_directory
	{
		static KeepOneCopyInDirectorySelector Selector;
		static IEnumerable<string> ToDelete;

		Establish context = () => { Selector = new KeepOneCopyInDirectorySelector("keep"); };

		Because of = () => { ToDelete = Selector.FilesToDelete(new[] { @"no-keep\1", @"no-keep\2", @"no-keep\3", @"no-keep\4" }); };

		It should_select_all_but_the_last_file =
			() => ToDelete.ShouldContainOnly(@"no-keep\1", @"no-keep\2", @"no-keep\3");
	}
}