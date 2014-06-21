using Machine.Specifications;

namespace DuplicateFinder.Core.CommandLine
{
  [Subject(typeof(ModifyableOptionSet))]
  class When_an_option_is_updated
  {
    static ModifyableOptionSet Options;

    Establish context = () => { Options = new ModifyableOptionSet { { "foo", v => { } }, { "bar", v => { } } }; };

    Because of = () => Options.Update<string>("foo", v => { });

    It should_put_the_updated_option_at_the_same_index =
      () => Options[0].Prototype.ShouldEqual("foo");
  }
}
