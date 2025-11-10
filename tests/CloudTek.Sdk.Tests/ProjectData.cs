using System.Runtime.CompilerServices;

namespace CloudTek.Sdk.Tests;

public class ProjectData
{
  public static IEnumerable<object[]> Data =>
    Directory.EnumerateDirectories(ProjectsDirectory)
      .Select(Path.GetFileName)!
      .Where(x => Directory.EnumerateFiles(Path.Combine(ProjectsDirectory, x!), "*.*proj").Any())!
      .Select(x => new object[] { x! })
      .ToList();

  private static Lazy<string> ThisFileDirectoryLazy { get; } =
    new(() =>
    {
      static string GetCallerFilePath([CallerFilePath] string? path = null) => path ?? "";

      var result = Directory.GetParent(GetCallerFilePath())!.FullName;
      return result;
    });

  private static string ThisFileDirectory => ThisFileDirectoryLazy.Value;
  internal static string ProjectsDirectory { get; } = Path.GetFullPath(Path.Join(ThisFileDirectory, "..", "projects"));
}
