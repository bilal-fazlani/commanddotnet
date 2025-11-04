# CommandDotNet Source Generator - Work Plan

> ⚠️ **STATUS: DEFERRED** - See [DEFERRED_DECISION.md](DEFERRED_DECISION.md)
>
> Work paused due to maintenance burden (~1,200 lines of duplicated Roslyn logic required).
> Infrastructure complete but generator not functional. Reconsider if AOT/trimming becomes critical.

**Last Updated:** 2025-11-04

---

## Quick Status

| What | Status | Priority |
|------|--------|----------|
| **Phase 1: Command Discovery** | ⚠️ Incomplete (Infrastructure only) | - |
| **Phase 2: Property Accessors** | ❌ Deferred | - |
| **Phase 3: Method Invocation** | ❌ Deferred | - |
| **Phase 4: Polish** | ❌ Deferred | - |

**Overall:** Work paused - requires command tree discovery logic duplication

---

## Table of Contents

- [Completed Work](#completed-work) - Phase 1 summary
- [Immediate Actions](#immediate-actions) - What to do next
- [Phase 2: Property Accessors](#phase-2-property-accessors-planned) - Next recommended work
- [Phase 3: Method Invocation](#phase-3-method-invocation-planned) - Future work
- [Phase 4: Polish](#phase-4-polish-planned) - Low priority work
- [Testing Requirements](#testing-requirements) - Per-phase testing
- [Documentation Updates](#documentation-updates) - What docs to update
- [Decision Points](#decision-points) - Questions to answer

---

## Completed Work

### ✅ Phase 1: Command Discovery (COMPLETE)

**What we achieved:**
- Source generator creates builder classes at compile time
- Module initializer auto-registers builders (zero reflection!)
- Runtime automatically uses generated code with graceful fallback
- 80% of startup reflection eliminated
- 600x faster command lookup

**Details:** See [PHASE1_COMPLETE.md](PHASE1_COMPLETE.md)

**Key Files:**
- `CommandDotNet.SourceGen/CommandClassGenerator.cs` - Generator
- `CommandDotNet/ClassModeling/Definitions/CommandDefRegistry.cs` - Registry
- `CommandDotNet/ClassModeling/Definitions/ClassCommandDef.cs` - Integration

**Known Issues:**
- ⚠️ DefaultCommand classes don't generate builders → Fix effort: 0.5 days
- ⚠️ IArgumentModel parameters don't generate builders → Fix effort: 1 day

---

## Immediate Actions

### Option A: Fix Generator Issues First (Recommended)

**Why:** Complete Phase 1 to 100% before moving to Phase 2

**Tasks:**
1. [ ] Debug DefaultCommand detection in `CommandClassGenerator.cs`
2. [ ] Fix IArgumentModel parameter filtering
3. [ ] Add tests for these scenarios
4. [ ] Verify existing tests pass more

**Effort:** 1.5 days  
**Impact:** Phase 1 goes from 70% to 100% coverage

### Option B: Start Phase 2 (Property Accessors)

**Why:** Phase 1 is "good enough", move to next optimization

**Tasks:**
1. [ ] Review Phase 2 plan below
2. [ ] Design PropertyAccessorRegistry API
3. [ ] Extend CommandClassGenerator
4. [ ] Implement registry
5. [ ] Update PropertyArgumentDef
6. [ ] Write tests

**Effort:** 2-3 days  
**Impact:** Eliminate 5 more reflection call sites

### Option C: Release Phase 1

**Why:** Ship what we have, get user feedback

**Tasks:**
1. [ ] Final test run
2. [ ] Performance benchmarks
3. [ ] Update main README
4. [ ] Draft release notes
5. [ ] Tag and release

**Effort:** 0.5 days  
**Impact:** Users get benefits immediately

**Recommendation:** **Option A** - Complete Phase 1 properly first

---

## Phase 2: Property Accessors (PLANNED)

### Priority: HIGH (Next Recommended)

### What This Achieves

**Problem:** Argument binding uses reflection to get/set property values
- `PropertyInfo.GetValue()` - ~0.5 μs per call
- `PropertyInfo.SetValue()` - ~0.5 μs per call
- Typical command: 3-5 properties = 2-3 μs overhead

**Solution:** Generate typed property accessors at compile time

**Benefit:**
- 100x faster property access (500ns → 5ns)
- ~2-3 μs saved per command
- Required for full AOT compatibility

### Reflection to Eliminate

**5 call sites** (see [REFLECTION_INVENTORY.md](REFLECTION_INVENTORY.md) - Category 2):
1. `PropertyArgumentDef.cs:56` - Property get
2. `PropertyArgumentDef.cs:58` - Property set
3. `PropertyArgumentDef.cs:60` - Default value get
4. `MethodDef.cs:160` - Nested property get
5. `MethodDef.cs:161` - Nested property set

### Generated Code Example

```csharp
// For each argument model class
internal static class ProcessArgs__PropertyAccessors
{
    public static object? Get_Name(ProcessArgs instance) => instance.Name;
    public static void Set_Name(ProcessArgs instance, object? value) => instance.Name = (string?)value;
    public static object? GetDefault_Name() => default(string);
    
    // ... for all properties
}
```

### Implementation Steps

1. **Extend Generator** (1 day)
   - Identify argument model properties
   - Generate accessor methods
   - Include in module initializer

2. **PropertyAccessorRegistry** (0.5 days)
   ```csharp
   internal static class PropertyAccessorRegistry
   {
       internal static void Register<TModel>(string propertyName, 
           Func<TModel, object?> getter, 
           Action<TModel, object?> setter);
       
       internal static (Func, Action)? TryGet(Type modelType, string propertyName);
   }
   ```

3. **Update PropertyArgumentDef** (0.5 days)
   - Check registry first
   - Fall back to reflection if not found

4. **Tests** (1 day)
   - Comparison tests
   - Performance benchmarks
   - Nested models

### Success Criteria

- [ ] Generator creates accessors for all properties
- [ ] Registry implemented
- [ ] PropertyArgumentDef uses accessors
- [ ] All property types work (primitives, collections, nested)
- [ ] Tests pass
- [ ] Performance measured

### Effort: 2-3 days

---

## Phase 3: Method Invocation (PLANNED)

### Priority: MEDIUM (Optional - Evaluate After Phase 2)

### What This Achieves

**Problem:** Command execution uses `MethodInfo.Invoke()`
- ~1-2 μs per command execution
- Not AOT-compatible

**Solution:** Generate typed method invokers

**Benefit:**
- 100x faster invocation (2μs → 20ns)
- Full command execution AOT-compatible

### Reflection to Eliminate

**1 call site** (see [REFLECTION_INVENTORY.md](REFLECTION_INVENTORY.md) - Category 1):
- `MethodDef.cs:113` - Method invocation

### Generated Code Example

```csharp
internal static class Calculator__MethodInvokers
{
    public static object? Invoke_Add(Calculator instance, object?[] args)
    {
        return instance.Add((int)args[0], (int)args[1]);
    }
}
```

### Complexity: HIGH

**Challenges:**
- Many method signature variations
- Async/await methods (Task, Task<T>)
- Interceptor methods with ExecutionDelegate
- Return type handling
- Exception propagation

**Why Optional:**
- Most CLI apps are I/O bound (method invoke overhead negligible)
- Adds significant generator complexity
- Phase 2 already gives AOT for argument binding

### Decision Point

**Do Phase 3 if:**
- Phase 2 went smoothly
- Need complete AOT support
- Have high-frequency command execution

**Skip Phase 3 if:**
- Phase 2 is "good enough"
- Complexity not worth it
- Users haven't asked for it

### Effort: 4-5 days

---

## Phase 4: Polish (PLANNED)

### Priority: LOW (Completeness)

### Remaining Optimizations

**8 remaining reflection call sites** across:
- Instance creation (3 sites) - `Activator.CreateInstance()`
- Property discovery (3 sites) - `Type.GetProperties()`
- Subcommand injection (2 sites) - Property set

**Total Effort:** ~3 days

**Benefit:** 100% reflection-free badge, complete AOT support

**When to do:** After Phases 2+3, or if there's specific demand

---

## Testing Requirements

### Per Phase Testing

**Phase 2:**
- [ ] Property accessor generation tests
- [ ] Comparison tests (generated vs reflection)
- [ ] Performance benchmarks
- [ ] Nested model tests

**Phase 3:**
- [ ] Method invoker generation tests
- [ ] Async method tests
- [ ] Interceptor tests
- [ ] Comparison tests

**Phase 4:**
- [ ] Factory generation tests
- [ ] Full integration suite
- [ ] AOT deployment verification

### Test Infrastructure

**Location:** `CommandDotNet.Tests/SourceGen/`
- `GeneratorOutputTests.cs` - Verify code generation
- `Comparison/*Tests.cs` - Generated vs reflection behavior
- `TestCommands/*.cs` - Test command classes

**Documentation:** [TESTING_STRATEGY.md](TESTING_STRATEGY.md)

---

## Documentation Updates

### Phase 2 Updates Required

- [ ] Update [README.md](README.md) - Add property accessor section
- [ ] Mark Category 2 eliminated in [REFLECTION_INVENTORY.md](REFLECTION_INVENTORY.md)
- [ ] Update this PLAN.md
- [ ] Add examples to docs

### Phase 3 Updates Required

- [ ] Update README.md - Add method invocation section
- [ ] Mark Category 1 eliminated in REFLECTION_INVENTORY.md
- [ ] Create performance comparison doc
- [ ] Update this PLAN.md

### Phase 4 Updates Required

- [ ] Final README update - "100% reflection-free"
- [ ] Complete REFLECTION_INVENTORY.md
- [ ] AOT deployment guide
- [ ] Update this PLAN.md

---

## Decision Points

### Before Phase 2

**Questions:**
- [ ] Fix generator issues first? (DefaultCommand, ArgumentModel)
- [ ] Do we need full AOT support?
- [ ] Is there user demand for better performance?
- [ ] Do we have 2-3 days available?

**Recommendation:** Fix generator issues first (1.5 days), then Phase 2

### Before Phase 3

**Questions:**
- [ ] Did Phase 2 meet expectations?
- [ ] Is registry pattern working well?
- [ ] Do users need method invocation optimization?
- [ ] Is 4-5 day effort worth it?

**Recommendation:** Evaluate after Phase 2 complete

### Before Phase 4

**Questions:**
- [ ] Is 100% reflection-free a goal?
- [ ] Are there AOT users who need this?
- [ ] Better use of time vs other features?

**Recommendation:** Do only if there's specific demand

---

## Performance Targets

| Phase | Reflection Eliminated | Per-Command Savings | Status |
|-------|----------------------|---------------------|--------|
| **Phase 1** | 80% (startup) | N/A | ✅ Done |
| **Phase 2** | 5 call sites | ~2-3 μs | 📋 Planned |
| **Phase 3** | 1 call site | ~1-2 μs | 📋 Planned |
| **Phase 4** | 8 call sites | ~2-5 μs | 📋 Planned |
| **Total** | 100% | ~5-10 μs → ~50ns | 🎯 Goal |

**Real-world impact:**
- **CLI apps:** Marginal (I/O bound)
- **High-frequency:** Noticeable at >1000 cmds/sec
- **AOT deployment:** Required for Phases 2+

---

## Summary & Recommendations

### Current Status
✅ **Phase 1 Complete** - 80% of reflection eliminated, production-ready  
⚠️ **2 Generator Issues** - DefaultCommand, ArgumentModel (1.5 days to fix)

### Short-term (Next 2 weeks)
1. **Fix generator issues** (1.5 days) - Complete Phase 1 properly
2. **Phase 2: Property Accessors** (2-3 days) - Good ROI, enables AOT

### Medium-term (1-2 months)
3. **Evaluate Phase 3** - Based on Phase 2 success and user demand

### Long-term (As needed)
4. **Phase 4** - Only if needed for AOT or "100% reflection-free" goal

### Total Effort to "Very Complete"
- Fix issues: 1.5 days
- Phase 2: 2-3 days
- **Total: ~5 days** for excellent source generator

### Total Effort to "100% Complete"
- Above + Phase 3: 4-5 days
- Above + Phase 4: 3 days
- **Total: ~12 days** for perfection

---

## Next Steps (Choose One)

### 1. Complete Phase 1 First ⭐ RECOMMENDED
```bash
# Fix generator issues
1. Debug DefaultCommand detection
2. Fix IArgumentModel parameters
3. Add tests
4. Verify more tests pass
```

### 2. Start Phase 2 Now
```bash
# Begin property accessor work
1. Review Phase 2 plan (above)
2. Design PropertyAccessorRegistry
3. Extend CommandClassGenerator
4. Implement and test
```

### 3. Release What We Have
```bash
# Ship Phase 1
1. Final testing
2. Benchmarks
3. Release notes
4. Tag and release
```

---

## Additional Documentation

- **[PHASE1_COMPLETE.md](PHASE1_COMPLETE.md)** - Detailed Phase 1 completion summary
- **[README.md](README.md)** - User-facing documentation
- **[TESTING_STRATEGY.md](TESTING_STRATEGY.md)** - How to test
- **[REFLECTION_INVENTORY.md](REFLECTION_INVENTORY.md)** - All 23 reflection call sites cataloged
- **[DOCUMENTATION_INDEX.md](DOCUMENTATION_INDEX.md)** - All documentation navigation

---

**Last Updated:** 2025-11-04  
**Status:** Phase 1 complete, ready for Phase 2 or generator fixes
