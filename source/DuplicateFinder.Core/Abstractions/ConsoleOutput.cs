using System;
using System.IO;

namespace DuplicateFinder.Core.Abstractions
{
	class ConsoleOutput : IOutput
	{
		public void WriteLine(string value)
		{
			Console.WriteLine(value);
		}

		public TextWriter GetTextWriter()
		{
			return Console.Out;
		}
	}
}