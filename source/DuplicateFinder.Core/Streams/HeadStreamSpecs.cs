using System;
using System.IO;
using System.Linq;
using System.Text;

using Machine.Specifications;

namespace DuplicateFinder.Core.Streams
{
  [Subject(typeof(HeadStream))]
  public class When_a_head_stream_is_created
  {
    static Stream Stream;
    static Stream Inner;

    Establish context = () =>
    {
      var bytes = Enumerable
        .Range(1, StreamLength)
        .Select(Convert.ToByte)
        .ToArray();

      Inner = new MemoryStream(bytes);
    };

    Because of = () => { Stream = new HeadStream(Inner, Head); };

    It should_report_the_head_length_as_the_stream_length =
      () => Stream.Length.ShouldEqual(Head);

    It should_report_zero_as_the_current_seek_position =
      () => Stream.Position.ShouldEqual(0);

    const int StreamLength = 200;
    const int Head = 20;
  }

  [Subject(typeof(HeadStream))]
  public class When_a_head_stream_is_closed
  {
    static Stream Stream;
    static Stream Inner;

    Establish context = () =>
    {
      Inner = new MemoryStream();
      Stream = new HeadStream(Inner, 1);
    };

    Because of = () => Stream.Close();

    It should_close_the_inner_stream =
      () => Inner.CanRead.ShouldBeFalse();
  }

  [Subject(typeof(HeadStream))]
  public class When_a_head_stream_is_created_with_the_head_being_longer_than_the_stream
  {
    static Stream Stream;
    static Stream Inner;

    Establish context = () =>
    {
      var bytes = Enumerable
        .Range(1, StreamLength)
        .Select(Convert.ToByte)
        .ToArray();

      Inner = new MemoryStream(bytes);
    };

    Because of = () => { Stream = new HeadStream(Inner, Head); };

    It should_report_the_inner_stream_s_length_as_the_stream_length =
      () => Stream.Length.ShouldEqual(StreamLength);

    It should_report_zero_as_the_current_seek_position =
      () => Stream.Position.ShouldEqual(0);

    const int StreamLength = 20;
    const int Head = 200;
  }

  [Subject(typeof(HeadStream), "Reading")]
  public class When_the_head_of_a_stream_is_read
  {
    static Stream Stream;
    static Stream Inner;
    static string Contents;
    static string HeadBytes;
    static Encoding StreamEncoding;

    Establish context = () =>
    {
      StreamEncoding = Encoding.Default;

      var bytes = Enumerable
        .Range(1, StreamLength)
        .Select(Convert.ToByte).ToArray();

      HeadBytes = StreamEncoding.GetString(bytes.Take(Head).ToArray());

      Inner = new MemoryStream(bytes);

      Stream = new HeadStream(Inner, Head);
    };

    Because of = () => { Contents = new StreamReader(Stream, StreamEncoding).ReadToEnd(); };

    It should_yield_the_specified_number_of_head_bytes =
      () => Contents.Length.ShouldEqual(Head);

    It should_yield_the_head_bytes =
      () => Contents.ShouldEqual(HeadBytes);

    It should_report_the_head_as_the_current_seek_position =
      () => Stream.Position.ShouldEqual(Head);

    const int StreamLength = 200;
    const int Head = 20;
  }

  [Subject(typeof(HeadStream), "Reading")]
  public class When_the_head_is_longer_than_the_stream
  {
    static Stream Stream;
    static Stream Inner;
    static string Contents;
    static string HeadBytes;
    static Encoding StreamEncoding;

    Establish context = () =>
    {
      StreamEncoding = Encoding.Default;

      var bytes = Enumerable
        .Range(1, StreamLength)
        .Select(Convert.ToByte)
        .ToArray();

      HeadBytes = StreamEncoding.GetString(bytes);

      Inner = new MemoryStream(bytes);

      Stream = new HeadStream(Inner, Head);
    };

    Because of = () => { Contents = new StreamReader(Stream, StreamEncoding).ReadToEnd(); };

    It should_yield_the_stream_s_number_of_bytes =
      () => Contents.Length.ShouldEqual(StreamLength);

    It should_yield_all_bytes =
      () => Contents.ShouldEqual(HeadBytes);

    It should_report_the_stream_length_as_the_current_seek_position =
      () => Stream.Position.ShouldEqual(StreamLength);

    const int StreamLength = 20;
    const int Head = 200;
  }
}
