using System.Diagnostics;

namespace CloudTek.Sdk.Tests;

internal record Project(string Name)
{
  public string Directory { get; } = Path.Combine(ProjectData.ProjectsDirectory, Name);

  public string GetFilepath(string filename) => Path.Combine(Directory, filename);

  public async Task<(int exitCode, string stdOut, string stdErr)> ExecuteDotNetCLI(params string[] args)
  {
    var process = StartDotNetProcess(args);

    await process.WaitForExitAsync().ConfigureAwait(false);

    return (process.ExitCode, process.StandardOutput.ReadToEnd(), process.StandardError.ReadToEnd());
  }

  public async Task<(int exitCode, string stdOut, string stdErr)>? RunDotNetBuild()
  {

      var result = await ExecuteDotNetCLI("build","--nologo", "--no-incremental")
        .ConfigureAwait(false);

      return result;
  }

  public async Task<(int exitCode, string stdOut, string stdErr)> RunDotNetToolRestore()
  {
    if (Path.Exists(GetFilepath(".config/dotnet-tools.json")))
    {
      var result = await ExecuteDotNetCLI("tool", "restore")
        .ConfigureAwait(false);

      result.exitCode.Should().Be(0);

      return result;
    }

    return (-1, null!, null!);
  }

  private Process StartDotNetProcess(params string[] args)
  {
    var startInfo = new ProcessStartInfo
    {
      UseShellExecute = false,
      CreateNoWindow = true,
      WindowStyle = ProcessWindowStyle.Hidden,
      RedirectStandardOutput = true,
      RedirectStandardError = true,
      WorkingDirectory = Directory,
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
    return Process.Start(startInfo)!;
  }
}
