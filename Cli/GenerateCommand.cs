using Microsoft.CodeAnalysis;
using Spectre.Console.Cli;
using webapigenerator.Enums;
using webapigenerator.Readers;
using webapigenerator.Utils;
using webapigenerator.Writers;

namespace webapigenerator.Cli;

public class GenerateCommand(WriteDirectories writer, ProjectReader reader, InstallTools installer, ProjectLocator locator) : AsyncCommand<GenerateSettings>
{
  private readonly WriteDirectories _writer = writer;
  private readonly ProjectReader _reader = reader;
  private readonly InstallTools _installer = installer;
  private readonly ProjectLocator _locator = locator;
  private ProjectMetadata? _projectMetadata;
  public List<ProjectTools> _tools = [];
  public bool withEF = false;

  public override async Task<int> ExecuteAsync(CommandContext context, GenerateSettings settings)
  {
    await GenerateCodeAsync(settings);
    return 1;
  }
  public async Task GenerateCodeAsync(GenerateSettings settings)
  {
    string modelDir = settings.ModelsPath!;
    Project project = LoadProjectAndExtractTools(settings);
    _projectMetadata = new(project);
    await InstallRequiredPackages(project, modelDir);
    await _writer.CreateDirectoriesAndFiles(settings, project, _projectMetadata, _tools);
  }

  private Project LoadProjectAndExtractTools(GenerateSettings settings)
  {
    Project project = _reader.LoadCodeAnalysisProject();
    _tools = _installer.ExtractCommandTools(settings);
    return project;
  }

  private async Task InstallRequiredPackages(Project project, string modelDir)
  {
    List<string> packageNames = await GetRequirements(modelDir);
    List<string> currentPackageNames = await _reader.GetPackageReferencesAsync(project);
    if (packageNames != null)
    {
      foreach (var name in packageNames)
      {
        if (!currentPackageNames.Contains(name))
        {
          await _installer.InstallTool(name);
        }
      }
      await _installer.RestoreTools();
    }
  }

  

  

  public async Task<List<string>> GetRequirements(string modelDir)
  {
    List<string> packageNames = [];
    List<ProjectTools> toolsNeeded = [];
    string csProjFile = _locator.LocateCsProj(modelDir)!;
    List<string> currentTools = _projectMetadata!.GetNugetPackages(csProjFile);
    // foreach (var tool in currentTools)
    // {
    //   if (!_tools.Contains(tool))
    //   {
    //     toolsNeeded.Add(tool);
    //   }
    // }
    if (toolsNeeded != null)
    {
      foreach (var tool in toolsNeeded)
      {
        List<string> packageName = _installer.GetPackageNames(tool, withEF);
        packageNames.AddRange(packageName);
      }
    }
    return packageNames;
  }
}