using System;
using System.Diagnostics;
using System.Linq;

using DuplicateFinder.Core.HashCodeHistory;

namespace DuplicateFinder.Core.Commands
{
	class PruneHistoryCommand : ICommand
	{
		readonly IOutput _output;

		public PruneHistoryCommand(IOutput output,
		                           IDuplicateFinder duplicateFinder,
		                           IRememberHashCodes history)
		{
			DuplicateFinder = duplicateFinder;
			_output = output;
			History = history;
		}

		public IDuplicateFinder DuplicateFinder
		{
			get;
			private set;
		}

		public IRememberHashCodes History
		{
			get;
			private set;
		}

		public void Execute()
		{
      var timer = new Stopwatch();
      timer.Start();

      try
      {
			  var hashesAndFiles = DuplicateFinder.CalculateHashes().ToList();

			  History.Forget(hashesAndFiles.Select(x => x.Key), DuplicateFinder.HashCodeProviders);
      }
      finally
      {
        timer.Stop();
        _output.WriteLine(String.Format("Took {0}", timer.Elapsed));
      }
		}
	}
}