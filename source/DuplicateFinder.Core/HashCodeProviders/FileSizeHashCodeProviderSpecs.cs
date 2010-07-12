using System.Collections.Generic;
using System.Linq;

using Machine.Specifications;

using Rhino.Mocks;

namespace DuplicateFinder.Core.HashCodeProviders
{
	[Subject(typeof(FileSizeHashCodeProvider))]
	public class When_the_hash_code_is_calculated_from_the_file_size
	{
		static IHashCodeProvider Provider;
		static IEnumerable<string> Hash;
		static IFileSystem FileSystem;

		Establish context = () =>
			{
				FileSystem = MockRepository.GenerateStub<IFileSystem>();
				FileSystem
					.Stub(x => x.GetSize(@"c:\some\file.txt"))
					.Return(1234567890);

				Provider = new FileSizeHashCodeProvider(FileSystem);
			};

		Because of = () => { Hash = Provider.CalculateHashCode(@"c:\some\file.txt"); };

		It should_use_the_file_size_as_the_hash_value =
			() => Hash.First().ShouldEqual("1234567890");
	}
}