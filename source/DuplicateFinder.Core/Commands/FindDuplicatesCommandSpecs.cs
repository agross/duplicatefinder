using System.Collections.Generic;
using System.Linq;

using FakeItEasy;

using Machine.Specifications;

namespace DuplicateFinder.Core.Commands
{
	[Subject(typeof(FindDuplicatesCommand))]
	public class When_duplicates_are_searched_and_there_are_no_duplicates
	{
		static FindDuplicatesCommand Command;
		static IDuplicateFinder DuplicateFinder;

		Establish context = () =>
			{
				DuplicateFinder = A.Fake<IDuplicateFinder>();
				A
                  .CallTo(() => DuplicateFinder.FindDuplicates())
                  .Returns(new string[][] { });

				Command = new FindDuplicatesCommand(A.Fake<IOutput>(),
				                                    DuplicateFinder,
				                                    A.Fake<ISelectFilesToDelete>(),
													A.Fake<IFileDeleter>());
			};

		Because of = () => Command.Execute();

		It should_succeed =
			() => true.ShouldBeTrue();
	}

	[Subject(typeof(FindDuplicatesCommand))]
	public class When_duplicates_are_searched_and_there_are_duplicates
	{
		static FindDuplicatesCommand Command;
		static IDuplicateFinder DuplicateFinder;
		static IOutput Output;
		static IFileDeleter Deleter;

		Establish context = () =>
			{
				DuplicateFinder = A.Fake<IDuplicateFinder>();
				A
                  .CallTo(() => DuplicateFinder.FindDuplicates())
				  .Returns(new[] { new[] { "file 1", "file 2" }, new[] { "file 3", "file 4", "file 5" } });

				Output = A.Fake<IOutput>();
				Deleter = A.Fake<IFileDeleter>();

				var deletionSelector = A.Fake<ISelectFilesToDelete>();
				A
					.CallTo(() => deletionSelector.FilesToDelete(null))
					.WithAnyArguments()
					.ReturnsLazily(x => (IEnumerable<string>) x.Arguments.First());

				Command = new FindDuplicatesCommand(Output,
				                                    DuplicateFinder,
				                                    deletionSelector,
													Deleter);
			};

		Because of = () => Command.Execute();

		It should_delete_all_duplicates =
			() => A
				.CallTo(() => Deleter.Delete(A<string>.That.Matches(y => y.Contains("file"))))
				.MustHaveHappened(Repeated.Exactly.Times(5));

		It should_print_the_number_of_bytes_that_were_deleted =
			() => A
				.CallTo(()=> Output.WriteLine("0 Bytes deleted."))
				.MustHaveHappened();
	}
}