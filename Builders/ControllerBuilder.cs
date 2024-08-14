using System.Text;

namespace webapigenerator.Builders;

public class ControllerBuilder : TypeBuilderBase
{
  private readonly List<string> _fields = new();
  private readonly List<string> _methods = new();
  private readonly List<string> _constructors = new();
  private readonly string? _baseClassName;

  public ControllerBuilder(string className, PathName? pathName, string? baseClassName = "ControllerBase")
      : base(className, pathName)
  {
    _baseClassName = baseClassName;
  }

  public void AddField(string accessibility, string type, string name, string? initialValue)
  {
    var field = $"{accessibility} {type} {name}";
    if (initialValue != null)
    {
      field += $" = {initialValue};";
    }
    else
    {
      field += ";";
    }
    _fields.Add(field);
  }

  public void AddConstructor(string accessibility, string[] parameters, string constructorBody)
  {
    var constructor = new StringBuilder();
    var parametersList = string.Join(", ", parameters);
    constructor.AppendLine($"{accessibility} {_name}({parametersList})");
    constructor.AppendLine("{");
    constructor.AppendLine(constructorBody);
    constructor.AppendLine("}");
    _constructors.Add(constructor.ToString());
  }

  public void AddMethod(string accessibility, string returnType, string name, string[] annotations, string parameters = "", string methodBody = "")
  {
    var method = new StringBuilder();
    foreach (var annotation in annotations)
    {
      method.AppendLine($"[{annotation}]");
    }
    method.AppendLine($"{accessibility} {returnType} {name}({parameters})");
    method.AppendLine("{");
    method.AppendLine(methodBody);
    method.AppendLine("}");
    _methods.Add(method.ToString());
  }

  protected override void BuildType(bool inherits = true)
  {
    _builder.AppendLine("[ApiController]");
    _builder.AppendLine("[Route(\"api/[controller]\")]");
    if (inherits && !string.IsNullOrEmpty(_baseClassName))
    {
      _builder.AppendLine($"public class {_name} : {_baseClassName}");
    }
    else
    {
      _builder.AppendLine($"public class {_name}");
    }
    _builder.AppendLine("{");
    foreach (var field in _fields)
    {
      _builder.AppendLine(field);
    }
    foreach (var constructor in _constructors)
    {
      _builder.AppendLine(constructor);
    }

    foreach (var method in _methods)
    {
      _builder.AppendLine(method);
    }
    _builder.AppendLine("}");
  }
}
