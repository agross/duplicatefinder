using System;

using FakeItEasy;

using Machine.Specifications;

namespace DuplicateFinder.Core.Deletion
{
  [Subject(typeof(FileDeleter))]
  public class When_a_file_is_deleted
  {
    static FileDeleter Deleter;
    static long FileSize;
    static IFileSystem FileSystem;

    Establish context = () =>
    {
      FileSystem = A.Fake<IFileSystem>();
      A
        .CallTo((() => FileSystem.GetSize(@"c:\some\file")))
        .Returns(42);

      Deleter = new FileDeleter(FileSystem,
                                A.Fake<IOutput>());
    };

    Because of = () => { FileSize = Deleter.Delete(@"c:\some\file"); };

    It should_delete_the_file =
      () => A
              .CallTo(() => FileSystem.Delete(@"c:\some\file"))
              .MustHaveHappened();

    It should_return_the_file_s_size =
      () => FileSize.ShouldEqual(42);
  }

  [Subject(typeof(FileDeleter))]
  public class When_deleting_a_file_fails
  {
    static FileDeleter Deleter;
    static long FileSize;
    static IFileSystem FileSystem;

    Establish context = () =>
    {
      FileSystem = A.Fake<IFileSystem>();
      A
        .CallTo((() => FileSystem.GetSize(@"c:\some\file")))
        .Throws(new InvalidOperationException());

      Deleter = new FileDeleter(FileSystem,
                                A.Fake<IOutput>());
    };

    Because of = () => { FileSize = Deleter.Delete(@"c:\some\file"); };

    It should_swallow_the_error =
      () => true.ShouldBeTrue();

    It should_return_a_file_size_of_zero =
      () => FileSize.ShouldEqual(0);
  }
}
