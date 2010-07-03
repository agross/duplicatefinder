using System;
using System.Collections;
using System.IO;
using System.Security.Cryptography;

namespace DeleteDuplicateFiles
{
	internal class Program
	{
		[STAThread]
		static void Main(string[] args)
		{
			Program p = new Program();

			DirectoryInfo di;
			if (args.Length == 0)
			{
				di = new DirectoryInfo(Directory.GetCurrentDirectory());
			}
			else
			{
				di = new DirectoryInfo(args[0]);
			}

			Hashtable duplicates = p.HashFiles(di);

			ulong bytesDeleted = 0;
			ulong filesDeleted = p.DeleteFiles(duplicates, ref bytesDeleted);
			p.PurgeEmptyDirs(di);

			Console.WriteLine("{0} of {1} duplicate(s) deleted [{2} Byte(s)]", filesDeleted, duplicates.Count, bytesDeleted);
		}

		Hashtable HashFiles(DirectoryInfo baseDirectory)
		{
			Hashtable result = new Hashtable();
			Hashtable fileHashes = new Hashtable();

			SHA1 sha = new SHA1CryptoServiceProvider();

			HashDirectory(baseDirectory, ref sha, ref fileHashes, ref result);

			return result;
		}

		void HashDirectory(DirectoryInfo baseDirectory, ref SHA1 sha, ref Hashtable fileHashes, ref Hashtable duplicates)
		{
			// Recurse through subdirectories hashing each file.
			foreach (DirectoryInfo di in baseDirectory.GetDirectories())
			{
				HashDirectory(di, ref sha, ref fileHashes, ref duplicates);
			}

			Console.WriteLine("Processing {0}", baseDirectory.FullName);

			FileInfo[] files = baseDirectory.GetFiles();
			foreach (FileInfo fi in files)
			{
				Console.WriteLine("Hashing {0}", fi.FullName);

				byte[] hash = null;

				try
				{
					using (FileStream fs = fi.OpenRead())
					{
						hash = sha.ComputeHash(fs);
					}

					fileHashes.Add(Convert.ToBase64String(hash), fi);
				}
				catch (UnauthorizedAccessException ex)
				{
					Console.WriteLine("Error hashing the file: {0}", ex.Message);
				}
				catch (ArgumentException)
				{
					Console.WriteLine("Duplicate: {0}", fi.FullName);
					duplicates.Add(fi, Convert.ToBase64String(hash));
				}
			}
		}

		ulong DeleteFiles(Hashtable duplicates, ref ulong bytesDeleted)
		{
			ulong result = 0;
			ulong thisFileSize = 0;
			bytesDeleted = 0;

			foreach (DictionaryEntry de in duplicates)
			{
				try
				{
					Console.WriteLine("Duplicate File: {0} (Hash: {1})", ((FileInfo) de.Key).FullName, de.Value);

					thisFileSize = (ulong) ((FileInfo) de.Key).Length;
					File.SetAttributes(((FileInfo) de.Key).FullName, FileAttributes.Normal);
					File.Delete(((FileInfo) de.Key).FullName);

					result++;
					bytesDeleted += thisFileSize;
				}
				catch (Exception ex)
				{
					Console.WriteLine("Error deleting the file: {0}", ex.Message);
				}
			}

			return result;
		}

		void PurgeEmptyDirs(DirectoryInfo baseDirectory)
		{
			// Recurse through subdirectories hashing each file.
			foreach (DirectoryInfo di in baseDirectory.GetDirectories())
			{
				PurgeEmptyDirs(di);

				if (di.GetFiles().Length == 0 && di.GetDirectories().Length == 0)
				{
					try
					{
						Console.WriteLine("Deleting empty directory {0}", di.FullName);
						di.Delete(false);
					}
					catch (Exception ex)
					{
						Console.WriteLine("Error deleting {0}:\n{1}", di.FullName, ex);
					}
				}
			}
		}
	}
}