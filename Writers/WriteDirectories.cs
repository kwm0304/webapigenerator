using Microsoft.CodeAnalysis;
using webapigenerator.Builders;
using webapigenerator.Cli;
using webapigenerator.Enums;
using webapigenerator.Readers;
using webapigenerator.Templates;
using webapigenerator.Utils;

namespace webapigenerator.Writers;

public class WriteDirectories
{
    private readonly MiddlewareBuilder _middlewareBuilder;
    private readonly FileWriter _writer;


    public WriteDirectories(MiddlewareBuilder middlewareBuilder, FileWriter writer)
    {
        _middlewareBuilder = middlewareBuilder;
        _writer = writer;
    }

    public async Task CreateDirectoriesAndFiles(GenerateSettings settings, Project project, ProjectMetadata? projectMetadata, List<ProjectTools> tools, string? projectName)
    {
        var modelsPath = settings.ModelsPath;
        var dataAccess = settings.DataAccess;
        var dataLayer = settings.DataLayer;
        var includeServices = settings.IncludeServices;
        var user = settings.User;
        var enableCaching = settings.EnableCaching;
        bool withEF = dataAccess!.StartsWith("EF");
        await CreateDirectoriesAsync(includeServices, project);
        await CreateStaticFiles(projectName, includeServices, modelsPath!);
        List<PathName> newClassPaths = await CreateFilesAsync(modelsPath, includeServices);

        if (withEF)
        {
            if (string.IsNullOrEmpty(user))
            {
                await CreateDbContextFileAsync(dataAccess);
            }
            else
            {
                await CreateDbContextFileAsync(dataAccess, user);
                await AddAuthMiddlewareAsync(user!, tools); // need to create go between class for this and writer that passes in necessary lines
            }
        }
        else
        {
            await CreateDataAccessFileAsync();
        }
        if (!string.IsNullOrEmpty(user))
        {
            if (!withEF)
            {
                await CreateUserStoreAsync();
            }
        }
        if (enableCaching)
        {
            await ConfigureCachingAsync();
        }
    }

    private async Task CreateStaticFiles(string? projectName, bool withServices, string modelsDir)
    {
        if (string.IsNullOrEmpty(projectName))
            throw new ArgumentNullException(nameof(projectName));
        IEnumerable<string>? modelClasses = await GetModelClassNames(modelsDir)!;
        var tasks = new List<Task>
    {
        CreateFile("Data", "AppDbContext.cs", path => AppDbContext.CreateBaseDbContext(path, projectName, modelClasses!)),
        CreateFile("Repositories", "Repository.cs", path => BaseRepositoryTemplate.CreateBaseRepository(path, "AppDbContext", projectName)),
        CreateFile("Services", "Service.cs", path => BaseServiceTemplate.CreateBaseService(path, projectName)),
        CreateFile("Controllers", "Controller", path => BaseControllerTemplate.CreateBaseController(path, withServices ? "IServices" : "IRepository", projectName))
    };
        await Task.WhenAll(tasks);
        Task CreateFile(string folder, string fileName, Func<PathName, string> createTemplate)
        {
            var path = new PathName(projectName, folder, fileName);
            return _writer.WriteToFile(path, createTemplate(path));
        }
    }

    public async Task AddAuthMiddlewareAsync(string user, List<ProjectTools> tools)//,  List<string> existingMiddlewareServices
    {
        List<string> linesToAdd = [];

        throw new NotImplementedException();
    }

    public async Task ConfigureCachingAsync()
    {
        throw new NotImplementedException();
    }

    public async Task CreateDbContextFileAsync(string dataAccess, string? user)
    {
        throw new NotImplementedException();
    }
    public async Task CreateDbContextFileAsync(string dataAccess)
    {
        throw new NotImplementedException();
    }

    public async Task CreateDirectoriesAsync(bool includeServices, Project project)
    {
        string projectPath = project.FilePath!;

        List<string> directoriesToCheck = ["Repositories", "Data", "Controller"];
        if (includeServices)
        {
            directoriesToCheck.Add("Services");
        }
        foreach (var name in directoriesToCheck)
        {
            string fullPath = Path.Combine(Path.GetDirectoryName(projectPath)!, name);
            if (!Directory.Exists(fullPath))
            {
                Directory.CreateDirectory(fullPath);
                Console.WriteLine($"{name} created at {fullPath}");
            }
            else
            {
                Console.WriteLine($"{name} directory already exists");
            }
        }
    }

    public async Task<List<PathName>> CreateFilesAsync(string? modelsPath, bool includeServices)
    {
        List<PathName> newClasses = [];
        List<string> directoriesToCheck = ["Repositories", "Controllers"];
        if (includeServices)
        {
            directoriesToCheck.Add("Services");
        }
        IEnumerable<string>? modelClassNames = await GetModelClassNames(modelsPath!);
        string rootDir = Directory.GetParent(modelsPath!)!.FullName;
        foreach (var name in modelClassNames!)
        {
            foreach (var subDirName in directoriesToCheck)
            {
                string appended = subDirName == "Repositories" ? "Repository" : subDirName == "Controllers" ? "Controller" : "Service";
                string className = name + appended;
                var pathname = new PathName(rootDir, subDirName, $"{className}.cs");
                pathname.EnsureDirectoryExists();
                newClasses.Add(pathname);
            }
        }
        return newClasses;
    }

    public async Task<IEnumerable<string>?> GetModelClassNames(string modelDirPath)
    {
        try
        {
            string modelPathType = IOHelpers.GetPathType(modelDirPath!);
            IEnumerable<string> classNames = Enumerable.Empty<string>();
            List<string> modelClassNames = [];
            if (modelPathType == "file")
            {
                string fileContent = await File.ReadAllTextAsync(modelDirPath!);
                // foreach (var c in classNames)
                // {
                //     if (fileContent.Contains())
                // }
            }
            else if (modelPathType == "directory")
            {
                var helper = new AnalysisHelper();
                classNames = await helper.GetClassNamesFromDirectoryAsync(modelDirPath!);
            }
            return classNames;
        }
        catch (InvalidOperationException ex)
        {
            Console.WriteLine($"Error: {ex.Message}");
            return null;
        }
    }

    public async Task CreateInterfacesAsync(string? dataLayer, bool includeServices)
    {
        throw new NotImplementedException();
    }

    public async Task CreateUserStoreAsync()
    {
        throw new NotImplementedException();
    }

    internal async Task CreateDataAccessFileAsync()
    {
        throw new NotImplementedException();
    }
}