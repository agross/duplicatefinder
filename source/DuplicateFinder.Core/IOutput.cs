using System.IO;

namespace DuplicateFinder.Core
{
	public interface IOutput
	{
		void WriteLine(string value);
		TextWriter GetTextWriter();
	}
}