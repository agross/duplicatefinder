using System.Collections.Generic;
using System.Linq;

using DuplicateFinder.Core.HashCodeHistory;

using FakeItEasy;

using Machine.Specifications;

namespace DuplicateFinder.Core.Commands
{
  [Subject(typeof(PruneHistoryCommand))]
  public class When_no_files_are_found
  {
    static PruneHistoryCommand Command;
    static IDuplicateFinder DuplicateFinder;
    static IRememberHashCodes History;

    Establish context = () =>
    {
      DuplicateFinder = A.Fake<IDuplicateFinder>();
      History = A.Fake<IRememberHashCodes>();

      Command = new PruneHistoryCommand(A.Fake<IOutput>(), DuplicateFinder, History);
    };

    Because of = () => Command.Execute();

    It should_not_forget_hash_codes =
      () =>
      A.CallTo(() => History.Forget(A<IEnumerable<string>>.That.IsEmpty(), DuplicateFinder.HashCodeProviders))
       .MustHaveHappened();
  }

  [Subject(typeof(PruneHistoryCommand))]
  public class When_files_are_found
  {
    static PruneHistoryCommand Command;
    static IRememberHashCodes History;
    static IDuplicateFinder DuplicateFinder;

    Establish context = () =>
    {
      DuplicateFinder = A.Fake<IDuplicateFinder>();
      A
        .CallTo(() => DuplicateFinder.CalculateHashes())
        .Returns(new[]
                 {
                   new { File = "foo", Hash = "1" },
                   new { File = "bar", Hash = "2" },
                   new { File = "baz", Hash = "2" }
                 }
                   .GroupBy(x => x.Hash, x => x.File));

      History = A.Fake<IRememberHashCodes>();
      Command = new PruneHistoryCommand(A.Fake<IOutput>(), DuplicateFinder, History);
    };

    Because of = () => Command.Execute();

    It should_forget_all_hash_codes =
      () => A
              .CallTo(
                      () =>
                      History.Forget(A<IEnumerable<string>>.That.IsSameSequenceAs(new[] { "1", "2" }),
                                     DuplicateFinder.HashCodeProviders))
              .MustHaveHappened();
  }
}
