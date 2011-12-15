using System;
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
			var hashesAndFiles = DuplicateFinder.CalculateHashes().ToList();

			History.Forget(hashesAndFiles.Select(x => x.Key));

			_output.WriteLine(String.Format("Forgot {0} hashes for {1} files.",
			                                hashesAndFiles.Count(),
			                                hashesAndFiles.Sum(x => x.Count())));
		}
	}
}