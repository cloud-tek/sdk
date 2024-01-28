using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace CloudTek.Sdk.Tests;

public class SampleProjectTests
{
    [Theory, MemberData(nameof(ProjectData.Data), MemberType = typeof(ProjectData))]
    public async Task ValidateProject(string projectName)
    {
        var project = new Project(projectName);
        if (Path.Exists(project.GetFilepath(".config/dotnet-tools.json")))
        {
            var toolRestore = RunDotnetInProject(project, "tool", "restore");
            await toolRestore!.WaitForExitAsync();
            toolRestore.ExitCode.Should().Be(0);
        }

        var process = RunDotnetInProject(project, "build", "--nologo", "--no-incremental");
        await process!.WaitForExitAsync();

        var msgPath = project.GetFilepath("message.txt");
        var stdout = process.StandardOutput.ReadToEnd();
        var stderr = process.StandardError.ReadToEnd();
        // msbuild doesn't seem to print errors to stderr, at least when we call it like above
        var fulloutput = stderr + stdout;
        if (Path.Exists(msgPath))
        {
            var msg = File.ReadAllText(msgPath);
            fulloutput.Should().Contain(msg);
        }

        if (projectName.EndsWith("Error"))
        {
            process.ExitCode.Should()
                .NotBe(0, because: "error projects should fail, but this passed with output: {0}", stdout);
        }
        else
        {
            process.ExitCode.Should()
                .Be(0, because: "valid projects should build, but this failed with output: {0}", stdout + stderr);
        }
    }

    private static Process? RunDotnetInProject(Project project, params string[] args)
    {
        var startInfo = new ProcessStartInfo
        {
            UseShellExecute = false,
            CreateNoWindow = true,
            WindowStyle = ProcessWindowStyle.Hidden,
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            WorkingDirectory = project.Directory,
            FileName = OperatingSystem.IsWindows() ? "dotnet.exe" : "dotnet",
        };
        foreach (var arg in args)
        {
            startInfo.ArgumentList.Add(arg);
        }
        // MSBuild variables like MSBuildExtensionsPath, MSBuildSDKsPath cause issues
        // with correct SDK resolution based on global.json in sample projects.
        // Also remove any locally set CloudTekDotnetSdk* variables
        static bool IsFilteredEnvVar(string value) =>
            value.StartsWith("MSBuild", StringComparison.OrdinalIgnoreCase) ||
            value.StartsWith("CloudTekSdk", StringComparison.OrdinalIgnoreCase);

        foreach (var key in startInfo.Environment.Keys.Where(IsFilteredEnvVar).ToArray())
        {
            startInfo.Environment.Remove(key);
        }
        // simulate CI builds, that changes some switches in Sdk
        startInfo.Environment["CI"] = "true";
        return Process.Start(startInfo);
    }

    private record Project(string Name)
    {
        public string Directory { get; } = Path.Combine(ProjectData.ProjectsDirectory, Name);

        public string GetFilepath(string filename) => Path.Combine(Directory, filename);
    }
}

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
