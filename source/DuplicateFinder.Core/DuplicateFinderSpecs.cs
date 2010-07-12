using System.Collections.Generic;
using System.Linq;

using DuplicateFinder.Core.HashCodeProviders;

using Machine.Specifications;
using Machine.Specifications.Utility;

using Rhino.Mocks;

namespace DuplicateFinder.Core
{
	[Subject(typeof(DuplicateFinder))]
	public class When_duplicates_are_searched
	{
		static IDuplicateFinder Finder;
		static IFileFinder[] FileFinders;
		static IHashCodeProvider[] HashCodeProviders;
		static IEnumerable<string>[] Duplicates;

		Establish context = () =>
			{
				FileFinders = new[]
				              {
				              	MockRepository.GenerateStub<IFileFinder>(),
				              	MockRepository.GenerateStub<IFileFinder>()
				              };
				FileFinders
					.First()
					.Stub(x => x.GetFiles())
					.Return(new[] { @"c:\path\dup 1.txt", @"c:\path\dup 2.txt" });

				FileFinders
					.Skip(1).First()
					.Stub(x => x.GetFiles())
					.Return(new[] { @"c:\other path\dup 3.txt", @"c:\other path\no dup.txt" });

				HashCodeProviders = new[]
				                    {
				                    	MockRepository.GenerateStub<IHashCodeProvider>(),
				                    	MockRepository.GenerateStub<IHashCodeProvider>()
				                    };

				HashCodeProviders
					.First()
					.Stub(y => y.CalculateHashCode(Arg<string>.Matches(s => s.Contains("no dup"))))
					.Return(new[] { "some other hash" });

				HashCodeProviders
					.Each(x => x
					           	.Stub(y => y.CalculateHashCode(null))
					           	.IgnoreArguments()
					           	.Return(new[] { "hash 1", "hash 2" }));

				Finder = new DuplicateFinder(FileFinders, HashCodeProviders);
			};

		Because of = () => { Duplicates = Finder.FindDuplicates().ToArray(); };

		It should_search_for_files =
			() => FileFinders.Each(x => x.AssertWasCalled(y => y.GetFiles()));

		It should_create_hash_values_for_each_file =
			() => HashCodeProviders.Each(x => x.AssertWasCalled(y => y.CalculateHashCode(null),
			                                                    o => o.IgnoreArguments().Repeat.Times(4)));

		It should_group_duplicates_by_hash_value =
			() => Duplicates.First().Count().ShouldEqual(3);

		It should_not_consider_unique_files =
			() => Duplicates.Count().ShouldEqual(1);
	}

	[Subject(typeof(DuplicateFinder))]
	public class When_duplicates_are_searched_within_in_one_path
	{
		static IDuplicateFinder Finder;
		static IFileFinder[] FileFinders;
		static IEnumerable<IEnumerable<string>> Duplicates;

		Establish context = () =>
			{
				FileFinders = new[]
				              {
				              	MockRepository.GenerateStub<IFileFinder>(),
				              	MockRepository.GenerateStub<IFileFinder>()
				              };
				FileFinders
					.Each(x => x
					           	.Stub(y => y.GetFiles())
					           	.Return(new[] { @"c:\path\file 1.txt", @"c:\path\file 2.txt" }));

				var hashCodeProviders = new[]
				                        {
				                        	MockRepository.GenerateStub<IHashCodeProvider>()
				                        };

				Finder = new DuplicateFinder(FileFinders, hashCodeProviders);
			};

		Because of = () => { Duplicates = Finder.FindDuplicates(); };

		It should_not_find_duplicates =
			() => Duplicates.ShouldBeEmpty();
	}
}