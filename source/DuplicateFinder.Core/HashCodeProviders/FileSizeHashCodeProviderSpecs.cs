using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

using FakeItEasy;

using Machine.Specifications;

namespace DuplicateFinder.Core.HashCodeProviders
{
  [Subject(typeof(FileSizeHashCodeProvider))]
  public class When_the_hash_code_is_calculated_from_the_file_size
  {
    static IHashCodeProvider Provider;
    static IEnumerable<string> Hash;
    static IFileSystem FileSystem;

    Establish context = () =>
    {
      FileSystem = A.Fake<IFileSystem>();
      A
        .CallTo((() => FileSystem.GetSize(@"c:\some\file.txt")))
        .Returns(1234567890);

      Provider = new FileSizeHashCodeProvider(FileSystem);
    };

    Because of = () => { Hash = Provider.CalculateHashCode(@"c:\some\file.txt"); };

    It should_use_the_file_size_as_the_hash_value =
      () => Hash.First().ShouldEqual("1234567890");
  }
}
