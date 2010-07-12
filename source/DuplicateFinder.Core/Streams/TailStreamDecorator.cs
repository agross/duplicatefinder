using System.IO;

namespace DuplicateFinder.Core.Streams
{
	internal class TailStreamDecorator : IStreamDecorator
	{
		readonly long _length;

		public TailStreamDecorator(long length)
		{
			_length = length;
		}

		public Stream GetStream(Stream stream)
		{
			return new TailStream(stream, _length);
		}
	}
}