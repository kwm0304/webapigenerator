using Microsoft.Build.Evaluation;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace webapigenerator.Readers;

public class ProjectMetadata
{
  private readonly IEnumerable<SyntaxTree> _syntaxTrees;
  public ProjectMetadata(Microsoft.CodeAnalysis.Project project)
  {
    _syntaxTrees = GetSyntaxTreesFromProject(project).GetAwaiter().GetResult();
  }
  private async Task<IEnumerable<SyntaxTree>> GetSyntaxTreesFromProject(Microsoft.CodeAnalysis.Project project)
  {
    var syntaxTrees = new List<SyntaxTree>();
    foreach (var document in project.Documents)
    {
      var syntaxTree = await document.GetSyntaxTreeAsync();
      if (syntaxTree != null)
      {
        syntaxTrees.Add(syntaxTree);
      }
    }
    return syntaxTrees;
  }

  public IEnumerable<string> GetAllClassNames()
  {
    var classNames = new List<string>();
    foreach (var tree in _syntaxTrees)
    {
      var root = tree.GetCompilationUnitRoot();
      var classes = root.DescendantNodes().OfType<ClassDeclarationSyntax>();
      foreach (var c in classes)
      {
        classNames.Add(c.Identifier.Text);
      }
    }
    return classNames;
  }

  public IEnumerable<string> GetAllNamespaces()
  {
    var namespaces = new HashSet<string>();
    foreach (var tree in _syntaxTrees)
    {
      var root = tree.GetCompilationUnitRoot();
      var namespaceDeclarations = root.DescendantNodes().OfType<NamespaceDeclarationSyntax>();
      foreach (var n in namespaceDeclarations)
      {
        namespaces.Add(n.Name.ToString());
      }
    }
    return namespaces;
  }

  public List<string> GetNugetPackages(string projectPath)
  {
    var references = new List<string>();
    var project = new Microsoft.Build.Evaluation.Project(projectPath);
    var packageRefs = project.GetItems("PackageReference");
    foreach (var reference in packageRefs)
    {
      references.Add(reference.EvaluatedInclude);
    }
    return references;
  }
}
