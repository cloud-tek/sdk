# CloudTek .NET SDK

## About

The CloudTek .NET SDK provides common project settings, such as build properties, coding styles, analyzers configuration etc.
The SDK is versioned and published on nuget It can be imported into dotnet projects.
The SDK is root dependency and does not contain any other dependencies.

## Using the SDK

The SDK is meant to be easily importable - only a few initial config lines are required to bring its benefits. The SDK also makes it possible to override any of its default settings.

## Importing

The sections below list the changes required in order to import the SDK.

Additional information can be found in the docs - [Reference a project SDK](https://docs.microsoft.com/en-us/visualstudio/msbuild/how-to-use-project-sdk?view=vs-2022#reference-a-project-sdk).

### global.json

It's necessary to include `CloudTek.Sdk` with the desired version in `global.json` in the repository root:

```json
{
    "sdk": {
      "version": "8.0.100",
      "rollForward": "latestFeature"
    },
    "msbuild-sdks": {
        "CloudTek.DotnetSdk": "1.0.0"
    }
}

```
### Directory.Build.props

The `Directory.Build.props` file should be updated in order to actually import the SDK:

```xml
<Project>
    <Import Project="Sdk.props" Sdk="CloudTek.DotnetSdk" />
    <PropertyGroup>
        <TargetFramework>$(NetCoreVersions)</TargetFramework>
    </PropertyGroup>

    <!-- other project-specific properties -->
</Project>
```

### Directory.Build.targets

The `Directory.Build.targets` file should be updated in order to actually import the SDK:

```xml
<Project>
    <Import Project="Sdk.targets" Sdk="CloudTek.DotnetSdk"/>

    <!-- other project-specific properties -->
</Project>
```

### Analyzers

The SDK imports and configures several external analyzers - StyleCop, Meziantou and AsyncFixer.

The existing analyzer package reference sections should be removed the currently selected package manager (nuget/paket).
## Overriding

The imported properties can be locally overridden on a per repo or per project basis.

## Project settings

In order to override or disable some of the imported components, the behavior changing properties can be added into `Directory.Build.props` or `.csproj` files.

* `Directory.Build.props` - repo wide:
```xml
<Project>
  <Import Project="Sdk.props" Sdk="CloudTek.DotnetSdk" />
  <PropertyGroup>
    <TargetFramework>$(NetCoreVersions)</TargetFramework>
  </PropertyGroup>

  <PropertyGroup>
    <!-- The line below disables nullable reference types -->
    <nullable>disable</nullable>
        <!-- The line below disables the StyleCop config -->
    <CloudTekSdkEnableStyleCop>false</CloudTekSdkEnableStyleCop>
  </PropertyGroup>
  <!-- other repo-specific properties -->
</Project>
```

* `*.csproj` - project settings:
```xml
<Project Sdk="Microsoft.CloudTek.DotNet">
  <PropertyGroup>
    <OutputType>Exe</OutputType>
    <GenerateDocumentationFile>true<GenerateDocumentationFile>
  </PropertyGroup>

  <!-- other project-specific properties -->
</Project>
```

## Editor config

The SDK contains a global analyzer config file containing the default configuration for various analyzers and formatters.

Its entries can be also overridden by entries from `.editorconfig`. Such editor config files are merged with the imported global config file.

More about the analyzer and editor config files can be found in [the docs](https://docs.microsoft.com/en-us/dotnet/fundamentals/code-analysis/configuration-files).

## IDEs support

### Rider

Make sure you have enabled:
- Preferences -> Editor -> Code Style -> Enable StyleCop support (Ruleset files)
- Preferences -> Editor -> Code Style -> Enable EditorConfig support
- Preferences -> Editor -> Inspection Settings -> Read settings from editorconfig, project settings and rule sets
- Preferences -> Editor -> Inspection Settings -> Roslyn -> Enable Roslyn analyzers and Source Generators
- Preferences -> Editor -> Inspection Settings -> Roslyn -> Include Roslyn analyzers in Solution-Wide Analysis

> **(!) Important (!)**
>
> Only some analyzers's warnings can be addressed by auto-format or code cleanup.
>
> Some of the warnings are not covered by Rider, and its settings need to be adjusted.
>
> For that, in editor.globalconfig is `# ReSharper properties` section with some already defined settings which align with analyzers. If you find some inconsistency and you find appropriate settings in Rider, which will fix it - please, contribute! :)

## License
Copyright 2024 CloudTek

Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License. You may obtain a copy of the License at

```http://www.apache.org/licenses/LICENSE-2.0```

Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied. See the License for the specific language governing permissions and limitations under the License.
