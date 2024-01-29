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
      return Directory.GetParent(GetCallerFilePath())!.FullName;

      static string GetCallerFilePath([CallerFilePath] string? path = null) => path ?? "";
    });

  private static string ThisFileDirectory => ThisFileDirectoryLazy.Value;
  internal static string ProjectsDirectory { get; } = Path.Combine(ThisFileDirectory, "projects");
}
