using System;

namespace DuplicateFinder.Core.CommandLine.TestHelpers
{
  static class StringExtensions
  {
    public static string[] Args(this string args)
    {
      return args.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
    }
  }
}