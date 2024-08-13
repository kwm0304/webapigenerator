using System.Diagnostics;
using webapigenerator.Cli;
using webapigenerator.Enums;

namespace webapigenerator.Utils;

public class InstallTools
{
  public List<ProjectTools> ExtractCommandTools(GenerateSettings settings)
  {
    var tools = new List<ProjectTools>();
    //data access
    if (settings.DataAccess!.StartsWith("EF"))
      tools.Add(ProjectTools.EntityFramework);
    else if (settings.DataAccess.Equals("Dapper", StringComparison.OrdinalIgnoreCase))
      tools.Add(ProjectTools.Dapper);
    else if (settings.DataAccess.Equals("Ado", StringComparison.OrdinalIgnoreCase))
      tools.Add(ProjectTools.Ado);
    //db
    if (settings.Database!.Equals("SQLS", StringComparison.OrdinalIgnoreCase))
      tools.Add(ProjectTools.SqlServer);
    else if (settings.Database.Equals("SQLL", StringComparison.OrdinalIgnoreCase))
      tools.Add(ProjectTools.SqlLite);
    //user + auth
    if (!string.IsNullOrEmpty(settings.User))
    {
      var userParts = settings.User.Split('.');
      if (userParts.Length > 1 && userParts[1].StartsWith("auth:"))
      {
        tools.Add(ProjectTools.AspNetCoreIdentity);
        var authType = userParts[1].Split(':')[1];
        if (authType.Equals("jwt", StringComparison.OrdinalIgnoreCase))
        {
          tools.Add(ProjectTools.Jwt);
        }
        else if (authType.Equals("cookie"))
        {
          tools.Add(ProjectTools.Cookie);
        }
      }
    }
    if (settings.EnableCaching)
    {
      tools.Add(ProjectTools.Caching);
    }
    return tools;
  }

  internal async Task<bool> InstallTool(string packageName)
  {
    try
    {
      var addPackageProcess = new Process
      {
        StartInfo = new ProcessStartInfo
        {
          FileName = "dotnet",
          Arguments = $"add package {packageName}",
          RedirectStandardOutput = true,
          RedirectStandardError = true,
          UseShellExecute = false,
          CreateNoWindow = true
        }
      };
      addPackageProcess.Start();
      string output = await addPackageProcess.StandardOutput.ReadToEndAsync();
      string error = await addPackageProcess.StandardError.ReadToEndAsync();
      addPackageProcess.WaitForExit();
      if (addPackageProcess.ExitCode != 0)
      {
        return false;
      }
      return true;
    }
    catch (Exception e)
    {
      Console.WriteLine($"Exception occurred during installment: {e.Message}");
      return false;
    }
  }

  public async Task RestoreTools()
  {
    var restoreProcess = new Process
    {
      StartInfo = new ProcessStartInfo
      {
        FileName = "dotnet",
        Arguments = "restore",
        RedirectStandardOutput = true,
        RedirectStandardError = true,
        UseShellExecute = false,
        CreateNoWindow = false
      }
    };
    restoreProcess.Start();
    string restoreError = await restoreProcess.StandardError.ReadToEndAsync();
    restoreProcess.WaitForExit();
    if (restoreProcess.ExitCode != 0)
    {
      Console.WriteLine($"Erorr during project restore: {restoreError}");
    }
    else 
    {
      Console.WriteLine("Project restored successfully");
    }
  }

  public List<string> GetPackageNames(ProjectTools tool, bool withEF)
  {
    var packages = new List<string>();
    switch (tool)
    {
      case ProjectTools.EntityFramework:
        if (withEF)
        {
          packages.Add("Microsoft.EntityFrameworkCore");
          packages.Add("Microsoft.EntityFrameworkCore.Tools");
          packages.Add("Microsoft.EntityFrameworkCore.Design");
        }
        break;
      case ProjectTools.Dapper:
        packages.Add("Dapper");
        break;
      case ProjectTools.SqlLite:
        packages.Add(withEF
            ? "Microsoft.EntityFrameworkCore.Sqlite"
            : "System.Data.SQLite");
        break;
      case ProjectTools.SqlServer:
        packages.Add(withEF
            ? "Microsoft.EntityFrameworkCore.SqlServer"
            : "System.Data.SqlClient");
        break;
      case ProjectTools.AspNetCoreIdentity:
        packages.Add("Microsoft.AspNetCore.Identity.EntityFrameworkCore");
        break;
      case ProjectTools.Jwt:
        packages.Add("System.IdentityModel.Tokens.Jwt");
        packages.Add("Microsoft.AspNetCore.Authentication.JwtBearer");
        break;
      case ProjectTools.Cookie:
        packages.Add("Microsoft.AspNetCore.Authentication.Cookies");
        break;
      case ProjectTools.Caching:
        packages.Add("Microsoft.Extensions.Caching.Memory");
        break;
      default:
        throw new ArgumentOutOfRangeException(nameof(tool), tool, null);
    }
    return packages;
  }
}