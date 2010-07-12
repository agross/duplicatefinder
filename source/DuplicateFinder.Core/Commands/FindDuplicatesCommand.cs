using System;

namespace DuplicateFinder.Core.Commands
{
	internal class FindDuplicatesCommand : ICommand
	{
		public FindDuplicatesCommand(IHashCodeProvider[] hashCodeProviders, string dir1, string dir2)
		{
			Directories = new[] { dir1, dir2 };
			HashCodeProviders = hashCodeProviders;
		}

		public IHashCodeProvider[] HashCodeProviders
		{
			get;
			private set;
		}

		public string[] Directories
		{
			get;
			private set;
		}

		public void Execute()
		{
			throw new NotImplementedException();
		}
	}
}