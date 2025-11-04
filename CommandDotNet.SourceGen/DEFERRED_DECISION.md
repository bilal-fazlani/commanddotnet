# Source Generator - Deferred Decision

**Date:** 2025-11-04  
**Status:** Work paused, deferred indefinitely  
**Reason:** Maintenance burden outweighs benefits for single-maintainer project

---

## What Was Completed

### Phase 1: Basic Infrastructure ✅

1. **Generator project created** (`CommandDotNet.SourceGen`)
   - Targets `netstandard2.0` (Roslyn requirement)
   - References `Microsoft.CodeAnalysis.CSharp`
   - Configured to package as analyzer

2. **Core generator implemented** (`CommandClassGenerator.cs`)
   - `IIncrementalGenerator` implementation
   - Syntax filtering for command classes
   - Individual builder generation logic
   - Module initializer generation

3. **Runtime integration** (`CommandDotNet` project)
   - `CommandDefRegistry` - zero-reflection registry
   - `GeneratedClassCommandDef` - uses generated builders
   - `ClassCommandDef.TryCreateFromGeneratedBuilder()` - automatic fallback
   - Module initializer pattern

4. **Test infrastructure** (`CommandDotNet.Tests`)
   - Test command classes created
   - Generator output tests
   - Comparison tests (generated vs reflection)

5. **Documentation**
   - README with architecture explanation
   - Testing strategy
   - Reflection inventory (23 call sites)
   - Phase planning documents

### Bugs Fixed During Development

- ✅ Duplicate hint names for nested classes
- ✅ Builder class name conflicts
- ✅ Namespace conflicts (added `global::` qualifiers)
- ✅ Public accessibility checks for nested classes
- ✅ Project reference configuration for local testing

### Build Status

- ✅ Solution builds successfully
- ✅ No source generator errors
- ❌ Generator generates module initializer but no individual builders
- ❌ Tests failing (no builders to test)

---

## Why Work Was Stopped

### The Discovery Problem

**Core Issue:** Source generators run at compile time and cannot execute code. They must use Roslyn APIs (`ITypeSymbol`, `IMethodSymbol`) instead of reflection APIs (`Type`, `MethodInfo`).

**Current Implementation:** Generator scans ALL classes in the project, treating any class with public methods as a command class. This generates builders for 50+ test classes unnecessarily.

**Correct Approach:** Generator should:
1. Find root command types (e.g., from `AppRunner<T>` usage or attributes)
2. Recursively discover subcommands using Roslyn equivalents of:
   - `ClassCommandDef.GetAllCommandClassTypes()`
   - `ClassCommandDef.GetNestedSubCommandTypes()`
3. Generate builders only for discovered command types

**This requires duplicating CommandDotNet's command discovery logic using Roslyn APIs.**

### Scope of Required Duplication

To complete all phases, the following logic must be duplicated with Roslyn equivalents:

#### Phase 1 - Class Discovery (~200-300 lines)
- `ClassCommandDef.GetAllCommandClassTypes()` - recursive command tree traversal
- `ClassCommandDef.GetNestedSubCommandTypes()` - find property/nested subcommands
- `ClassCommandDef.ParseMethods()` - identify interceptor/default/local commands
- Attribute detection (`[Subcommand]`, `[DefaultCommand]`)
- Public accessibility checks

#### Phase 2 - Argument Discovery (~400-500 lines)
- Method parameter analysis
- `[Operand]` and `[Option]` attribute parsing
- `IArgumentModel` property discovery
- Default value handling
- Validation attribute parsing
- Arity rules
- Custom attribute containers

#### Phase 3 - Type Descriptors (~300-400 lines)
- Custom type parsing
- Enumeration handling
- Collection type detection
- Type conversion logic
- Nullable reference type analysis

#### Phase 4 - Full Generation (~300-400 lines additional)
- Parameter binding logic
- Validation integration
- Middleware hooks
- All runtime behaviors

**Total Duplication: ~1,200-1,600 lines of parallel Roslyn-based logic**

### Maintenance Burden

Every change to command discovery/modeling requires:

1. ✅ Update runtime code (reflection-based)
2. ✅ Update generator code (Roslyn-based)
3. ✅ Keep implementations synchronized
4. ✅ Test both paths independently
5. ✅ Document both implementations
6. ✅ Handle edge cases in both

**For a single-maintainer library, this represents unsustainable technical debt.**

### Additional Concerns

1. **Middleware complexity** - CommandDotNet's middleware can dynamically modify the command tree. The generator cannot replicate this behavior at compile time.

2. **Configuration variance** - Different `AppSettings` configurations affect command discovery. The generator would need to handle all possible configurations or generate conservative builders.

3. **Test scenarios** - Test projects often use varied configurations per test. Supporting this requires complex attribute-based configuration or multiple generator entry points.

4. **Breaking changes** - Any change to command modeling logic becomes a breaking change for both runtime AND generated code.

---

## What's Left in Codebase

### Files to Keep

- ✅ `CommandDotNet.SourceGen/` project - infrastructure complete
- ✅ `CommandDefRegistry.cs` - useful pattern even without generator
- ✅ `GeneratedClassCommandDef.cs` - ready for future use
- ✅ All documentation - reflects design decisions
- ✅ `REFLECTION_INVENTORY.md` - valuable reference

### Known Issues (Unfixed)

1. **Generator produces no individual builders**
   - Module initializer generated with 50+ registrations
   - Individual `*__CommandClassBuilder.g.cs` files not generated
   - Likely silent failure in `GenerateClassBuilder()` method

2. **Generator too broad**
   - Generates for all classes with public methods
   - Includes test classes (`*Tests`)
   - Needs command tree discovery logic

3. **Test command classes not detected**
   - `Calculator`, `InterceptorCommand`, `DefaultCommandTest` not in module initializer
   - Only test fixture classes registered
   - Discovery logic needed

---

## Future Consideration

This work should be reconsidered if:

1. **Community contributions** - Multiple contributors willing to maintain parallel implementations
2. **AOT demand** - Users require Native AOT or aggressive trimming
3. **Performance problems** - Reflection startup time becomes a bottleneck (currently ~10μs per command)
4. **Tooling improvements** - Better shared abstractions between Reflection and Roslyn APIs
5. **Industry shift** - .NET ecosystem moves strongly toward source generation

### Alternative Approaches to Explore

1. **Opt-in attributes** - `[GenerateCommandDef]` for explicit control, minimal duplication
2. **Reflection emit** - Generate IL at runtime (faster than reflection, no code duplication)
3. **Compiled expression trees** - Hybrid approach for hot paths only
4. **Roslyn source generators v2** - Future Roslyn improvements may simplify the implementation

---

## Recommendation

**Defer indefinitely.** The current reflection-based approach:
- ✅ Works reliably
- ✅ Easy to maintain
- ✅ Handles all edge cases
- ✅ Performance is acceptable for CLI applications
- ✅ No user complaints about startup time

The source generator:
- ❌ Requires 1,200+ lines of duplicated logic
- ❌ Ongoing maintenance burden
- ❌ Limited user demand
- ❌ Complexity not justified by benefits

**Archive this work as a proof-of-concept** and revisit only if circumstances change.

---

## References

- **REFLECTION_INVENTORY.md** - Complete catalog of 23 reflection call sites
- **PHASE1_COMPLETE.md** - Details of infrastructure implementation
- **TESTING_STRATEGY.md** - Test approach for generated code
- **README.md** - Architecture and user guide
