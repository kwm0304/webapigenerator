using Microsoft.CodeAnalysis;
using Spectre.Console.Cli;
using webapigenerator.Enums;
using webapigenerator.Readers;
using webapigenerator.Utils;
using webapigenerator.Writers;

namespace webapigenerator.Cli;

public class GenerateCommand(WriteDirectories writer, ProjectReader reader, InstallTools installer) : AsyncCommand<GenerateSettings>
{
  private readonly WriteDirectories _writer = writer;
  private readonly ProjectReader _reader = reader;
  private readonly InstallTools _installer = installer;
  public List<ProjectTools> _tools = [];
  public bool withEF = false;

  public override async Task<int> ExecuteAsync(CommandContext context, GenerateSettings settings)
  {
    await GenerateCodeAsync(settings);
    return 1;
  }
  public async Task GenerateCodeAsync(GenerateSettings settings)
  {
    Project project = LoadProjectAndExtractTools(settings);
    await InstallRequiredPackages(project);
    await CreateDirectoriesAndFiles(settings);
    await ConfigureAdditionalSettings(settings);
  }

  private Project LoadProjectAndExtractTools(GenerateSettings settings)
  {
    Project project = _reader.LoadCodeAnalysisProject();
    _tools = _installer.ExtractCommandTools(settings);
    return project;
  }

  private async Task InstallRequiredPackages(Project project)
  {
    List<string> packageNames = await GetRequirements();
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

  private async Task CreateDirectoriesAndFiles(GenerateSettings settings)
  {
    var modelsPath = settings.ModelsPath;
    var dataAccess = settings.DataAccess;
    var dataLayer = settings.DataLayer;
    var includeServices = settings.IncludeServices;
    bool withEF = dataAccess!.StartsWith("EF");
    await _writer.CreateDirectoriesAsync(dataLayer, includeServices);
    await _writer.CreateFilesAsync(modelsPath, dataLayer, includeServices, withEF);
    if (withEF)
    {
      await _writer.CreateDbContextFileAsync(dataAccess);
    }
    else
    {
      await _writer.CreateDataAccessFileAsync();
    }
  }

  private async Task ConfigureAdditionalSettings(GenerateSettings settings)
  {
    var user = settings.User;
    var enableCaching = settings.EnableCaching;
    bool withEF = settings.DataAccess!.StartsWith("EF");
    if (!string.IsNullOrEmpty(user))
    {
      await _writer.AddAuthMiddlewareAsync(user);
      if (!withEF)
      {
        await _writer.CreateUserStoreAsync();
      }
    }
    if (enableCaching)
    {
      await _writer.ConfigureCachingAsync();
    }
  }

  public async Task<List<string>> GetRequirements()
  {
    List<string> packageNames = [];
    List<ProjectTools> toolsNeeded = [];
    List<ProjectTools> currentTools = await _reader.ReadCsProj();
    foreach (var tool in currentTools)
    {
      if (!_tools.Contains(tool))
      {
        toolsNeeded.Add(tool);
      }
    }
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