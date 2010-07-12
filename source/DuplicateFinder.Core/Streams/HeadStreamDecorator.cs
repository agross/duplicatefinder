using System.IO;

namespace DuplicateFinder.Core.Streams
{
	internal class HeadStreamDecorator : IStreamDecorator
	{
		readonly long _length;

		public HeadStreamDecorator(long length)
		{
			_length = length;
		}

		public Stream GetStream(Stream stream)
		{
			return new HeadStream(stream, _length);
		}
	}
}