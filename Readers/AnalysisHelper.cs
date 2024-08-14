using Microsoft.CodeAnalysis.MSBuild;

namespace webapigenerator.Readers;

public class AnalysisHelper
{
  public async Task<IEnumerable<string>> GetClassNamesFromDirectoryAsync(string modelDirPath)
  {
    var projectLocator = new ProjectLocator();
    string? csprojPath = projectLocator.LocateCsProj(modelDirPath)
      ?? throw new FileNotFoundException("No .csproj file found in the specified directory.");
    var workspace = MSBuildWorkspace.Create();
    var project = await workspace.OpenProjectAsync(csprojPath);
    var projectMetadata = new ProjectMetadata(project);
    return projectMetadata.GetAllClassNames();
  }
}