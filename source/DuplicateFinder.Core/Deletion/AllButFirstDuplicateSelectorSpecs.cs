using System.Collections.Generic;

using Machine.Specifications;

namespace DuplicateFinder.Core.Deletion
{
	[Subject(typeof(AllButFirstDuplicateSelector))]
	public class When_all_but_the_first_duplicate_should_be_deleted
	{
		static AllButFirstDuplicateSelector Selector;
		static IEnumerable<string> ToDelete;

		Establish context = () => { Selector = new AllButFirstDuplicateSelector(); };

		Because of = () => { ToDelete = Selector.FilesToDelete(new[] { "1", "2", "3" }); };

		It should_select_all_but_the_first_file =
			() => ToDelete.ShouldContainOnly("2", "3");
	}
}