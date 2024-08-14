namespace webapigenerator.Builders;

public class InterfaceBuilder(string interfaceName, PathName? pathName, string? baseClassName) : TypeBuilderBase(interfaceName, pathName!)
{
  private readonly List<string> _properties = [];
  private readonly List<string> _methods = [];
  private readonly string? _baseClassName = baseClassName;

  public void AddProperty(string type, string name, bool hasGetter = true, bool hasSetter = true)
  {
    var property = $"{type} {name} {{";
    if (hasGetter)
    {
      property += " get;";
    }
    if (hasSetter)
    {
      property += " set;";
    }
    property += " }";
    _properties.Add(property);
  }

  public void AddMethod(string returnType, string name, string parameters = "")
  {
    var method = $"{returnType} {name}({parameters});";
    _methods.Add(method);
  }

  protected override void BuildType(bool inherits = false)
  {
    if (inherits && !string.IsNullOrEmpty(_baseClassName))
    {
      _builder.AppendLine($"public interface {_name} : {_baseClassName}");
    }
    else
    {
      _builder.AppendLine($"public interface {_name}");
    }
    _builder.AppendLine("{");
    foreach (var property in _properties)
    {
      _builder.AppendLine(property);
    }
    if (_properties.Count > 0)
    {
      _builder.AppendLine("");
    }
    foreach (var method in _methods)
    {
      _builder.AppendLine(method);
    }
    _builder.AppendLine("}");
  }
}