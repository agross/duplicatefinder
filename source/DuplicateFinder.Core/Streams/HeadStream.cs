using System;
using System.IO;

namespace DuplicateFinder.Core.Streams
{
	internal class HeadStream : Stream
	{
		readonly long _headBytes;
		readonly Stream _inner;

		public HeadStream(Stream inner, long headBytes)
		{
			_inner = inner;
			_headBytes = headBytes;
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
			get { return _headBytes <= _inner.Length ? _headBytes : _inner.Length; }
		}

		public override long Position
		{
			get { return _inner.Position; }
			set { Seek(value, SeekOrigin.Begin); }
		}

		public override void Flush()
		{
			throw new NotSupportedException();
		}

		public override long Seek(long offset, SeekOrigin origin)
		{
			var seekTo = offset;
			if (origin == SeekOrigin.End)
			{
				seekTo -= RelativeInnerPosition();
			}

			return _inner.Seek(seekTo, origin);
		}

		public override void SetLength(long value)
		{
			throw new NotSupportedException();
		}

		public override int Read(byte[] buffer, int offset, int count)
		{
			if (Position >= _headBytes)
			{
				return 0;
			}

			var bytesRead = _inner.Read(buffer, offset, count);
			if (bytesRead + Position >= _headBytes)
			{
				Seek(_headBytes, SeekOrigin.Begin);
				bytesRead = (int) _headBytes;
			}

			return bytesRead;
		}

		public override void Write(byte[] buffer, int offset, int count)
		{
			throw new NotSupportedException();
		}

		long RelativeInnerPosition()
		{
			var offset = _inner.Length - _headBytes;
			if (offset < 0)
			{
				offset = 0;
			}
			return offset;
		}

		public override void Close()
		{
			_inner.Close();
			base.Close();
		}
	}
}