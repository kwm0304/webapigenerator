using System.ComponentModel;
using Spectre.Console.Cli;

namespace webapigenerator.Cli;

public class GenerateSettings : CommandSettings
{
  [Description("Path to models directory or file")]
  [CommandArgument(0, "<modelsPath>")]
  public string? ModelsPath { get; set; }

  [CommandOption("-da|--data-access")]
  public string? DataAccess { get; set; }

  [CommandOption("-db|--database")]
  public string? Database { get; set; }

  [CommandOption("-s|--services")]
  public bool IncludeServices { get; set; }

  [CommandOption("-u|--user")]
  public string? User { get; set; }

  [CommandOption("-c|--caching")]
  public bool EnableCaching { get; set; }
}