namespace DuplicateFinder.Core
{
  public interface IFileDeleter
  {
    long Delete(string path);
  }
}
