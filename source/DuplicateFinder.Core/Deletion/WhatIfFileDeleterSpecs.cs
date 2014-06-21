using FakeItEasy;

using Machine.Specifications;

namespace DuplicateFinder.Core.Deletion
{
  [Subject(typeof(WhatIfFileDeleter))]
  public class When_not_deleting_files
  {
    static IFileDeleter Deleter;
    static long BytesDeleted;
    static IOutput Output;

    Establish context = () =>
    {
      Output = A.Fake<IOutput>();
      Deleter = new WhatIfFileDeleter(Output);
    };

    Because of = () => { BytesDeleted = Deleter.Delete("foo"); };

    It should_not_delete_files =
      () => BytesDeleted.ShouldEqual(0);

    It should_output_what_would_have_been_deleted =
      () => A.CallTo(() => Output.WriteLine(A<string>.That.Contains("foo")));
  }
}
