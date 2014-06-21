using System.Collections.Generic;
using System.IO;
using System.Linq;

using FakeItEasy;

using Machine.Specifications;
using Machine.Specifications.Utility;

namespace DuplicateFinder.Core.HashCodeProviders
{
  [Subject(typeof(FileContentHashCodeProvider))]
  public class When_the_hash_code_is_calculated_from_the_file_contents
  {
    static IHashCodeProvider Provider;
    static IEnumerable<string> Hash;
    static IFileSystem FileSystem;

    Establish context = () =>
    {
      FileSystem = A.Fake<IFileSystem>();
      A
        .CallTo((() => FileSystem.CreateStreamFrom(null)))
        .WithAnyArguments()
        .Returns(A.Fake<Stream>());

      Provider = new FileContentHashCodeProvider(FileSystem);
    };

    Because of = () => { Hash = Provider.CalculateHashCode(@"c:\some\file.txt").ToArray(); };

    It should_read_the_file_contents =
      () => A
              .CallTo((() => FileSystem.CreateStreamFrom(@"c:\some\file.txt")))
              .MustHaveHappened();

    It should_compute_the_hash_of_the_file_contents =
      () => Hash.First().ShouldNotBeEmpty();
  }

  [Subject(typeof(FileContentHashCodeProvider))]
  public class When_the_hash_code_is_limited_to_some_parts_of_the_file_contents
  {
    static IHashCodeProvider Provider;
    static IEnumerable<string> Hash;
    static IFileSystem FileSystem;
    static IStreamDecorator[] Decorators;
    static Stream AppliedStream;
    static Stream Stream;

    Establish context = () =>
    {
      Stream = A.Fake<Stream>();

      FileSystem = A.Fake<IFileSystem>();
      A
        .CallTo((() => FileSystem.CreateStreamFrom(@"c:\some\file.txt")))
        .Returns(Stream);

      Decorators = new[]
                   {
                     A.Fake<IStreamDecorator>(),
                     A.Fake<IStreamDecorator>()
                   };

      AppliedStream = A.Fake<Stream>();

      Decorators.Each(x => A
                             .CallTo(() => x.GetStream(null))
                             .WithAnyArguments()
                             .Returns(AppliedStream));

      Provider = new FileContentHashCodeProvider(FileSystem, Decorators);
    };

    Because of = () => { Hash = Provider.CalculateHashCode(@"c:\some\file.txt").ToArray(); };

    It should_apply_each_stream_processor_to_the_source_stream =
      () => Decorators.Each(x => A.CallTo(() => x.GetStream(Stream)).MustHaveHappened());

    It should_compute_the_hash_of_the_file_contents_for_each_content_part =
      () => Hash.Count().ShouldEqual(2);
  }
}
