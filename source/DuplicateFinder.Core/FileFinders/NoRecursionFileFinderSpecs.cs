using System.Collections.Generic;
using System.Linq;

using FakeItEasy;

using Machine.Specifications;

namespace DuplicateFinder.Core.FileFinders
{
  [Subject(typeof(NoRecursionFileFinder))]
  public class When_files_are_searched_without_recursion
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
        .CallTo(() => FileSystem.AllFilesWithin(@"c:\some\path", false))
        .Returns(FilesOnDisk.Take(2));

      Finder = new NoRecursionFileFinder(FileSystem, @"c:\some\path");
    };

    Because of = () => { Files = Finder.GetFiles(); };

    It should_yield_the_list_of_files =
      () => Files.ShouldContainOnly(FilesOnDisk.Take(2));
  }
}
