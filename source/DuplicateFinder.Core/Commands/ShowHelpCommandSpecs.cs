using System.IO;

using Machine.Specifications;

using NDesk.Options;

using Rhino.Mocks;

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

				Output = MockRepository.GenerateStub<IOutput>();
				Output
					.Stub(x => x.GetTextWriter())
					.Return(OutputString);

				var options = new OptionSet { { "foo", v => { } } };

				Command = new ShowHelpCommand(options, Output, "some message");
			};

		Because of = () => Command.Execute();

		It should_display_messages =
			() => Output.AssertWasCalled(x => x.WriteLine("some message"));

		It should_display_the_help_contents =
			() => OutputString.GetStringBuilder().ToString().ShouldContain("foo");
	}
}