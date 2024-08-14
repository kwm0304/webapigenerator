using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Formatting;

namespace webapigenerator;

public class FileWriter
{
  public async Task WriteToFile(PathName pathName, string code)
  {
    pathName.EnsureDirectoryExists();
    var formatted = FormatCode(code);
    await File.WriteAllTextAsync(pathName.GetFilePath(), formatted);
  }

  private string FormatCode(string code)
  {
    var syntaxTree = CSharpSyntaxTree.ParseText(code);
    var root = syntaxTree.GetRoot().NormalizeWhitespace();
    using var workspace = new AdhocWorkspace();
    var options = workspace.Options;
    options = options.WithChangedOption(FormattingOptions.NewLine, LanguageNames.CSharp, "\n");
    options = options.WithChangedOption(FormattingOptions.UseTabs, LanguageNames.CSharp, false);
    options = options.WithChangedOption(FormattingOptions.TabSize, LanguageNames.CSharp, 4);
    options = options.WithChangedOption(FormattingOptions.IndentationSize, LanguageNames.CSharp, 4);
    var formattedRoot = Formatter.Format(root, workspace, options);
    return formattedRoot.ToFullString();
  }
}
