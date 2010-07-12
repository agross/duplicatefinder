using System.Collections.Generic;

using Machine.Specifications;

using Rhino.Mocks;

namespace DuplicateFinder.Core
{
	[Subject(typeof(RecursiveFileFinder))]
	public class When_files_are_searched_recursively
	{
		static IFileFinder Finder;
		static IEnumerable<string> Files;
		static IFileSystem FileSystem;

		Establish context = () =>
			{
				FileSystem = MockRepository.GenerateStub<IFileSystem>();
				FileSystem
					.Stub(x => x.AllFilesWithin(@"c:\some\path"))
					.Return(new[] { @"c:\some\path\file1.txt", @"c:\some\path\file2.txt" });

				Finder = new RecursiveFileFinder(FileSystem, @"c:\some\path");
			};

		Because of = () => { Files = Finder.GetFiles(); };

		It should_yield_the_list_of_files =
			() => Files.ShouldContainOnly(@"c:\some\path\file1.txt", @"c:\some\path\file2.txt");
	}
}