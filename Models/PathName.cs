namespace webapigenerator;

public class PathName(string rootDirectoryName, string subDirectoryName, string fileName)
{
    public string RootDirectoryName { get; set; } = rootDirectoryName;
    public string SubDirectoryName { get; set; } = subDirectoryName;
    public string FileName { get; set; } = fileName;

    public string GetDirectoryPath()
  {
    return Path.Combine(RootDirectoryName, SubDirectoryName);
  }

  public string GetFilePath()
  {
    return Path.Combine(GetDirectoryPath(), FileName);
  }

  public void EnsureDirectoryExists()
  {
    var directoryPath = GetDirectoryPath();
    if (!Directory.Exists(directoryPath))
    {
      Directory.CreateDirectory(directoryPath);
    }
  }
}