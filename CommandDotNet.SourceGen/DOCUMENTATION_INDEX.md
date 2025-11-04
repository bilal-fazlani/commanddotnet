# CommandDotNet Source Generator Documentation

> ⚠️ **PROJECT STATUS: DEFERRED** - See **[DEFERRED_DECISION.md](DEFERRED_DECISION.md)**
>
> Infrastructure complete but work paused due to maintenance burden.

> 📋 **[PLAN.md](PLAN.md)** - Work plan and status

---

## Documentation Files (7 total)

| File | Purpose | Audience |
|------|---------|----------|
| **[DEFERRED_DECISION.md](DEFERRED_DECISION.md)** | ⚠️ Why work was paused, scope analysis | Everyone |
| **[PLAN.md](PLAN.md)** | Work plan, what was completed | Contributors |
| **[README.md](README.md)** | User guide, architecture overview | Users & Contributors |
| **[PHASE1_COMPLETE.md](PHASE1_COMPLETE.md)** | Phase 1 implementation details | Contributors |
| **[REFLECTION_INVENTORY.md](REFLECTION_INVENTORY.md)** | All 23 reflection call sites cataloged | Optimization work |
| **[TESTING_STRATEGY.md](TESTING_STRATEGY.md)** | Testing approach and framework | Test authors |
| **[DOCUMENTATION_INDEX.md](DOCUMENTATION_INDEX.md)** | This file - quick reference | Everyone |

---

## Quick Reference

### For Contributors (Start Here)
1. **[PLAN.md](PLAN.md)** ⭐ - What's done, what's next, priorities
2. **[README.md](README.md)** - Architecture and user guide
3. **[TESTING_STRATEGY.md](TESTING_STRATEGY.md)** - How to test

### For Understanding What Was Built
- **[PHASE1_COMPLETE.md](PHASE1_COMPLETE.md)** - Everything about Phase 1:
  - Generator implementation
  - Registry + module initializer pattern
  - Runtime integration
  - Test infrastructure
  - Known issues
  - Performance results

### For Optimization Work
- **[REFLECTION_INVENTORY.md](REFLECTION_INVENTORY.md)** - Detailed catalog:
  - All 23 reflection call sites
  - Exact file:line locations
  - Purpose and optimization path for each

---

## Quick Answers

**Q: What's the status?**  
A: ⚠️ **DEFERRED** - Read [DEFERRED_DECISION.md](DEFERRED_DECISION.md) for why

**Q: Where do I start?**  
A: Read [DEFERRED_DECISION.md](DEFERRED_DECISION.md) first, then [PLAN.md](PLAN.md)

**Q: What work was completed?**  
A: Infrastructure only. See [PLAN.md](PLAN.md) or [PHASE1_COMPLETE.md](PHASE1_COMPLETE.md)

**Q: Why was it stopped?**  
A: Maintenance burden (~1,200 lines of duplicated Roslyn logic). See [DEFERRED_DECISION.md](DEFERRED_DECISION.md)

**Q: Will this be completed?**  
A: Only if AOT/trimming becomes critical or community contributors help. See [DEFERRED_DECISION.md](DEFERRED_DECISION.md)

**Q: How does it work?**  
A: Read [README.md](README.md) or [PHASE1_COMPLETE.md](PHASE1_COMPLETE.md) (detailed)

**Q: Why two projects?**  
A: See [README.md](README.md) - "Architecture: Why Two Projects?"

**Q: What reflection remains?**  
A: See [REFLECTION_INVENTORY.md](REFLECTION_INVENTORY.md) - 23 call sites cataloged

**Q: How do I test?**  
A: See [TESTING_STRATEGY.md](TESTING_STRATEGY.md)

**Q: Is the generator working?**  
A: No - generates module initializer but no individual builders. Needs command discovery logic.

---

**Status:** Infrastructure complete, generator non-functional, work deferred ⚠️

