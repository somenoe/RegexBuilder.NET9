# DocFX Initialization Setup

## Overview

This document describes the process of initializing and setting up DocFX for the RegexBuilder.NET9 project to generate API documentation and serve it locally.

## Prerequisites

- .NET SDK 9.x or later installed
- DocFX tool installed globally (`dotnet tool install -g docfx`)
- Git repository initialized

## Installation

### Step 1: Install DocFX as a Global Tool

```powershell
dotnet tool install -g docfx
```

**Output:**

```bash
You can invoke the tool using the following command: docfx
Tool 'docfx' is already installed.
```

### Step 2: Initialize DocFX Project

Navigate to the source directory (`src`) and initialize a new DocFX project:

```powershell
cd d:\pj\RegexBuilder.NET9\src
docfx init -y
```

**What this does:**

- Creates a `docfx.json` configuration file
- Sets up the default folder structure for documentation
- Initializes metadata settings for API documentation extraction

### Step 3: Build and Serve Documentation

To build the documentation and serve it locally on `http://localhost:8080`:

```powershell
cd d:\pj\RegexBuilder.NET9\src
docfx --serve
```

**Output:**

- Processes all .csproj files in the project
- Extracts XML comments from source code
- Generates HTML documentation site
- Serves the documentation on `http://localhost:8080`

## Generated Structure

After running `docfx init -y`, the following files and directories are created:

### Main Configuration File

- **`docfx.json`**: Main configuration file containing metadata and build settings

### Documentation Directories

- **`_site/`**: Output directory containing the generated documentation website
- **`docs/`**: Markdown documentation files
  - `toc.yml`: Table of contents for documentation
  - `introduction.md`: Introduction page
  - `getting-started.md`: Getting started guide
- **`api/`**: Generated API reference (auto-generated YAML files)

### Home Page

- **`index.md`**: Landing page for the documentation site

### Table of Contents

- **`toc.yml`**: Main table of contents for the documentation site

## Configuration Details

### docfx.json Structure

```json
{
  "$schema": "https://raw.githubusercontent.com/dotnet/docfx/main/schemas/docfx.schema.json",
  "metadata": [
    {
      "src": [
        {
          "src": "../src",
          "files": ["**/*.csproj"]
        }
      ],
      "dest": "api"
    }
  ],
  "build": {
    "content": [
      {
        "files": ["**/*.{md,yml}"],
        "exclude": ["_site/**"]
      }
    ],
    "resource": [
      {
        "files": ["images/**"]
      }
    ],
    "output": "_site",
    "template": ["default", "modern"],
    "globalMetadata": {
      "_appName": "",
      "_appTitle": "",
      "_enableSearch": true,
      "pdf": true
    }
  }
}
```

**Key sections:**

- **metadata**: Specifies C# projects to extract documentation from
- **build**: Defines content, resources, and output settings
- **template**: Uses modern template for styling
- **globalMetadata**: Enables search and PDF generation

## Generated Files

After building, the `api/` directory contains:

- **Namespace files**: `RegexBuilder.yml`, `RegexBuilder.Tests.yml`
- **Class files**: `RegexBuilder.CommonPatterns.yml`, etc.
- **Table of contents**: `api/toc.yml`

These YAML files are auto-generated from the C# source code and XML comments.

## Testing the Documentation Locally

### Step 1: Start the server

```powershell
cd d:\pj\RegexBuilder.NET9\src
docfx --serve
```

### Step 2: Open browser

Navigate to `http://localhost:8080` to view the documentation site.

### Step 3: Stop the server

Press `Ctrl+C` in the terminal to shut down the server.

## Common Issues and Solutions

### Issue: FileNotFoundException for docfx.json

**Cause:** Running `docfx` from the wrong directory

**Solution:** Ensure you are in the `src` directory where `docfx.json` is located

```powershell
cd d:\pj\RegexBuilder.NET9\src
docfx --serve
```

### Issue: XML comments not appearing in documentation

**Cause:** Missing or improperly formatted XML comments in C# code

**Solution:** Ensure C# classes and methods have proper XML documentation comments:

```csharp
/// <summary>
/// Description of the method
/// </summary>
public void MyMethod() { }
```

## Best Practices

1. **Keep XML comments up-to-date**: Ensure all public classes, methods, and properties have XML documentation comments.

2. **Organize documentation**: Use the `docs/` folder for conceptual documentation, guides, and tutorials.

3. **Update toc.yml**: Keep the table of contents files updated when adding new documentation.

4. **Test locally before publishing**: Always test the documentation locally on `http://localhost:8080` before committing.

5. **Version control**: Commit the `docfx.json` file and markdown files, but add `_site/` and `api/` to `.gitignore` (auto-generated).

## Next Steps

After initializing DocFX:

1. Customize `index.md` with project information
2. Add guides and tutorials to the `docs/` folder
3. Ensure C# source code has proper XML comments
4. Test the documentation locally
5. Set up GitHub Pages deployment (see GitHub Pages plan document)

## References

- [DocFX Official Documentation](https://dotnet.github.io/docfx/)
- [DocFX Getting Started Guide](https://dotnet.github.io/docfx/tutorial/docfx_getting_started.html)
- [DocFX Configuration Reference](https://dotnet.github.io/docfx/docs/config.html)
