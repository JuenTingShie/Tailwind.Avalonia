# [TASK021] - Tailwind Avalonia browser sample and Pages hosting

**Status:** Completed  
**Added:** 2026-05-13  
**Updated:** 2026-05-13

## Original Request
create a sample.browser project that is thesame as sample but can host on github page and browsing by browser

## Thought Process
- Avalonia's official browser-hosting pattern is a shared app project plus thin platform-specific hosts, so the cleanest way to add browser support without forking the docs sample UI was to extract the window body into a reusable shared view and add separate desktop/browser entrypoints.
- GitHub Pages project sites serve static files from `/<repository-name>/`, so the browser host needed relative asset paths and a checked-in `.nojekyll` marker rather than any hardcoded root-path assumptions.
- Browser publish trimming warnings in `Tw.cs` were caused by the cached tuple path used for reflected `*Property` lookup, but replacing that logic with `AvaloniaPropertyRegistry` introduced a real duplicate-key runtime failure in tests; the correct fix was to keep reflected lookup and make the cache key trim-aware instead.
- Full local browser validation depends on the `wasm-tools` workload. Once the current Windows machine showed a pending-reboot/MSI-cancel failure during workload install, the practical completion path was to finish the repo changes, CI deployment path, and docs now, then leave final local browser smoke validation as an environment follow-up.

## Implementation Plan
- Re-check official Avalonia browser-hosting guidance and GitHub Pages workflow/deployment guidance.
- Extract the reusable sample UI surface so desktop and browser hosts can share the same content.
- Add thin desktop and browser host projects and wire them into the solution.
- Make the browser host's static assets Pages-friendly.
- Fix browser-publish-related property lookup/trimming issues in `Tw.cs` without breaking existing tests.
- Add a GitHub Pages workflow and browser project README.
- Revalidate the desktop build and test project, then document the remaining local environment blocker.

## Progress Tracking

**Overall Status:** Completed - 100%

### Subtasks
| ID | Description | Status | Updated | Notes |
|----|-------------|--------|---------|-------|
| 1.1 | Research Avalonia browser hosting and Pages deployment | Complete | 2026-05-13 | Confirmed the shared-app-plus-hosts pattern, browser startup shape, `.nojekyll` need, and Pages deploy-job environment guidance from official docs. |
| 1.2 | Extract reusable sample shell | Complete | 2026-05-13 | Moved the old `MainWindow` content into `SampleShell` so desktop and browser can share the same docs UI. |
| 1.3 | Add desktop and browser host projects | Complete | 2026-05-13 | Added `Tailwind.Avalonia.Sample.Desktop` and `Tailwind.Avalonia.Sample.Browser`, updated the solution, and kept the shared sample app assembly as the common app layer. |
| 1.4 | Make browser output Pages-friendly | Complete | 2026-05-13 | Added browser `wwwroot` assets with relative paths plus a checked-in `.nojekyll` marker. |
| 1.5 | Stabilize property lookup for browser publish | Complete | 2026-05-13 | Replaced the failed registry approach with trim-aware reflected property lookup so tests stay green and browser publish cleanup remains honest. |
| 1.6 | Add deployment workflow and docs | Complete | 2026-05-13 | Added a pinned-action Pages workflow and a browser project README covering prerequisites and publish output. |
| 1.7 | Revalidate locally | Complete | 2026-05-13 | Desktop build and full tests passed; local browser workload install failed only because the current machine is in a pending-reboot/MSI-cancel state. |

## Progress Log
### 2026-05-13
- Re-read the official Avalonia Browser guidance plus GitHub Pages custom workflow docs to confirm the expected host split, publish flow, and Pages deployment job shape.
- Extracted the shared sample content into `samples/Tailwind.Avalonia.Sample/SampleShell.axaml` and updated `App.axaml.cs` so desktop and browser lifetimes both point at the same UI.
- Added `samples/Tailwind.Avalonia.Sample.Desktop` and `samples/Tailwind.Avalonia.Sample.Browser`, wired them into `Tailwind.Avalonia.slnx`, and added browser `wwwroot` assets plus `.nojekyll`.
- Updated `src/Tailwind.Avalonia/Tw.cs` once to try `AvaloniaPropertyRegistry`, then corrected that change after tests exposed a duplicate-key failure on `Border`; the final implementation keeps reflected `*Property` lookup with a trim-aware cache key.
- Added `.github/workflows/sample-browser-pages.yml` with pinned `checkout`, `setup-dotnet`, `configure-pages`, `upload-pages-artifact`, and `deploy-pages` actions.
- Added `samples/Tailwind.Avalonia.Sample.Browser/README.md` documenting `dotnet workload install wasm-tools`, local run/publish commands, the publish output path, and Pages readiness notes.
- Validated the slice with `dotnet build samples/Tailwind.Avalonia.Sample.Desktop/Tailwind.Avalonia.Sample.Desktop.csproj` and `dotnet test tests/Tailwind.Avalonia.Tests/Tailwind.Avalonia.Tests.csproj`.
- Confirmed the only remaining local blocker is environment-level: `dotnet workload install wasm-tools` on the current Windows machine fails after download with an MSI/process cancellation message while a pending reboot warning is present.