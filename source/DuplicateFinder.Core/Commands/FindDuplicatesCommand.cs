using System;
using System.Collections.Generic;
using System.Linq;

namespace DuplicateFinder.Core.Commands
{
	public class FindDuplicatesCommand : ICommand
	{
		readonly IOutput _output;
		readonly ISelectFilesToDelete _select;

		public FindDuplicatesCommand(IOutput output,
		                             IDuplicateFinder duplicateFinder,
		                             ISelectFilesToDelete deletionSelector,
		                             IFileDeleter fileDeleter)
		{
			DuplicateFinder = duplicateFinder;
			FileDeleter = fileDeleter;
			_output = output;
			_select = deletionSelector;
		}

		public IDuplicateFinder DuplicateFinder
		{
			get;
			private set;
		}

		public IEnumerable<IEnumerable<string>> Duplicates
		{
			get;
			private set;
		}

		public IFileDeleter FileDeleter
		{
			get;
			private set;
		}

		public void Execute()
		{
			Duplicates = DuplicateFinder.FindDuplicates().ToList();

			var bytesDeleted = Duplicates
				.Select(x =>
					{
						var delete = _select.FilesToDelete(x);
						var keep = x.Except(delete);

						return new { Keep = keep, Delete = delete };
					})
				.SelectMany(x =>
					{
						foreach (var keep in x.Keep)
						{
							_output.WriteLine(String.Format("Keeping {0}", keep));
						}

						return x.Delete;
					})
				.Select(FileDeleter.Delete)
				.Sum();

			_output.WriteLine(String.Format("{0} deleted.", bytesDeleted.ToFileSize()));
		}
	}
}