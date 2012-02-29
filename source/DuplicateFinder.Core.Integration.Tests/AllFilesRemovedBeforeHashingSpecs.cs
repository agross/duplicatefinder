using System.Collections.Generic;
using System.IO;
using System.Linq;

using DuplicateFinder.Core.HashCodeHistory;

using Machine.Specifications;

namespace DuplicateFinder.Core.Integration.Tests
{
	[Tags("integration")]
	public class When_all_files_are_removed_before_they_are_hashed
	{
		static DuplicateFinder Finder;

		Establish context =
			() =>
				{
					Output = new StringOutput();

					Finder = new DuplicateFinder(new[] { new NonExistingFilesFinder() },
					                             new[] { new ThrowingHashCodeProvider() },
					                             Output,
					                             new NullHistory());
				};

		Because of = () => Finder.FindDuplicates().Duplicates.ToArray();

		It should_succeed =
			() => true.ShouldBeTrue();

		It should_log_the_inaccessible_files =
			() => Output.GetTextWriter().ToString().ShouldContain("File is inaccessible or has been deleted: a file that does not exist");

		static StringOutput Output;
	}

	internal class NonExistingFilesFinder : IFileFinder
	{
		public IEnumerable<string> GetFiles()
		{
			yield return "a file that does not exist";
		}
	}

	internal class ThrowingHashCodeProvider : IHashCodeProvider
	{
		public IEnumerable<string> CalculateHashCode(string path)
		{
			throw new FileNotFoundException(path);
		}
	}

	internal class StringOutput : IOutput
	{
		readonly StringWriter _writer;

		public StringOutput()
		{
			_writer = new StringWriter();
		}

		public void WriteLine(string value)
		{
			_writer.WriteLine(value);
		}

		public TextWriter GetTextWriter()
		{
			return _writer;
		}
	}
}