using System.Collections.Generic;
using System.Linq;

using Machine.Specifications;

namespace DuplicateFinder.Core
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

    public static IEnumerable<T> AsEnumerable<T>(this IEnumerator<T> instance)
    {
      while (instance.MoveNext())
      {
        yield return instance.Current;
      }
    }
  }

  [Subject(typeof(DuplicateFinder.FilesByDrive))]
  public class When_the_number_of_partitions_is_equal_to_the_number_of_drives
  {
    const int RequestedPartitionCount = 2;
    static DuplicateFinder.FilesByDrive FilesByDrive;
    static IList<IEnumerator<string>> Partitions;

    Establish context = () =>
    {
      var files = new[] { @"c:\foo", @"d:\foo" };

      FilesByDrive = new DuplicateFinder.FilesByDrive(files);
    };

    Because of = () => Partitions = FilesByDrive.GetPartitions(RequestedPartitionCount);

    It should_yield_the_requested_number_of_partitions =
      () => Partitions.Count.ShouldEqual(RequestedPartitionCount);

    It should_yield_a_nonempty_enumerators_for_all_drives =
      () => Partitions.ShouldEachConformTo(p => p.Count() == 1);
  }

  [Subject(typeof(DuplicateFinder.FilesByDrive))]
  public class When_files_belong_to_one_drive
  {
    const int RequestedPartitionCount = 2;
    static DuplicateFinder.FilesByDrive FilesByDrive;
    static IList<IEnumerator<string>> Partitions;

    Establish context = () =>
    {
      var files = new[] { @"c:\foo", @"d:\foo", @"c:\bar" };

      FilesByDrive = new DuplicateFinder.FilesByDrive(files);
    };

    Because of = () => Partitions = FilesByDrive.GetPartitions(RequestedPartitionCount);

    It should_yield_the_requested_number_of_partitions =
      () => Partitions.Count.ShouldEqual(RequestedPartitionCount);

    It should_group_files_by_drive_1 =
      () => Partitions.First().Count().ShouldEqual(2);

    It should_group_files_by_drive_letter =
      () => Partitions.First().AsEnumerable().ShouldEachConformTo(x => x.StartsWith(@"c:\"));

    It should_group_files_by_drive_2 =
      () => Partitions.Last().Count().ShouldEqual(1);
  }

  [Subject(typeof(DuplicateFinder.FilesByDrive))]
  public class When_the_number_of_partitions_is_smaller_than_the_number_of_drives_with_equal_distribution
  {
    const int RequestedPartitionCount = 2;
    static DuplicateFinder.FilesByDrive FilesByDrive;
    static IList<IEnumerator<string>> Partitions;

    Establish context = () =>
    {
      var files = new[] { @"c:\foo", @"d:\foo", @"e:\foo", @"f:\foo" };

      FilesByDrive = new DuplicateFinder.FilesByDrive(files);
    };

    Because of = () => Partitions = FilesByDrive.GetPartitions(RequestedPartitionCount);

    It should_yield_the_requested_number_of_partitions =
      () => Partitions.Count.ShouldEqual(RequestedPartitionCount);

    It should_distribute_exceeding_drives_to_the_first_partition =
      () =>
      {
        var list = Partitions.First().AsEnumerable().ToList();
        list.ShouldContainOnly(@"c:\foo", @"e:\foo");
      };

    It should_distribute_exceeding_drives_to_the_last_partition =
      () =>
      {
        var list = Partitions.Last().AsEnumerable().ToList();
        list.ShouldContainOnly(@"d:\foo", @"f:\foo");
      };
  }

  [Subject(typeof(DuplicateFinder.FilesByDrive))]
  public class When_the_number_of_partitions_is_smaller_than_the_number_of_drives_with_unequal_distribution
  {
    const int RequestedPartitionCount = 2;
    static DuplicateFinder.FilesByDrive FilesByDrive;
    static IList<IEnumerator<string>> Partitions;

    Establish context = () =>
    {
      var files = new[] { @"c:\foo", @"d:\foo", @"e:\foo", @"f:\foo", @"g:\foo" };

      FilesByDrive = new DuplicateFinder.FilesByDrive(files);
    };

    Because of = () => Partitions = FilesByDrive.GetPartitions(RequestedPartitionCount);

    It should_yield_the_requested_number_of_partitions =
      () => Partitions.Count.ShouldEqual(RequestedPartitionCount);

    It should_distribute_exceeding_drives_to_the_first_partition =
      () =>
      {
        var list = Partitions.First().AsEnumerable().ToList();
        list.ShouldContainOnly(@"c:\foo", @"e:\foo", @"g:\foo");
      };

    It should_distribute_exceeding_drives_to_the_last_partition =
      () =>
      {
        var list = Partitions.Last().AsEnumerable().ToList();
        list.ShouldContainOnly(@"d:\foo", @"f:\foo");
      };
  }

  [Subject(typeof(DuplicateFinder.FilesByDrive))]
  public class When_the_number_of_partitions_is_larger_than_the_number_of_drives
  {
    const int RequestedPartitionCount = 2;
    static DuplicateFinder.FilesByDrive FilesByDrive;
    static IList<IEnumerator<string>> Partitions;

    Establish context = () =>
    {
      var files = new[] { @"c:\foo" };

      FilesByDrive = new DuplicateFinder.FilesByDrive(files);
    };

    Because of = () => Partitions = FilesByDrive.GetPartitions(RequestedPartitionCount);

    It should_yield_the_requested_number_of_partitions =
      () => Partitions.Count.ShouldEqual(RequestedPartitionCount);

    It should_yield_a_nonempty_enumerator_for_the_first_drive =
      () => Partitions.First().Count().ShouldEqual(1);

    It should_yield_empty_enumerators_for_other_partitions =
      () => Partitions.Skip(1).ShouldEachConformTo(p => p.Count() == 0);
  }
}
