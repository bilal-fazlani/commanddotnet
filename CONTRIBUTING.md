# Contributing to CommandDotNet

First off, thank you for considering contributing to CommandDotNet! 🎉

It's people like you that make CommandDotNet such a great framework. We welcome contributions from everyone, whether you're fixing a bug, adding a feature, improving documentation, or just asking questions.

## Table of Contents

- [Code of Conduct](#code-of-conduct)
- [Getting Started](#getting-started)
- [Development Setup](#development-setup)
- [How Can I Contribute?](#how-can-i-contribute)
- [Development Workflow](#development-workflow)
- [Coding Standards](#coding-standards)
- [Testing Guidelines](#testing-guidelines)
- [Submitting Changes](#submitting-changes)
- [Release Process](#release-process)

## Code of Conduct

This project and everyone participating in it is governed by our commitment to providing a welcoming and inclusive environment. Please be respectful and considerate in all interactions.

## Getting Started

### Prerequisites

- **.NET SDK 8.0 or 9.0** - [Download here](https://dotnet.microsoft.com/download)
- **Git** - [Download here](https://git-scm.com/downloads)
- **IDE** (recommended):
  - [Visual Studio 2022](https://visualstudio.microsoft.com/) (Community edition is fine)
  - [JetBrains Rider](https://www.jetbrains.com/rider/)
  - [Visual Studio Code](https://code.visualstudio.com/) with C# extension

### Before You Start

1. **Check existing issues** - Someone might already be working on it
2. **Start a discussion** - For significant changes, please open a discussion first
3. **Read the docs** - Familiarize yourself with [CommandDotNet documentation](https://commanddotnet.bilal-fazlani.com)

## Development Setup

1. **Fork the repository** on GitHub

2. **Clone your fork** locally:
   ```bash
   git clone https://github.com/YOUR-USERNAME/commanddotnet.git
   cd commanddotnet
   ```

3. **Add the upstream repository**:
   ```bash
   git remote add upstream https://github.com/bilal-fazlani/commanddotnet.git
   ```

4. **Restore dependencies**:
   ```bash
   dotnet restore
   ```

5. **Build the solution**:
   ```bash
   dotnet build
   ```

6. **Run tests** to verify everything works:
   ```bash
   dotnet test
   ```

## How Can I Contribute?

### Reporting Bugs

Before creating bug reports, please check existing issues. When creating a bug report, include:

- **Clear title and description**
- **Steps to reproduce** the issue
- **Expected vs actual behavior**
- **.NET version** and OS
- **Code sample** if possible (minimal reproducible example)

Use the [bug report issue template](.github/ISSUE_TEMPLATE/bug_report.md) if available.

### Suggesting Features

Feature suggestions are welcome! Please:

1. **Start a discussion** first for significant features
2. **Check the roadmap** to see if it's already planned
3. **Describe the use case** - why is this feature needed?
4. **Provide examples** of how it would be used

### Documentation Improvements

Documentation is just as important as code! You can help by:

- Fixing typos or clarifying existing docs
- Adding examples
- Improving API documentation (XML comments)
- Writing tutorials or guides

Documentation is in the `docs/` directory and uses [MkDocs](https://www.mkdocs.org/).

### Code Contributions

We love code contributions! Here's how to get started:

1. **Find an issue** to work on or create one
2. **Comment on the issue** to let others know you're working on it
3. **Fork and create a branch** from `master`
4. **Make your changes** following our coding standards
5. **Write tests** for your changes
6. **Update documentation** if needed
7. **Submit a pull request**

## Development Workflow

### Creating a Branch

```bash
# Update your master branch
git checkout master
git pull upstream master

# Create a feature branch
git checkout -b feature/your-feature-name
# or for bug fixes
git checkout -b fix/issue-number-description
```

### Making Changes

1. **Write code** following our coding standards
2. **Add tests** - We maintain ~52% line coverage (aim to maintain or improve)
3. **Update docs** if needed
4. **Ensure zero build warnings**
5. **Run tests locally** before pushing

### Running Tests

```bash
# Run all tests
dotnet test

# Run specific test project
dotnet test CommandDotNet.Tests

# Run tests with coverage
dotnet test --collect:"XPlat Code Coverage"
```

### Building Documentation

The documentation uses MkDocs and uses [mdsnippets](https://github.com/SimonCropp/MarkdownSnippets) to embed code and reusable markdown.

```bash
# Install mdsnippets (if not already installed)
dotnet tool install -g MarkdownSnippets.Tool

# Generate documentation snippets from code examples
mdsnippets

# Build and serve documentation locally
cd docs
pip install mkdocs
mkdocs serve
```

Visit http://localhost:8000 to view the documentation locally.

**Note:** Run `mdsnippets` from the repository root to update code snippets in the docs before building.

## Coding Standards

### General Principles

- **Follow existing code style** - Consistency is key
- **Use modern C# features** - We target .NET 8.0/9.0
- **Nullable reference types** - Always enabled
- **File-scoped namespaces** - Use them
- **Primary constructors** - Preferred where appropriate


### Naming Conventions

- **PascalCase** for public members, types, namespaces
- **camelCase** for parameters, local variables
- **_camelCase** for private fields (with underscore)
- **IPascalCase** for interfaces

### XML Documentation

Public APIs must have XML documentation:

```csharp
/// <summary>
/// Parses command line arguments and executes the appropriate command.
/// </summary>
/// <param name="args">The command line arguments to parse</param>
/// <returns>The exit code returned by the command</returns>
public int Run(string[] args)
{
    // Implementation
}
```

## Testing Guidelines

### Test Coverage

- **Maintain or improve** the current coverage (~85% line coverage, ~72% branch coverage)
- **Test public APIs** thoroughly
- **Test edge cases** and error conditions
- Run tests with coverage: `dotnet test --framework net9.0 --settings coverlet.runsettings --collect:"XPlat Code Coverage"`

### Using TestTools

CommandDotNet provides excellent testing tools. Use them!

```csharp
[Fact]
public void SomeCommand_Should_Work()
{
    new AppRunner<MyApp>()
        .Verify(new Scenario
        {
            When = { Args = "mycommand --option value" },
            Then = { ExitCode = 0 }
        });
}
```

### Test Organization

- **FeatureTests/** - End-to-end feature tests
- **UnitTests/** - Focused unit tests
- **Name tests clearly** - Test name should describe what's being tested

## Submitting Changes

### Pull Request Process

1. **Update your branch** with the latest from master:
   ```bash
   git checkout master
   git pull upstream master
   git checkout your-branch
   git rebase master
   ```

2. **Push your changes**:
   ```bash
   git push origin your-branch
   ```

3. **Create a Pull Request** on GitHub

4. **Fill out the PR template** with:
   - Description of changes
   - Related issue numbers
   - Testing performed
   - Breaking changes (if any)

### PR Guidelines

- **One feature per PR** - Keep PRs focused
- **Write clear commit messages** - Describe *what* and *why*
- **Ensure CI passes** - All tests must pass
- **Respond to feedback** - Be open to suggestions
- **Keep it up to date** - Rebase on master if needed

### Commit Messages

```bash
# Good commit messages
git commit -m "Add support for clubbed options"
git commit -m "Fix #123: Parse error with piped input"
git commit -m "docs: Update getting started guide"

# Less helpful
git commit -m "Fixed stuff"
git commit -m "Updates"
```

## Release Process

Releases are managed by the maintainers. The process involves:

1. Update version numbers
2. Update release notes
3. Push a project-specific tag (e.g., `CommandDotNet.7.0.1`)
4. GitHub Actions automatically publishes to NuGet
5. Create a GitHub release

## Questions?

- **General questions** - [Start a discussion](https://github.com/bilal-fazlani/commanddotnet/discussions)
- **Bug reports** - [Create an issue](https://github.com/bilal-fazlani/commanddotnet/issues)
- **Chat** - Join us on [Discord](https://discord.gg/QFxKSeG)

## Recognition

Contributors will be:
- Listed in release notes
- Acknowledged in the README (for significant contributions)
- Appreciated by the community! 🙏

## License

By contributing, you agree that your contributions will be licensed under the MIT License.

---

**Thank you for contributing to CommandDotNet!** 🚀
