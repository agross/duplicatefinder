using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.AccessControl;
using System.Security.Principal;

namespace DuplicateFinder.Core.Abstractions
{
  class FileSystem : IFileSystem
  {
    public IEnumerable<string> AllFilesWithin(string path)
    {
      return ReadableFiles(path);
    }

    public Stream CreateStreamFrom(string path)
    {
      return new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
    }

    public long GetSize(string path)
    {
      return new FileInfo(path).Length;
    }

    public void Delete(string path)
    {
      File.SetAttributes(path, FileAttributes.Normal);
      File.Delete(path);
    }

    public IEnumerable<string> ReadAllLines(string path)
    {
      if (File.Exists(path))
      {
        return File.ReadAllLines(path);
      }

      return Enumerable.Empty<string>();
    }

    public void WriteAllLines(string path, IEnumerable<string> lines)
    {
      File.WriteAllLines(path, lines);
    }

    public bool Exists(string path)
    {
      return File.Exists(path);
    }

    static IEnumerable<string> ReadableFiles(string path)
    {
      if (!CanRead(path))
      {
        Console.WriteLine("Cannot read {0}", path);
        yield break;
      }

      var subdirs = Directory.EnumerateDirectories(path, "*.*", SearchOption.TopDirectoryOnly);
      foreach (var child in subdirs.SelectMany(ReadableFiles))
      {
        yield return child;
      }

      foreach (var file in Directory.EnumerateFiles(path, "*.*", SearchOption.TopDirectoryOnly))
      {
        yield return file;
      }
    }

    static bool CanRead(string path)
    {
      DirectorySecurity acl;

      try
      {
        acl = Directory.GetAccessControl(path);
      }
      catch (UnauthorizedAccessException)
      {
        return false;
      }

      var rules = acl.GetAccessRules(true, true, typeof(SecurityIdentifier));
      var readRelatedRules = rules
        .Cast<FileSystemAccessRule>()
        .Where(rule => (FileSystemRights.Read & rule.FileSystemRights) == FileSystemRights.Read);

      var allowed = false;
      var denied = false;
      foreach (var rule in readRelatedRules)
      {
        switch (rule.AccessControlType)
        {
          case AccessControlType.Allow:
            allowed = true;
            break;
          case AccessControlType.Deny:
            denied = true;
            break;
        }
      }

      return allowed && !denied;
    }
  }
}
