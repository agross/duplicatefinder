using System.IO;

using Machine.Specifications;

using Rhino.Mocks;

namespace DuplicateFinder.Core.Streams
{
	[Subject(typeof(TailStreamDecorator))]
	public class When_a_stream_s_tail_is_requested
	{
		static TailStreamDecorator Decorator;
		static Stream Decorated;

		Establish context = () => { Decorator = new TailStreamDecorator(42); };

		Because of = () => { Decorated = Decorator.GetStream(MockRepository.GenerateStub<Stream>()); };

		It should_create_a_tail_stream =
			() => Decorated.ShouldBeOfType<TailStream>();
	}
}