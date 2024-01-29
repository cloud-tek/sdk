namespace CloudTek.Sdk.Tests;

public class ProjectValidationTests
{
    [Theory, MemberData(nameof(ProjectData.Data), MemberType = typeof(ProjectData))]
    public async Task ValidateProject(string name)
    {
        // Arrange
        var project = new Project(name);
        var msgPath = project.GetFilepath("message.txt");

        // Act
        await project.RunDotNetToolRestore();
        var result = await project.RunDotNetBuild()!;

        // Assert
        var output = result.stdOut + result.stdErr;
        if (Path.Exists(msgPath))
        {
          var msg = await File.ReadAllTextAsync(msgPath);
          output
            .Contains(msg, StringComparison.InvariantCultureIgnoreCase)
            .Should()
            .BeTrue();
        }

        if (name.EndsWith("Error"))
        {
            result.exitCode
              .Should()
              .NotBe(0, because: "error projects should fail, but this passed with output: {0}", result.stdOut);
        }
        else
        {
          result.exitCode
            .Should()
            .Be(0, because: "valid projects should build, but this failed with output: {0}", output);
        }
    }
}
