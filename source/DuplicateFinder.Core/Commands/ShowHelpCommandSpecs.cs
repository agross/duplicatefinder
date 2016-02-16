using System.IO;

using FakeItEasy;

using Machine.Specifications;

using Mono.Options;

namespace DuplicateFinder.Core.Commands
{
  [Subject(typeof(ShowHelpCommand))]
  public class When_the_help_screen_is_displayed
  {
    static IOutput Output;
    static ShowHelpCommand Command;
    static StringWriter OutputString;

    Establish context = () =>
    {
      OutputString = new StringWriter();

      Output = A.Fake<IOutput>();
      A
        .CallTo((() => Output.GetTextWriter()))
        .Returns(OutputString);

      var options = new OptionSet { { "foo", v => { } } };

      Command = new ShowHelpCommand(options, Output, "some message");
    };

    Because of = () => Command.Execute();

    It should_display_messages =
      () => A
              .CallTo(() => Output.WriteLine("some message"))
              .MustHaveHappened();

    It should_display_the_help_contents =
      () => OutputString.GetStringBuilder().ToString().ShouldContain("foo");
  }
}
