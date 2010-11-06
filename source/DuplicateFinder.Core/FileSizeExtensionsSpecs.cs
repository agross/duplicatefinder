using Machine.Specifications;

namespace DuplicateFinder.Core
{
	[Subject(typeof(FileSizeExtensions))]
	public class When_zero_bytes_are_converted_to_file_size
	{
		static string Size;

		Because of = () => { Size = 0.ToFileSize(); };

		It should_print_bytes =
			() => Size.ShouldEqual("0 Bytes");
	}
	
	[Subject(typeof(FileSizeExtensions))]
	public class When_bytes_are_converted_to_file_size
	{
		static string Size;

		Because of = () => { Size = 42.ToFileSize(); };

		It should_print_bytes =
			() => Size.ShouldEqual("42 Bytes");
	}
	
	[Subject(typeof(FileSizeExtensions))]
	public class When_kilobytes_are_converted_to_file_size
	{
		static string Size;

		Because of = () => { Size = 420000.ToFileSize(); };

		It should_print_rounded_kilobytes =
			() => Size.ShouldEqual("410,16 KB");
	}
	
	[Subject(typeof(FileSizeExtensions))]
	public class When_megabytes_are_converted_to_file_size
	{
		static string Size;

		Because of = () => { Size = 420000000.ToFileSize(); };

		It should_print_rounded_megabytes = 
			() => Size.ShouldEqual("400,54 MB");
	}
	
	[Subject(typeof(FileSizeExtensions))]
	public class When_gigabytes_are_converted_to_file_size
	{
		static string Size;

		Because of = () => { Size = 420000000000.ToFileSize(); };

		It should_print_rounded_gigabytes
			= () => Size.ShouldEqual("391,16 GB");
	}
	
	[Subject(typeof(FileSizeExtensions))]
	public class When_terabytes_are_converted_to_file_size
	{
		static string Size;

		Because of = () => { Size = 420000000000000.ToFileSize(); };

		It should_print_rounded_terabytes
			= () => Size.ShouldEqual("381,99 TB");
	}
}