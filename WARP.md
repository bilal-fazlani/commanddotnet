# Project Guidelines

## Testing Strategy

Favor testing at the seams to enable easy refactoring of internal implementation without requiring extensive test updates. Focus tests on public APIs and integration points rather than internal implementation details.

Tests should prefer using Verify with full output assertions to be consistent with existing test patterns. Prefer Output over OutputContainsTexts except where testing a specific line.

## Documentation Guidelines

### Code Snippets

**Always use mdsnippets for C# code examples in documentation**. This ensures documentation stays in sync with code changes.

1. Add example code to `CommandDotNet.DocExamples/` directory
2. Mark code sections with snippet markers:
   ```csharp
   // begin-snippet: snippet_name
   public void MyExample() { }
   // end-snippet
   ```

3. Reference the snippet in markdown:
   ```markdown
   <!-- snippet: snippet_name -->
   <!-- endSnippet -->
   ```

4. Run `./scripts/mkdocs-snippets.sh` to update docs with actual code

The tool extracts snippets from source files and generates markdown with links to the source, ensuring examples are always accurate and up-to-date when signatures change.

See existing examples in `docs/` for reference patterns.
