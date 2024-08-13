using Microsoft.CodeAnalysis;
using webapigenerator.Cli;
using webapigenerator.Enums;

namespace webapigenerator.Readers;

public class ProjectReader
{
    public static Project LoadCodeAnalysisProject(
      string projectFilePath,
      IEnumerable<string> files)
      {
        var workspace = new AdhocWorkspace();
        var project = workspace.AddProject(Path.GetFileName(projectFilePath), "C#");
        var projectWithFiles = project.WithAllSourceFiles(files);
        project = projectWithFiles ?? project;
        return project;
      }

    internal async Task<List<string>> ExtractTools(GenerateSettings settings)
    {
      //will normalize the names
        throw new NotImplementedException();
    }

    internal async Task<List<ProjectTools>> ReadCsProj()
    {
      // will parse cs proj for the string after include foreach project reference line
        throw new NotImplementedException();
    }
}
