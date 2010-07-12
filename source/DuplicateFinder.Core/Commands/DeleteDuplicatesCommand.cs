using System;

namespace DuplicateFinder.Core.Commands
{
	internal class DeleteDuplicatesCommand : ICommand
	{
		public DeleteDuplicatesCommand(IHashCodeProvider[] hashCodeProviders, string dir1, string dir2, string keepFilesIn, bool whatIf)
		{
			Scan = new[] { dir1, dir2 };
			HashCodeProviders = hashCodeProviders;
			KeepFilesIn = keepFilesIn;
			WhatIf = whatIf;
		}

		public string[] Scan
		{
			get;
			private set;
		}

		public IHashCodeProvider[] HashCodeProviders
		{
			get;
			private set;
		}

		public string KeepFilesIn
		{
			get;
			private set;
		}

		public bool WhatIf
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