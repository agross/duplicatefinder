using System.IO;

using Machine.Specifications;

using Rhino.Mocks;

namespace DuplicateFinder.Core.Streams
{
	[Subject(typeof(HeadStreamDecorator))]
	public class When_a_stream_s_head_is_requested
	{
		static HeadStreamDecorator Decorator;
		static Stream Decorated;

		Establish context = () => { Decorator = new HeadStreamDecorator(42); };

		Because of = () => { Decorated = Decorator.GetStream(MockRepository.GenerateStub<Stream>()); };

		It should_create_a_head_stream =
			() => Decorated.ShouldBeOfType<HeadStream>();
	}
}