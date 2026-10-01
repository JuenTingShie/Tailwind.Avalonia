<!-- code-review-graph MCP tools -->
## MCP Tools: code-review-graph

**IMPORTANT: This project has a knowledge graph. ALWAYS use the
code-review-graph MCP tools BEFORE using Grep/Glob/Read to explore
the codebase.** The graph is faster, cheaper (fewer tokens), and gives
you structural context (callers, dependents, test coverage) that file
scanning cannot.

### When to use graph tools FIRST

- **Exploring code**: `semantic_search_nodes_tool` or `query_graph_tool` instead of Grep
- **Understanding impact**: `get_impact_radius_tool` instead of manually tracing imports
- **Code review**: `detect_changes_tool` + `get_review_context_tool` instead of reading entire files
- **Finding relationships**: `query_graph_tool` with callers_of/callees_of/imports_of/tests_for
- **Architecture questions**: `get_architecture_overview_tool` + `list_communities_tool`

Fall back to Grep/Glob/Read **only** when the graph doesn't cover what you need.

### Key Tools

| Tool | Use when |
| ------ | ---------- |
| `detect_changes_tool` | Reviewing code changes — gives risk-scored analysis |
| `get_review_context_tool` | Need source snippets for review — token-efficient |
| `get_impact_radius_tool` | Understanding blast radius of a change |
| `get_affected_flows_tool` | Finding which execution paths are impacted |
| `query_graph_tool` | Tracing callers, callees, imports, tests, dependencies |
| `semantic_search_nodes_tool` | Finding functions/classes by name or keyword |
| `get_architecture_overview_tool` | Understanding high-level codebase structure |
| `refactor_tool` | Planning renames, finding dead code |

### Workflow

1. The graph auto-updates on file changes (via hooks).
2. Use `detect_changes_tool` for code review.
3. Use `get_affected_flows_tool` to understand impact.
4. Use `query_graph_tool` pattern="tests_for" to check coverage.

## Scope: Tailwind CSS v4.3 only

This library implements **Tailwind CSS v4.3** and nothing beyond it. The official documentation at
https://tailwindcss.com/docs (v4.3) is the single source of truth.

- **Only official classes.** Every class name, value form, variant and modifier must exist in the
  Tailwind v4.3 docs. Do not invent classes, aliases, shorthands, value forms or variants that
  Tailwind does not have, even when they would be convenient for Avalonia.
- **Same name, same meaning.** A class must keep Tailwind's semantics (units, defaults, scale values,
  "last class wins" behaviour). Where Avalonia cannot match exactly, map to the closest equivalent and
  document the difference; never repurpose a Tailwind name for something else.
- **No extra functionality.** Do not add behaviour Tailwind does not provide (extra scale steps,
  Avalonia-only utilities, custom keywords, additional variants). A feature request for something
  outside Tailwind is out of scope.
- **Verify before implementing.** Check the class on tailwindcss.com before writing code, and cite the
  doc page in the issue/PR. If it is not in the v4.3 docs, do not implement it.
- **Removed or renamed in v4** (for example bare `shadow`, `leading-tight`-style names, `ring-offset-*`
  no longer documented) are not added. Existing support for such classes should be treated as legacy:
  do not extend it, and prefer aligning or removing it when touching that code.
- **Unsupported official classes** stay unimplemented with the reason recorded in the README Notes
  column and the sample page's "Not implemented yet" note; do not substitute a non-Tailwind class.
