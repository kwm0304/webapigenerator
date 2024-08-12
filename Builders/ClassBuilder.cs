using System.Text;

namespace webapigenerator.Builders;

public class ClassBuilder(string className, PathName? pathName) : TypeBuilderBase(className, pathName!)
{
    private readonly List<string> _properties = [];
    private readonly List<string> _fields = [];
    private readonly List<string> _methods = [];

    public void AddField(string accessibility, string type, string name, string? initialValue)
    {
      var field = $"{accessibility} {type} {name}";
      if (initialValue != null)
      {
        field += $"={initialValue};";
      }
      else 
      {
        field += ";";
      }
      _fields.Add(field);
    }

    public void AddProperty(string accessibility, string type, string name, bool hasGetter = true, bool hasSetter = true)
    {
      var property = $"{accessibility} {type} {name} {{";
      if (hasGetter)
      {
        property += "get;";
      }
      if (hasSetter)
      {
        property += "set;";
      }
      property += "}";
      _properties.Add(property);
    }

    public void AddMethod(string accessibility, string returnType, string name, string parameters = "", string methodBody = "")
    {
      var method = new StringBuilder();
      method.AppendLine($"{accessibility} {returnType} {name}({parameters})");
      method.AppendLine("{");
      method.AppendLine(methodBody);
      method.AppendLine("}");
      _methods.Add(method.ToString());
    }

    protected override void BuildType()
    {
      _builder.AppendLine($"public class {_name}");
      _builder.AppendLine("{");
      foreach (var field in _fields)
      {
        _builder.AppendLine(field);
      }
      foreach (var property in _properties)
      {
        _builder.AppendLine(property);
      }
      foreach (var method in _methods)
      {
        _builder.AppendLine(method);
      }
      _builder.AppendLine("}");
    }
}