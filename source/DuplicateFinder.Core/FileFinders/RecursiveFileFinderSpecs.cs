using System.Collections.Generic;

using FakeItEasy;

using Machine.Specifications;

namespace DuplicateFinder.Core.FileFinders
{
  [Subject(typeof(RecursiveFileFinder))]
  public class When_files_are_searched_recursively
  {
    static IFileFinder Finder;
    static IEnumerable<string> Files;
    static IFileSystem FileSystem;
    static string[] FilesOnDisk = new[]
    {
      @"c:\some\path\file1.txt",
      @"c:\some\path\file2.txt",
      @"c:\some\path\subdir\file1.txt"
    };

    Establish context = () =>
    {
      FileSystem = A.Fake<IFileSystem>();
      A
        .CallTo(() => FileSystem.AllFilesWithin(@"c:\some\path", true))
        .Returns(FilesOnDisk);

      Finder = new RecursiveFileFinder(FileSystem, @"c:\some\path");
    };

    Because of = () => { Files = Finder.GetFiles(); };

    It should_yield_the_list_of_files =
      () => Files.ShouldContainOnly(FilesOnDisk);
  }
}
