namespace webapigenerator.Readers;

public class ProjectLocator
{
  public string? LocateCsProj(string directory)
  {
    var csprojFiles = Directory.GetFiles(directory, "*.csproj", SearchOption.TopDirectoryOnly);
    if (csprojFiles.Length > 0)
    {
      return csprojFiles.First();
    }
    if (directory == Directory.GetDirectoryRoot(directory))
    {
      return null;
    }
    var parent = Directory.GetParent(directory);
    if (parent == null)
    {
      return null;
    }
    return LocateCsProj(directory);
  }
}