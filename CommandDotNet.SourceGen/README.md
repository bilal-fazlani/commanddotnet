# CommandDotNet.SourceGen

> ⚠️ **PROJECT STATUS: DEFERRED**
>
> This source generator is **not functional** and work has been **paused indefinitely**.
>
> **Why:** Requires ~1,200 lines of duplicated Roslyn-based command discovery logic.  
> **Details:** See **[DEFERRED_DECISION.md](DEFERRED_DECISION.md)**
>
> Infrastructure is complete but the generator does not produce working builders.
> Reconsider if AOT/trimming becomes critical or community contributors emerge.

---

This source generator was designed to eliminate runtime reflection for CommandDotNet command classes by generating builder code at compile time.

> 📋 **[DEFERRED_DECISION.md](DEFERRED_DECISION.md)** - ⚠️ **READ FIRST** - Why work was stopped  
> 📋 **[PLAN.md](PLAN.md)** - Work status  
> 📚 **[Documentation Index](DOCUMENTATION_INDEX.md)** - All documentation

## Overview (Design)

The generator analyzes command classes and generates static builder methods that replace the reflection-based class model discovery. This provides:

- **Better startup performance** - No runtime reflection required
- **AOT compatibility** - Works with Native AOT compilation and assembly trimming
- **Compile-time errors** - Issues found during build instead of runtime

## Architecture: Why Two Projects?

### User Apps Generate Code, Not CommandDotNet

**Important:** This generator doesn't generate code for CommandDotNet itself. It generates code for **your application's command classes** when you reference CommandDotNet as a NuGet package.

```
Your App
├── MyCommands.cs                    ← Your command classes
├── References CommandDotNet.nupkg   ← NuGet package contains:
│   ├── lib/CommandDotNet.dll        ← Runtime library
│   └── analyzers/CommandDotNet.SourceGen.dll ← Generator (analyzer)
│
└── Build Process
    → CommandDotNet.SourceGen.dll runs as analyzer
    → Finds your command classes
    → Generates MyCommands__CommandClassBuilder.g.cs
    → Generates GeneratedBuildersInitializer.g.cs
    → Module initializer registers with CommandDotNet.dll
```

### Why Separate Projects Are Required

