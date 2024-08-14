using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;
using webapigenerator.Cli;
using webapigenerator.Enums;

namespace webapigenerator.Readers;

public class ProjectReader
{
  public Project LoadCodeAnalysisProject()
  {
    string directoryPath = GetCurrentDirectory();
    var files = GetFilesFromDirectory(directoryPath);
    var project = InitializeProject();
    project = AddDocumentsToProject(project, files);
    return project;
  }

  private Project InitializeProject()
  {
    string? projectFilePath = GetCurrentDirectory();
    var workspace = new AdhocWorkspace();
    var projectName = Path.GetFileNameWithoutExtension(projectFilePath);
    var projectId = ProjectId.CreateNewId();
    var versionStamp = VersionStamp.Create();
    var projectInfo = ProjectInfo.Create(projectId, versionStamp, projectName!, projectName!, LanguageNames.CSharp);

    return workspace.AddProject(projectInfo);
  }
  public static IEnumerable<string> GetFilesFromDirectory(string directoryPath)
    {
        return Directory.GetFiles(directoryPath, "*.cs", SearchOption.AllDirectories);
    }

  private Project AddDocumentsToProject(Project project, IEnumerable<string> files)
  {
    foreach (var file in files)
    {
      var sourceText = File.ReadAllText(file);
      var documentId = DocumentId.CreateNewId(project.Id);
      var documentInfo = DocumentInfo.Create(
          documentId,
          Path.GetFileName(file),
          loader: TextLoader.From(TextAndVersion.Create(SourceText.From(sourceText), VersionStamp.Create())),
          filePath: file);
      project = project.AddDocument(documentInfo.Name, SourceText.From(sourceText), filePath: file).Project;
    }
    return project;
  }

  private string GetCurrentDirectory()
  {
    return Directory.GetCurrentDirectory();
  }

  public async Task<List<string>> GetPackageReferencesAsync(Project project)
  {
    var packages = new List<string>();
    var compilation = await project.GetCompilationAsync();

    if (compilation != null)
    {
      foreach (var reference in compilation.References)
      {
        if (reference is PortableExecutableReference peReference)
        {
          var id = peReference.Properties.Aliases.FirstOrDefault()
          ?? peReference.FilePath;
          if (!string.IsNullOrEmpty(id))
          {
            packages.Add(id);
          }
        }
      }
    }
    return packages;
  }
}