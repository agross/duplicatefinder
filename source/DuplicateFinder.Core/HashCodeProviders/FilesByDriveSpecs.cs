using System.Collections.Generic;
using System.Linq;

using Machine.Specifications;

namespace DuplicateFinder.Core.HashCodeProviders
{
  static class EnumeratorExtensions
  {
    public static int Count<T>(this IEnumerator<T> instance)
    {
      var count = 0;
      while (instance.MoveNext())
      {
        count++;
      }

      return count;
    }
  }

  [Subject(typeof(DuplicateFinder.FilesByDrive))]
  public class When_the_number_of_partitions_is_equal_to_the_number_of_drives
  {
    static DuplicateFinder.FilesByDrive FilesByDrive;
    static IList<IEnumerator<string>> Partitions;

    Establish context = () =>
    {
      var files = new[] { @"c:\foo", @"d:\foo" };

      FilesByDrive = new DuplicateFinder.FilesByDrive(files);
    };

    Because of = () => { Partitions = FilesByDrive.GetPartitions(PartitionCount); };

    It should_yield_the_requested_number_of_partitions =
      () => Partitions.Count.ShouldEqual(PartitionCount);

    It should_yield_a_nonempty_enumerators_for_all_drives =
      () => Partitions.ShouldEachConformTo(p => p.Count() == 1);

    const int PartitionCount = 2;
  }
  
  [Subject(typeof(DuplicateFinder.FilesByDrive))]
  public class When_the_number_of_partitions_is_smaller_than_the_number_of_drives
  {
    static DuplicateFinder.FilesByDrive FilesByDrive;
    static IList<IEnumerator<string>> Partitions;

    Establish context = () =>
    {
      var files = new[] { @"c:\foo", @"d:\foo", @"e:\foo" };

      FilesByDrive = new DuplicateFinder.FilesByDrive(files);
    };

    Because of = () => { Partitions = FilesByDrive.GetPartitions(PartitionCount); };

    It should_yield_the_requested_number_of_partitions =
      () => Partitions.Count.ShouldEqual(PartitionCount);

    It should_yield_a_nonempty_enumerator_for_the_first_partition =
      () => Partitions.First().Count().ShouldEqual(1);
    
    It should_merge_the_last_drives_into_one_partition =
      () => Partitions.Last().Count().ShouldEqual(2);

    const int PartitionCount = 2;
  }

  [Subject(typeof(DuplicateFinder.FilesByDrive))]
  public class When_the_number_of_partitions_is_larger_than_the_number_of_drives
  {
    static DuplicateFinder.FilesByDrive FilesByDrive;
    static IList<IEnumerator<string>> Partitions;

    Establish context = () =>
    {
      var files = new[] { @"c:\foo" };

      FilesByDrive = new DuplicateFinder.FilesByDrive(files);
    };

    Because of = () => { Partitions = FilesByDrive.GetPartitions(PartitionCount); };

    It should_yield_the_requested_number_of_partitions =
      () => Partitions.Count.ShouldEqual(PartitionCount);

    It should_yield_a_nonempty_enumerator_for_the_first_drive =
      () => Partitions.First().Count().ShouldEqual(1);

    It should_yield_empty_enumerators_for_other_partitions =
      () => Partitions.Skip(1).ShouldEachConformTo(p => p.Count() == 0);

    const int PartitionCount = 2;
  }
}
