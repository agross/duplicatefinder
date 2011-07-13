using System.Collections.Generic;
using System.IO;
using System.Linq;

using FakeItEasy;

using Machine.Specifications;
using Machine.Specifications.Utility;

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
				              	A.Fake<IFileFinder>(),
				              	A.Fake<IFileFinder>()
				              };
				var finder1 = FileFinders[0];
				A
					.CallTo(() => finder1.GetFiles())
					.Returns(new[] { @"c:\path\dup 1.txt", @"c:\path\dup 2.txt" });

				var finder2 = FileFinders[1];
				A
					.CallTo(() => finder2.GetFiles())
					.Returns(new[] { @"c:\other path\dup 3.txt", @"c:\other path\no dup.txt" });

				HashCodeProviders = new[]
				                    {
				                    	A.Fake<IHashCodeProvider>(),
				                    	A.Fake<IHashCodeProvider>()
				                    };

				HashCodeProviders
					.Each(x => A.CallTo(() => x.CalculateHashCode(null))
					           	.WithAnyArguments()
					           	.Returns(new[] { "hash 1", "hash 2" }));

				var hash1 = HashCodeProviders.First();
				A
					.CallTo(() => hash1.CalculateHashCode(A<string>.That.Matches(x => x.Contains("no dup"))))
					.Returns(new[] { "some other hash" });

				Finder = new DuplicateFinder(FileFinders, HashCodeProviders, A.Fake<IOutput>());
			};

		Because of = () => { Duplicates = Finder.FindDuplicates().ToArray(); };

		It should_search_for_files =
			() => FileFinders.Each(x => A.CallTo(() => x.GetFiles()).MustHaveHappened());

		It should_create_hash_values_for_each_file =
			() => HashCodeProviders
				.Each(x => A
					.CallTo(() => x.CalculateHashCode(A<string>.Ignored))
					.MustHaveHappened(Repeated.Exactly.Times(4)));

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
				              	A.Fake<IFileFinder>(),
				              	A.Fake<IFileFinder>()
				              };
				FileFinders
					.Each(x => A
								.CallTo(() => x.GetFiles())
					           	.Returns(new[] { @"c:\path\file 1.txt", @"c:\path\file 2.txt" }));

				var hashCodeProviders = new[]
				                        {
				                        	A.Fake<IHashCodeProvider>()
				                        };

				Finder = new DuplicateFinder(FileFinders, hashCodeProviders, A.Fake<IOutput>());
			};

		Because of = () => { Duplicates = Finder.FindDuplicates(); };

		It should_not_find_duplicates =
			() => Duplicates.ShouldBeEmpty();
	}

	[Subject(typeof(DuplicateFinder))]
	public class When_duplicates_are_searched_and_files_cannot_be_read
	{
		static IDuplicateFinder Finder;
		static IFileFinder[] FileFinders;
		static IEnumerable<IEnumerable<string>> Duplicates;

		Establish context = () =>
			{
				FileFinders = new[]
				              {
				              	A.Fake<IFileFinder>(),
				              	A.Fake<IFileFinder>()
				              };
				FileFinders
					.Each(x => A
								.CallTo(() => x.GetFiles())
					           	.Returns(new[] { @"c:\path\file 1.txt" }));

				var throws = A.Fake<IHashCodeProvider>();
				A
					.CallTo(() => throws.CalculateHashCode(null))
					.WithAnyArguments()
					.Throws(new FileNotFoundException());

				var hashCodeProviders = new[]
				                        {
				                        	throws
				                        };

				Finder = new DuplicateFinder(FileFinders, hashCodeProviders, A.Fake<IOutput>());
			};

		Because of = () => { Duplicates = Finder.FindDuplicates(); };

		It should_succeed =
			() => true.ShouldBeTrue();
	}
}