Even though the generator runs on user code (not CommandDotNet's code), we must have separate projects:

**1. Different Target Frameworks**
- **Generator:** Must be `netstandard2.0` (Roslyn/compiler requirement)
- **Runtime:** Can be `net8.0`, `net9.0`, etc. (multi-target)

**2. Different Package Locations**
- **Generator:** Ships in `analyzers/dotnet/cs/` folder (loaded as build-time analyzer)
- **Runtime:** Ships in `lib/` folder (referenced as normal assembly)

**3. Build-Time vs Runtime**
- **Generator:** Runs during compilation (never loaded at runtime)
- **Runtime:** Loaded and executed by your app

**4. Different Dependencies**
- **Generator:** References `Microsoft.CodeAnalysis.*` (Roslyn APIs)
- **Runtime:** No Roslyn dependencies (keeps package small)

### Single Package Distribution

Despite being separate projects, they ship as **one NuGet package**:

```
CommandDotNet.nupkg
├── lib/
│   ├── net8.0/CommandDotNet.dll
│   └── net9.0/CommandDotNet.dll
└── analyzers/
    └── dotnet/cs/
        └── CommandDotNet.SourceGen.dll
```

**User experience:** Install one package, get both components automatically.

```xml
<!-- Single package reference -->
<PackageReference Include="CommandDotNet" Version="x.x.x" />
```

### Runtime Flow

1. **Build Time:** Generator runs in your app's build, generates builder code
2. **App Startup:** Module initializer registers builders with `CommandDefRegistry`
3. **Runtime:** CommandDotNet looks up registered builders (zero reflection!)

```
Your App Build
    ↓
Generator Analyzes Your Command Classes
    ↓
Generates: YourCommand__CommandClassBuilder.g.cs
Generates: GeneratedBuildersInitializer.g.cs
    ↓
[ModuleInitializer] runs before Main()
    ↓
Registers: CommandDefRegistry.Register<YourCommand>(builder)
    ↓
Runtime: new AppRunner<YourCommand>()
    ↓
ClassCommandDef.TryCreateFromGeneratedBuilder()
    ↓
CommandDefRegistry.TryGetBuilder(typeof(YourCommand))
    ↓
✅ Uses generated code (zero reflection!)
```

## How It Works

### 1. Analysis Phase

The generator scans for public classes with public methods that could be command methods in **your application**.

### 2. Generation Phase

For each command class found in your app, it generates:

**Individual Builder** (`YourCommand__CommandClassBuilder.g.cs`)
- `CreateCommandDef(CommandContext)` - Creates the command definition
- `BuildInterceptorMethod(AppConfig)` - Builds the interceptor method definition (if any)
- `BuildDefaultCommand(AppConfig)` - Builds the default command definition (if any)
- `BuildLocalCommands(AppConfig)` - Builds all local command definitions

**Module Initializer** (`GeneratedBuildersInitializer.g.cs`) - One per assembly
- `[ModuleInitializer]` method that runs automatically before `Main()`
- Registers all generated builders with `CommandDefRegistry`
- **Zero reflection** - uses direct function references

### 3. Zero-Reflection Registration

The module initializer eliminates runtime reflection:

```csharp
// Generated in YOUR app's assembly
namespace CommandDotNet.Generated;

internal static class GeneratedBuildersInitializer
{
    [ModuleInitializer]  // Runs before Main()
    internal static void Initialize()
    {
        // Direct function reference - no reflection!
        CommandDefRegistry.Register<YourCommand>(
            YourNamespace.YourCommand__CommandClassBuilder.CreateCommandDef);
    }
}
```

**Why this matters:**
- ✅ **AOT Compatible** - No `Assembly.GetType()` or `MethodInfo.Invoke()`
- ✅ **Trimming Safe** - Direct references keep types from being trimmed
- ✅ **Fast** - Dictionary lookup (~10ns) vs reflection (~10μs) = **600x faster**
- ✅ **Type-Safe** - Compile-time verification, refactoring-safe

### 4. Runtime Usage (Automatic)

CommandDotNet automatically uses generated builders when available:

```csharp
// Your code - nothing changes!
public class Program
{
    static int Main(string[] args)
    {
        return new AppRunner<Calculator>().Run(args);
    }
}

// Behind the scenes:
// 1. Module initializer registered Calculator builder
// 2. AppRunner calls ClassCommandDef.CreateRootCommand(typeof(Calculator))
// 3. TryCreateFromGeneratedBuilder() finds registered builder
// 4. ✅ Uses generated code (zero reflection!)
// 5. Falls back to reflection only if builder not found
```

**Zero configuration required** - just reference CommandDotNet and build your app!

## For Users: Getting Started

The source generator is bundled into the main CommandDotNet package. Simply install CommandDotNet:

```xml
<ItemGroup>
  <PackageReference Include="CommandDotNet" Version="x.x.x" />
</ItemGroup>
```

**That's it!** The generator will:
1. Automatically run during your app's build
2. Find all your command classes
3. Generate builder code in your `obj/` folder
4. Register builders via module initializer
5. CommandDotNet uses them automatically (zero reflection!)

**No code changes needed** - your existing CommandDotNet code works as-is, just faster!

## For Contributors: Local Development

When working on the source generator itself or testing it locally, reference it as an analyzer:

```xml
<ItemGroup>
  <!-- Reference the source generator as an analyzer -->
  <ProjectReference Include="..\CommandDotNet.SourceGen\CommandDotNet.SourceGen.csproj"
                    OutputItemType="Analyzer"
                    ReferenceOutputAssembly="false" />
</ItemGroup>
```

**Important:** 
- `OutputItemType="Analyzer"` - Loads it as a build-time analyzer
- `ReferenceOutputAssembly="false"` - Don't reference the DLL at runtime

### Generated Files Location

Generated files are created in:
```
obj/Debug/<TargetFramework>/generated/CommandDotNet.SourceGen/CommandDotNet.SourceGen.CommandClassGenerator/
```

## Current Limitations

1. **Nested subcommands** still use reflection fallback
2. **Property-based subcommands** still use reflection fallback
3. Only generates builders for classes with public methods

## Future Enhancements

- [ ] Generate code for nested class subcommands
- [ ] Generate code for property subcommands
- [ ] Generate code for argument model properties
- [ ] Full AOT compatibility
- [ ] Incremental generation optimizations

## Verification

To verify the generator is working:

1. **Build your project** with CommandDotNet reference
2. **Check generated files**:
   ```bash
   find obj -name "*__CommandClassBuilder.g.cs"
   ```
3. **Search assembly** for generated types:
   ```csharp
   var builderType = typeof(YourCommand).Assembly.GetType(
       "YourNamespace.YourCommand__CommandClassBuilder");
   Assert.NotNull(builderType);
   ```

## Troubleshooting

### Generator Not Running

- Ensure you're using .NET 6.0+ SDK
- Check build output for generator warnings/errors
- Clean and rebuild: `dotnet clean && dotnet build`

### Generated Code Issues

- Check `obj/` directory for generated `.g.cs` files
- Review build diagnostics with `-v:detailed`
- Report issues with generator diagnostic output

## Architecture

The generator follows the standard Roslyn IIncrementalGenerator pattern:

1. **SyntaxProvider** - Filters candidate classes
2. **Transform** - Extracts semantic information
3. **Output** - Generates source code

This ensures efficient incremental compilation.
