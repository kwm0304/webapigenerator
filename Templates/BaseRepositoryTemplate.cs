using webapigenerator.Builders;

namespace webapigenerator.Templates;

public class BaseRepositoryTemplate
{
    public InterfaceBuilder CreateBaseIRepository(PathName pathName)
    {
      InterfaceBuilder builder = new("IRepository<T> where T", pathName, "class");
      builder.AddMethod("Task<List<T>>", "GetAllAsync","");
      builder.AddMethod("Task<T>", "GetByIdAsync", "int? id");
      builder.AddMethod("Task<T>", "CreateAsync", "T entity");
      builder.AddMethod("Task", "UpdateAsync", "T entity");
      builder.AddMethod("Task", "DeleteByIdAsync", "int id");
      return builder;
    }
    public ClassBuilder CreateBaseRepository(PathName pathName, string dbContextName)
    {
      string[] args = [$"{dbContextName} context"];
      ClassBuilder builder = new("Repository", pathName, "IRepository<T> where T : class");
      builder.AddField("private readonly", dbContextName, "_context", null);
      builder.AddField("private readonly", "DbSet<T>", "_dbSet", null);
      builder.AddConstructor("public", args, "_context = context;");
      builder.AddMethod("public", "async Task<T>", "CreateAsync", "T entity", "await _dbSet.AddAsync(entity);\nawait _context.SaveChangesAsync();\nreturn entity");
      builder.AddMethod("public", "async Task<List<T>>", "GetAllAsync", "", "return await _dbSet.ToListAsync();");
      builder.AddMethod("public", "async Task<T?>", "GetByIdAsync", "int id", "if (id is null)\n{\nreturn null;\n}\nreturn await _dbSet.FindAsync(id);");
      builder.AddMethod("public", "async Task", "UpdateAsync", "T entity", "_context.Entry(entity).State = EntityState.Modified;\nawait _context.SaveChangesAsync();");
      builder.AddMethod("public", "async Task", "DeleteByIdAsync", "int id", 
      "var entity = await _dbSet.FindAsync(id);\nif (entity != null)\n{\n_dbSet.Remove(entity);\nawait _context.SaveChangesAsync();\n");
      return builder;
    }
}
