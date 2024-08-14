namespace webapigenerator.Utils;

public class IOHelpers
{
  public static bool IsFile(string path)
  {
    return File.Exists(path);
  }
  public static bool IsDirectory(string path)
  {
    return File.Exists(path);
  }
  public static string GetPathType(string path)
  {
    if (IsFile(path))
    {
      return "file";
    }
    else if (IsDirectory(path))
    {
      return "directory";
    }
    else
    {
      throw new InvalidOperationException("Path is nethier file or directory");
    }
  }
}
