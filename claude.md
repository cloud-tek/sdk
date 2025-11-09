# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Documentation
- Use **context7** to find andy relevant documentation needed. Ensure to feed any relevant knowledge to any relevant subtasks. Use context7 at all times to do research of important documentation if you're unsure of something.

## Build and Test Commands

### Building the Solution
```bash
# Standard build
dotnet build
dotnet build -c Release

# Build with warnings as errors (CI validation)
dotnet build -warnaserror
```

### Running Tests
```bash
# Run all tests
dotnet test -c Release

# Run tests for specific framework
dotnet test -c Release --framework net8.0

# Run specific test by name
dotnet test -c Release --filter DisplayName="TestName"

# Run tests in a specific project
dotnet test path/to/project.csproj -c Release
```

### Code Quality
```bash
# Format check
dotnet format --verify-no-changes
```

### Documentation

## High-Level Architecture

### Project Structure
- **`/src/CloudTek.Sdk/`** - Contains an MSBuild SDK


### Key Architectural Concepts
- **Common standards** - Basic project properties propagated globally from the SDK to all client projects
- **Common coding style** - Analyzers propagated globally from the SDK to all client projects

### Testing Patterns
- Pass `ITestOutputHelper output` to test constructors for debugging

## Code Style and Conventions

### C# Style
- Allman style braces (opening brace on new line)
- 4 spaces indentation, no tabs
- Private fields prefixed with underscore `_fieldName`
- Use `var` when type is apparent
- Default to `sealed` classes and records
- Enable `#nullable enable` in new/modified files
- Never use `async void`, `.Result`, or `.Wait()`
- Always pass `CancellationToken` through async call chains

### API Design
- Include unit tests with all changes

### Test Naming
- Use `DisplayName` attribute for descriptive test names
- Follow pattern: `Should_ExpectedBehavior_When_Condition`

## Development Workflow

### Git Branches
- **`main`** - Main development branch (default for PRs)
- Feature branches: `feature/description`
- Bugfix branches: `bug/description`

### Making Changes
1. Always read existing code patterns in the module you're modifying
2. Follow existing conventions for that specific module
3. Add/update tests for your changes
4. Run incremental tests before committing
5. Ensure API compatibility tests pass for core changes

### Target Frameworks
- **.NET 9.0** - Primary target

## Important Files
- `Directory.Build.props` - MSBuild properties, package versions
- `global.json` - .NET SDK version (8.0.403)
- `xunit.runner.json` - Test configuration (60s timeout, no parallelization)
- `RELEASE_NOTES.md` - Version history and changelog
