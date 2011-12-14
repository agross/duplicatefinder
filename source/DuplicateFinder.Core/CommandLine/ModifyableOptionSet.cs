using System;
using System.Linq;

using Mono.Options;

namespace DuplicateFinder.Core.CommandLine
{
	class ModifyableOptionSet : OptionSet
	{
		public void Update<T>(string key, Action<T> parser)
		{
			var option = Items.First(x => x.Prototype.Equals(key));
			var index = IndexOf(option);
			RemoveItem(index);

			Add(option.Prototype, option.Description, parser);
		}
	}
}