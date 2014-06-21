using System.Collections.Generic;
using System.Linq;

using Machine.Specifications;

namespace DuplicateFinder.Core.HashCodeProviders
{
  [Subject(typeof(FileNameHashCodeProvider))]
  public class When_the_hash_code_is_calculated_from_the_file_name
  {
    static IHashCodeProvider Provider;
    static IEnumerable<string> Hash;

    Establish context = () => { Provider = new FileNameHashCodeProvider(); };

    Because of = () => { Hash = Provider.CalculateHashCode(@"c:\some\file.txt"); };

    It should_use_the_file_name_as_the_hash =
      () => Hash.First().ShouldEqual("file.txt");
  }
}
