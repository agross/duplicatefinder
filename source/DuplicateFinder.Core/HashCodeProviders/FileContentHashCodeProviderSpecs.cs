using System.Collections.Generic;
using System.IO;
using System.Linq;

using Machine.Specifications;
using Machine.Specifications.Utility;

using Rhino.Mocks;

namespace DuplicateFinder.Core.HashCodeProviders
{
	[Subject(typeof(FileContentHashCodeProvider))]
	public class When_the_hash_code_is_calculated_from_the_file_contents
	{
		static IHashCodeProvider Provider;
		static IEnumerable<string> Hash;
		static IFileSystem FileSystem;

		Establish context = () =>
			{
				FileSystem = MockRepository.GenerateStub<IFileSystem>();
				FileSystem
					.Stub(x => x.CreateStreamFrom(null))
					.IgnoreArguments()
					.Return(MockRepository.GenerateStub<Stream>());

				Provider = new FileContentHashCodeProvider(FileSystem);
			};

		Because of = () => { Hash = Provider.CalculateHashCode(@"c:\some\file.txt").ToArray(); };

		It should_read_the_file_contents =
			() => FileSystem.AssertWasCalled(x => x.CreateStreamFrom(@"c:\some\file.txt"));

		It should_compute_the_hash_of_the_file_contents =
			() => Hash.First().ShouldNotBeEmpty();
	}

	[Subject(typeof(FileContentHashCodeProvider))]
	public class When_the_hash_code_is_limited_to_some_parts_of_the_file_contents
	{
		static IHashCodeProvider Provider;
		static IEnumerable<string> Hash;
		static IFileSystem FileSystem;
		static IStreamDecorator[] Decorators;
		static Stream AppliedStream;
		static Stream Stream;

		Establish context = () =>
			{
				Stream = MockRepository.GenerateStub<Stream>();

				FileSystem = MockRepository.GenerateStub<IFileSystem>();
				FileSystem
					.Stub(x => x.CreateStreamFrom(@"c:\some\file.txt"))
					.Return(Stream);

				Decorators = new[]
				             {
				             	MockRepository.GenerateStub<IStreamDecorator>(),
				             	MockRepository.GenerateStub<IStreamDecorator>()
				             };

				AppliedStream = MockRepository.GenerateStub<Stream>();

				Decorators.Each(x => x
				                     	.Stub(y => y.GetStream(null))
				                     	.IgnoreArguments()
				                     	.Return(AppliedStream));

				Provider = new FileContentHashCodeProvider(FileSystem, Decorators);
			};

		Because of = () => { Hash = Provider.CalculateHashCode(@"c:\some\file.txt").ToArray(); };

		It should_apply_each_stream_processor_to_the_source_stream =
			() => Decorators.Each(x => x.AssertWasCalled(y => y.GetStream(Stream)));

		It should_compute_the_hash_of_the_file_contents_for_each_content_part =
			() => Hash.Count().ShouldEqual(2);
	}
}