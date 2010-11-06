using System.Linq;

using Machine.Specifications;

using Rhino.Mocks;

namespace DuplicateFinder.Core.Commands
{
	[Subject(typeof(FindDuplicatesCommand))]
	public class When_duplicates_are_searched_and_there_are_no_duplicates
	{
		static FindDuplicatesCommand Command;
		static IDuplicateFinder DuplicateFinder;

		Establish context = () =>
			{
				DuplicateFinder = MockRepository.GenerateStub<IDuplicateFinder>();
				DuplicateFinder
					.Stub(x => x.FindDuplicates())
					.Return(new string[][] { });

				Command = new FindDuplicatesCommand(MockRepository.GenerateStub<IOutput>(),
				                                    DuplicateFinder,
				                                    MockRepository.GenerateStub<ISelectFilesToDelete>(),
													MockRepository.GenerateStub<IFileDeleter>());
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
				DuplicateFinder = MockRepository.GenerateStub<IDuplicateFinder>();
				DuplicateFinder
					.Stub(x => x.FindDuplicates())
					.Return(new[] { new[] { "file 1", "file 2" }, new[] { "file 3", "file 4", "file 5" } });

				Output = MockRepository.GenerateStub<IOutput>();
				Deleter = MockRepository.GenerateStub<IFileDeleter>();

				var deletionSelector = MockRepository.GenerateStub<ISelectFilesToDelete>();
				deletionSelector
					.Stub(x => x.FilesToDelete(null))
					.IgnoreArguments()
					.WhenCalled(x => x.ReturnValue = x.Arguments.First());

				Command = new FindDuplicatesCommand(Output,
				                                    DuplicateFinder,
				                                    deletionSelector,
													Deleter);
			};

		Because of = () => Command.Execute();

		It should_delete_all_duplicates =
			() => Deleter.AssertWasCalled(x => x.Delete(Arg<string>.Matches(y => y.Contains("file"))), o => o.Repeat.Times(5));

		It should_print_the_number_of_bytes_that_were_deleted =
			() => Output.AssertWasCalled(x => x.WriteLine("0 Bytes deleted."));
	}
}