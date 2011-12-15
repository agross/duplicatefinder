namespace DuplicateFinder.Core.Commands
{
	public class PruneHistoryCommand
	{
		DuplicateFinder _duplicateFinder;

		public IDuplicateFinder DuplicateFinder
		{
			get
			{
				return _duplicateFinder;
			}
		}
	}
}