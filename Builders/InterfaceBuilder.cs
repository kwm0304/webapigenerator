namespace webapigenerator.Builders;

public class InterfaceBuilder(string interfaceName, PathName? pathName) : TypeBuilderBase(interfaceName, pathName!)
{
  private readonly List<string> _properties = [];
  private readonly List<string> _methods = [];

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
    var method = $"{returnType} {name}({parameters})";
    _methods.Add(method);
  }

  protected override void BuildType()
  {
    _builder.AppendLine($"public interface {_name}");
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