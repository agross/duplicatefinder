using System;
using System.Linq;

namespace DuplicateFinder.Core
{
	internal static class FileSizeExtensions
	{
		const int ByteConversion = 1024;

		public static string ToFileSize(this int source)
		{
			return ToFileSize(Convert.ToInt64(source));
		}

		public static string ToFileSize(this long source)
		{
			var bytes = Convert.ToDouble(source);

			var match = new[]
			            {
			            	new
			            	{
			            		Bucket = 0D,
			            		Label = "Bytes"
			            	},
			            	new
			            	{
			            		Bucket = Math.Pow(ByteConversion, 1),
			            		Label = "KB"
			            	},
			            	new
			            	{
			            		Bucket = Math.Pow(ByteConversion, 2),
			            		Label = "MB"
			            	},
			            	new
			            	{
			            		Bucket = Math.Pow(ByteConversion, 3),
			            		Label = "GB"
			            	},
			            	new
			            	{
			            		Bucket = Math.Pow(ByteConversion, 4),
			            		Label = "TB"
			            	}
			            }
				.OrderByDescending(x => x.Bucket)
				.Where(x => bytes >= x.Bucket)
				.Select(x => new
				             {
				             	x.Label,
								Divisor = x.Bucket == 0D ? 1 : x.Bucket
				             })
				.First();

			return Build(bytes, match.Divisor, match.Label);
		}

		static string Build(double bytes, double divisor, string label)
		{
			return string.Concat(Math.Round(bytes / divisor, 2), " " + label);
		}
	}
}