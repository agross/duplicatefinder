using System;
using System.IO;

using Machine.Specifications;

namespace DuplicateFinder.Core.Integration.Tests
{
	public class When_seen_files_are_deleted_and_get_recreated
	{
		static ICommand Run1;

		Establish context =
			() =>
			{
				var parser = new CommandLineParser();

				Run1 = parser.Parse(@"--content --history history SeenAndGone\Run1_FileExists".Args());
				Run2 = parser.Parse(@"--content --history history SeenAndGone\Run2_FileWasDeleted".Args());
				Run3 = parser.Parse(@"--content --history history SeenAndGone\Run3_FileIsRecreated".Args());
			};

		Because of = () =>
			{
				Run1.Execute();
				Run2.Execute();
				Run3.Execute();
			};

		It should_delete_the_file_that_was_recreated =
			() => File.Exists(@"SeenAndGone\Run3_FileIsRecreated\match.txt").ShouldBeFalse();

		static ICommand Run2;
		static ICommand Run3;
	}
}