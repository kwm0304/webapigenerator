using webapigenerator.Builders;

namespace webapigenerator.Templates;

public class BaseControllerTemplate
{
  private readonly static string publicStr = "public";
  private readonly static string publicAsync = "public async";
  private readonly static string iActionResult = "Task<IActionResult>";
  private readonly static string catchBlock = "\ncatch (Exception e)\n{\nConsole.WriteLine($\"Error: {e.Message}\");\nreturn BadRequest(e.Message);\n";
  private readonly static string tryStart = "try\n{\n";
  public static ControllerBuilder CreateBaseController(PathName pathName, string dataAccess, string projectName)
  {
    string[] args = ["IService<T> service"];
    string[] annotations = ["HttpGet"];
    string dataType = dataAccess == "Service" ? "IService<T>" : "IRepository<T>";
    string dataTypeShort = dataAccess.ToLower();
    string dataTypeUnderscore = "_" + dataTypeShort;
    ControllerBuilder builder = new("Controller<T>", pathName);
    builder.AddUsing($"{projectName}.{dataAccess};");
    builder.AddField("private readonly", dataType, "_service", null);
    builder.AddConstructor(publicStr, args, $"{dataTypeUnderscore} = {dataTypeShort};");
    //get all
    builder.AddMethod(publicAsync, iActionResult, "GetAllAsync", annotations, "", 
    tryStart + $"var entities = await {dataTypeUnderscore}.GetAllAsync();\nreturn Ok(entities);\n}}" + catchBlock);
    //getbyid
    builder.AddMethod(publicAsync, iActionResult, "GetById", ["HttpGet(\"id\")"], "int id",
    tryStart + $"var entity = await {dataTypeUnderscore}.GetByIdAsync(id);\nif (entity == null)\n{{\nreturn NotFound(\"Nothing found with this id\");\n}}\nreturn Ok(entity);\n}}" + catchBlock);
    //create
    builder.AddMethod(publicAsync, iActionResult, "CreateAsync", ["HttpPost"], "[FromBody]T entity",
    "if (entity == null)\n{\nreturn BadRequest(\"Entity cannot be null\");\n}\n" + tryStart + 
    $"var created = await {dataTypeUnderscore}.CreateAsync(entity);\nreturn Ok(created);\n}}" + catchBlock);
    //update
    builder.AddMethod(publicAsync, iActionResult, "UpdateAsync", ["HttpPut"],"[FromBody]T entity",
    "if (entity == null)\n{\nreturn BadRequest(\"Entity cannot be null\");\n}\n" + tryStart +
    $"var updated = await {dataTypeUnderscore}.UpdateAsync(entity);\nif (entity == null)\n{{\nreturn BadRequest(\"Entity cannot be null\");\n}}" +
    "return Ok(entity);\n}" + catchBlock);
    //delete
    builder.AddMethod(publicAsync, iActionResult, "DeleteByIdAsync", ["HttpDelete(\"{id}\")"], "int id",
    tryStart + $"await {dataTypeUnderscore}.DeleteByIdAsync(id);\nreturn Ok(\"Deleted successfully\");\n}}" + catchBlock);
    return builder;
  }
}