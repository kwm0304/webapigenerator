using webapigenerator.Builders;

namespace webapigenerator.Templates;

public class BaseControllerTemplate
{
  public static string publicStr = "public";
  public static string publicAsync = "public async";
  public static string taskStr = "Task";

  public InterfaceBuilder CreateBaseIService(PathName pathName)
  {
    InterfaceBuilder builder = new("IService<T> where T", pathName, "class");
    builder.AddMethod(taskStr + "<List<T>>", "GetAllAsync", "");
    builder.AddMethod(taskStr + "<T>", "GetByIdAsync", "int id");
    builder.AddMethod(taskStr + "<T>", "CreateAsync", "T entity");
    builder.AddMethod(taskStr, "UpdateAsync", "T entity");
    builder.AddMethod(taskStr, "DeleteByIdAsync", "int id");
    return builder;
  }
  public ClassBuilder CreateBaseService(PathName pathName)
  {
    string[] args = ["IRepository<T> repository"];
    ClassBuilder builder = new("Service<T>", pathName, "IService<T> where T : class");
    builder.AddField("private readonly", "IRepository<T>", "_repository", null);
    builder.AddConstructor(publicStr, args, "_repository = repository;");
    builder.AddMethod(publicAsync, $"{taskStr}<T>", "CreateAsync", "T entity", "await _repository.CreateAsync(entity);\nreturn entity;");
    builder.AddMethod(publicAsync, taskStr, "DeleteByIdAsync", "int id",
    "await _repository.DeleteByIdAsync(id);");
    builder.AddMethod(publicAsync, taskStr + "<List<T>>", "GetAllAsync", "return await _repository.GetAllAsync();");
    builder.AddMethod(publicAsync, taskStr + "<T>", "GetByIdAsync", "int id", "return await _repository.GetByIdAsync(id);");
    builder.AddMethod(publicAsync, taskStr, "UpdateAsync", "T entity", "await _repository.UpdateAsync(entity);");
    return builder;
  }
}
