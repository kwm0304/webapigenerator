using webapigenerator.Builders;

namespace webapigenerator.Templates;

public class AppDbContext
{
  private static readonly string publicStr = "public";
  public static string CreateBaseDbContext(PathName pathName, string projectName, IEnumerable<string> entities)
  {
    ClassBuilder builder = new("AppDbContext", pathName, "DbContext");
    builder.AddUsing("Microsoft.EntityFrameworkCore");
    builder.AddUsing($"{projectName}.Models");
    foreach (var entity in entities)
    {
      builder.AddProperty(publicStr, $"DbSet<{entity}>", $"{entity}s", "get; set;");
    }
    string[] constructorArgs = ["DbContextOptions<AppDbContext> options"];
    builder.AddConstructor(publicStr, constructorArgs, ": base(options) { }");
    string onModelCreatingBody = "// Configure your model here\nbase.OnModelCreating(modelBuilder);";
    builder.AddMethod("protected override", "void", "OnModelCreating", "ModelBuilder modelBuilder", onModelCreatingBody);
    return builder.ToString();
  }
}