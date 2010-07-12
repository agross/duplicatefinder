using System;
using System.IO;

namespace DuplicateFinder.Core.Streams
{
	public class TailStream : Stream
	{
		readonly Stream _inner;
		readonly long _tailBytes;

		public TailStream(Stream inner, long tailBytes)
		{
			_inner = inner;
			_tailBytes = tailBytes;
			Seek(0, SeekOrigin.Begin);
		}

		public override bool CanRead
		{
			get { return _inner.CanRead; }
		}

		public override bool CanSeek
		{
			get { return _inner.CanSeek; }
		}

		public override bool CanWrite
		{
			get { return false; }
		}

		public override long Length
		{
			get { return _tailBytes <= _inner.Length ? _tailBytes : _inner.Length; }
		}

		public override long Position
		{
			get { return _inner.Position - RelativeInnerPosition(); }
			set { Seek(value, SeekOrigin.Begin); }
		}

		public override void Flush()
		{
			throw new NotSupportedException();
		}

		public override long Seek(long offset, SeekOrigin origin)
		{
			var seekTo = offset;
			if (origin == SeekOrigin.Begin)
			{
				seekTo += RelativeInnerPosition();
			}

			return _inner.Seek(seekTo, origin);
		}

		public override void SetLength(long value)
		{
			throw new NotSupportedException();
		}

		public override int Read(byte[] buffer, int offset, int count)
		{
			return _inner.Read(buffer, offset, count);
		}

		public override void Write(byte[] buffer, int offset, int count)
		{
			throw new NotSupportedException();
		}

		long RelativeInnerPosition()
		{
			var offset = _inner.Length - _tailBytes;
			if (offset < 0)
			{
				offset = 0;
			}
			return offset;
		}
	}
}