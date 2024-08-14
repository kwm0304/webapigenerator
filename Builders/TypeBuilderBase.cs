using System.Text;

namespace webapigenerator.Builders;

public abstract class TypeBuilderBase(string name, PathName? pathName)
{
  protected StringBuilder _builder = new StringBuilder();
  protected string _name = name;
  protected PathName _pathName = pathName!;
  protected List<string> _usings = [];

    public void AddUsing(string usingDirective)
  {
    _usings.Add(usingDirective);
  }

  public void Clear()
  {
    _builder.Clear();
  }

  public override string ToString()
  {
    _builder.Clear();
    foreach (var usingDirective in _usings)
    {
      _builder.AppendLine($"using {usingDirective}");
    }
    if (_usings.Count > 0)
    {
      _builder.AppendLine("");
    }
    if (_pathName != null && !string.IsNullOrEmpty(_pathName.SubDirectoryName))
    {
      _builder.AppendLine($"namespace {_pathName.SubDirectoryName}");
      _builder.AppendLine("{");
    }
    BuildType(inherits: true);
    if (_pathName != null && !string.IsNullOrEmpty(_pathName.SubDirectoryName))
    {
      _builder.AppendLine("}");
    }
    return _builder.ToString();
  }

  protected abstract void BuildType(bool inherits);
}