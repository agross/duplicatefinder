using System;
using System.Linq;

using Mono.Options;

namespace DuplicateFinder.Core.CommandLine
{
	class ModifyableOptionSet : OptionSet
	{
		public void Update<T>(string key, Action<T> parser)
		{
			int originalIndex;
			var option = Delete(key, out originalIndex);

			Add(option.Prototype, option.Description, parser);

			int iDontCare;
			option = Delete(key, out iDontCare);

			InsertItem(originalIndex, option);
		}

		Option Delete(string key, out int index)
		{
			var option = Items.First(x => x.Prototype.Equals(key));
			index = IndexOf(option);
			RemoveItem(index);
			return option;
		}
	}
}