using System.IO;

using FakeItEasy;

using Machine.Specifications;

namespace DuplicateFinder.Core.Streams
{
  [Subject(typeof(HeadStreamDecorator))]
  public class When_a_stream_s_head_is_requested
  {
    static HeadStreamDecorator Decorator;
    static Stream Decorated;

    Establish context = () => { Decorator = new HeadStreamDecorator(42); };

    Because of = () => { Decorated = Decorator.GetStream(A.Fake<Stream>()); };

    It should_create_a_head_stream =
      () => Decorated.ShouldBeOfExactType<HeadStream>();
  }
}
