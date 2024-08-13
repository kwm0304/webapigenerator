

using System.Text;

namespace webapigenerator.Cli;

public class ArgumentEscaper
{
  public static string EscapeAndConcatenateArgArrayForProcessStart(IEnumerable<string> args)
  {
    return string.Join(" ", EscapeArgArray(args));
  }

  private static IEnumerable<string> EscapeArgArray(IEnumerable<string> args)
  {
    var escapedArgs = new List<string>();
    foreach (var arg in args)
    {
      escapedArgs.Add(EscapeSingleArg(arg));
    }
    return escapedArgs;
  }

  private static string EscapeSingleArg(string arg)
  {
    var sb = new StringBuilder();
    var needsQuotes = ShouldSurroundWithQuotes(arg);
    var isQuoted = needsQuotes || IsSurroundedWithQuotes(arg);
    if (needsQuotes) sb.Append("\"");
    for (int i = 0; i < arg.Length; i++)
    {
      var backslashCount = 0;
      while (i < arg.Length && arg[i] == '\\')
      {
        backslashCount++;
        i++;
      }
      if (i == arg.Length && isQuoted)
      {
        sb.Append('\\', 2 * backslashCount);
      }
      else if (i == arg.Length)
      {
        sb.Append('\\', backslashCount);
      }
      else if (arg[i] == '"')
      {
        sb.Append('\\', (2 * backslashCount) + 1);
      }
      else
      {
        sb.Append('\\', backslashCount);
        sb.Append(arg[i]);
      }
    }
    if (needsQuotes) sb.Append("\"");
    return sb.ToString();
  }

  internal static bool ShouldSurroundWithQuotes(string argument)
  {
    // Don't quote already quoted strings
    if (IsSurroundedWithQuotes(argument))
    {
      return false;
    }

    // Only quote if whitespace exists in the string
    return ArgumentContainsWhitespace(argument);
  }

  internal static bool IsSurroundedWithQuotes(string argument)
  {
    return argument.StartsWith("\"", StringComparison.Ordinal) &&
           argument.EndsWith("\"", StringComparison.Ordinal);
  }

  internal static bool ArgumentContainsWhitespace(string argument)
  {
    return argument.Contains(" ") || argument.Contains("\t") || argument.Contains("\n");
  }
}