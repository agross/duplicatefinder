using System.IO;

using DuplicateFinder.Core.CommandLine;

using Machine.Specifications;

namespace DuplicateFinder.Core.IntegrationSpecs
{
  [Tags("integration")]
  public class When_recursion_is_disabled
  {
    static ICommand Command;

    Establish context =
      () => Command =
              new CommandLineParser()
                .Parse("--no-recursion --content NoRecursion".Args());

    Because of = () => Command.Execute();

    It should_not_consider_subdirectories =
      () =>
      {
        File.Exists(@"NoRecursion\subdir-1\would-be-duplicate.txt").ShouldBeTrue();
        File.Exists(@"NoRecursion\subdir-2\would-be-duplicate.txt").ShouldBeTrue();
      };
  }
}
