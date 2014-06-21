namespace DuplicateFinder.Core.CommandLine.Factories
{
  static class OptionSetExtensions
  {
    public static OptionSetWithArguments With(this ModifyableOptionSet instance, string[] args)
    {
      return new OptionSetWithArguments(instance, args);
    }

    internal class OptionSetWithArguments
    {
      readonly string[] _args;
      readonly ModifyableOptionSet _optionSet;

      public OptionSetWithArguments(ModifyableOptionSet optionSet, string[] args)
      {
        _optionSet = optionSet;
        _args = args;
      }

      public bool Has(string option)
      {
        var flag = false;
        _optionSet.Update<string>(option, v => flag = v != null);
        _optionSet.Parse(_args);
        return flag;
      }
    }
  }
}
