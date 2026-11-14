using System;
using System.IO;

namespace SafeExamBrowser.Configuration;

internal class SubStream : Stream
{
	private long length;

	private long offset;

	private Stream original;

	public override bool CanRead => original.CanRead;

	public override bool CanSeek => original.CanSeek;

	public override bool CanWrite => false;

	public override long Length => length;

	public override long Position { get; set; }

	public SubStream(Stream original, long offset, long length)
	{
		this.original = original;
		this.offset = offset;
		this.length = length;
		if (!original.CanRead)
		{
			throw new ArgumentException("The original stream must support reading!", "original");
		}
		if (!original.CanSeek)
		{
			throw new ArgumentException("The original stream must support seeking!", "original");
		}
		if (original.Length < offset + length || offset < 0 || length < 1)
		{
			throw new ArgumentOutOfRangeException("Specified subsection is outside the bounds of the original stream!");
		}
	}

	public override void Flush()
	{
		throw new NotSupportedException();
	}

	public override int Read(byte[] buffer, int offset, int count)
	{
		long position = original.Position;
		if (Position < 0 || Position >= Length)
		{
			return 0;
		}
		if (Position + count >= Length)
		{
			count = Convert.ToInt32(Length - Position);
		}
		original.Seek(this.offset + Position, SeekOrigin.Begin);
		int num = original.Read(buffer, offset, count);
		Position += num;
		original.Seek(position, SeekOrigin.Begin);
		return num;
	}

	public override int ReadByte()
	{
		if (Position < 0 || Position >= Length)
		{
			return -1;
		}
		return base.ReadByte();
	}

	public override long Seek(long offset, SeekOrigin origin)
	{
		switch (origin)
		{
		case SeekOrigin.Begin:
			Position = offset;
			break;
		case SeekOrigin.Current:
			Position += offset;
			break;
		case SeekOrigin.End:
			Position = length + offset;
			break;
		default:
			throw new NotImplementedException($"Seeking from position '{origin}' is not implemented!");
		}
		return Position;
	}

	public override void SetLength(long value)
	{
		throw new NotSupportedException();
	}

	public override void Write(byte[] buffer, int offset, int count)
	{
		throw new NotSupportedException();
	}
}
