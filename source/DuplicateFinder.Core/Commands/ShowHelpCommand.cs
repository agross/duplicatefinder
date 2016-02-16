using System;
using System.Collections.Generic;
using System.Diagnostics;

using Mono.Options;

namespace DuplicateFinder.Core.Commands
{
  class ShowHelpCommand : ICommand
  {
    readonly OptionSet _options;
    readonly IOutput _output;

    public ShowHelpCommand(OptionSet options, IOutput output, params string[] messages)
    {
      _options = options;
      _output = output;
      Messages = messages;
    }

    public IEnumerable<string> Messages { get; private set; }

    public void Execute()
    {
      _output.WriteLine("Usage: {0} [options] directory [directory]", Process.GetCurrentProcess().ProcessName);
      _output.WriteLine("Searches and deletes duplicate files." + Environment.NewLine);
      _output.WriteLine("Options:");

      _options.WriteOptionDescriptions(_output.GetTextWriter());

      _output.WriteLine(String.Empty);

      foreach (var message in Messages)
      {
        _output.WriteLine(message);
      }
    }
  }
}
