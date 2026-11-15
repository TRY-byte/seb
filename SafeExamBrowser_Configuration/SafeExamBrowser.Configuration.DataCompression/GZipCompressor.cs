using System;
using System.IO;
using System.IO.Compression;
using SafeExamBrowser.Configuration.Contracts.DataCompression;
using SafeExamBrowser.Logging.Contracts;

namespace SafeExamBrowser.Configuration.DataCompression;

public class GZipCompressor : IDataCompressor
{
	private const int ID1 = 31;

	private const int ID2 = 139;

	private const int CM = 8;

	private const int FOOTER_LENGTH = 8;

	private const int HEADER_LENGTH = 10;

	private ILogger logger;

	public GZipCompressor(ILogger logger)
	{
		this.logger = logger;
	}

	public Stream Compress(Stream data)
	{
		MemoryStream memoryStream = new MemoryStream();
		double num = (double)data.Length / 1000.0;
		logger.Debug($"Starting compression of '{data}' with {num} KB data...");
		data.Seek(0L, SeekOrigin.Begin);
		using (GZipStream destination = new GZipStream(memoryStream, CompressionMode.Compress, leaveOpen: true))
		{
			data.CopyTo(destination);
		}
		logger.Debug($"Successfully compressed {num} KB to {(double)memoryStream.Length / 1000.0} KB data.");
		return memoryStream;
	}

	public Stream Decompress(Stream data)
	{
		MemoryStream memoryStream = new MemoryStream();
		double num = (double)data.Length / 1000.0;
		logger.Debug($"Starting decompression of '{data}' with {num} KB data...");
		data.Seek(0L, SeekOrigin.Begin);
		using (GZipStream gZipStream = new GZipStream(data, CompressionMode.Decompress))
		{
			gZipStream.CopyTo(memoryStream);
		}
		logger.Debug($"Successfully decompressed {num} KB to {(double)memoryStream.Length / 1000.0} KB data.");
		return memoryStream;
	}

	public bool IsCompressed(Stream data)
	{
		try
		{
			bool flag = data.Length > 18;
			data.Seek(0L, SeekOrigin.Begin);
			if (flag)
			{
				int num = data.ReadByte();
				int num2 = data.ReadByte();
				int num3 = data.ReadByte();
				return num == 31 && num2 == 139 && num3 == 8;
			}
			logger.Debug($"'{data}' is not long enough ({data.Length} bytes) to be a gzip-compressed stream.");
		}
		catch (Exception ex)
		{
			logger.Error($"Failed to check whether '{data}' with {(double?)data?.Length / 1000.0} KB data is a gzip-compressed stream!", ex);
		}
		return false;
	}

	public byte[] Peek(Stream data, int count)
	{
		data.Seek(0L, SeekOrigin.Begin);
		using GZipStream gZipStream = new GZipStream(data, CompressionMode.Decompress, leaveOpen: true);
		using MemoryStream memoryStream = new MemoryStream();
		byte[] array = new byte[count];
		int count2 = gZipStream.Read(array, 0, array.Length);
		memoryStream.Write(array, 0, count2);
		return memoryStream.ToArray();
	}
}
