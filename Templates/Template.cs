namespace webapigenerator.Templates;

public class Template
{
  public PathName Path { get; set; }
  public string Code { get; set; }
  public Template() {}
  public Template(PathName path, string code)
  {
    Path = path;
    Code = code;
  }
}
