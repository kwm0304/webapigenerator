using Microsoft.CodeAnalysis;

namespace webapigenerator.Readers;

public class ProjectExtensions
{
    public static Project WithAllSourceFiles(this Project project, IEnumerable<string> files)
    {
      foreach (string file in files)
      {
        project = project.AddDocument(file, File.ReadAllText(file)).Project;
      }
      return project;
    }

    public static Document GetUpdatedDocument(this Project project, IFileSystem fileSystem, ModelType type)
    {
      
    }
}
