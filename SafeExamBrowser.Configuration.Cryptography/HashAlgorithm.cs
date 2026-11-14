using System.Linq;
using System.Security.Cryptography;
using System.Text;
using SafeExamBrowser.Configuration.Contracts.Cryptography;

namespace SafeExamBrowser.Configuration.Cryptography;

public class HashAlgorithm : IHashAlgorithm
{
	public string GenerateHashFor(string password)
	{
		using SHA256Managed sHA256Managed = new SHA256Managed();
		byte[] bytes = Encoding.UTF8.GetBytes(password);
		byte[] source = sHA256Managed.ComputeHash(bytes);
		return string.Join(string.Empty, source.Select((byte b) => b.ToString("x2")));
	}
}
