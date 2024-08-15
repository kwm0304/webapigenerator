using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Formatting;
using webapigenerator.Templates;

namespace webapigenerator;

public class FileWriter
{
  public async Task WriteToFile(Template template)
  {
    template.Path.EnsureDirectoryExists();
    var formatted = FormatCode(template.Code);
    await File.WriteAllTextAsync(template.Path.GetFilePath(), formatted);
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
