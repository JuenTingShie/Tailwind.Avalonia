# Project Brief

## Summary
Build an AvaloniaUI package library that brings Tailwind CSS philosophy into Avalonia.

## Primary Goal
- Deliver reusable styling primitives for Avalonia 12+ with Tailwind 4.2 naming and mental model.
- First milestone is Spacing utilities: Padding and Margin.
- Target runtime and SDK baseline is .NET 10.

## Scope Rules
- Prefer constrained, composable utility-style APIs over ad-hoc styling.
- Preserve Avalonia-native consumption paths where possible.
- Resource naming must stay consistent with established shared token conventions.

## Initial Naming Rules
- `p-<number>` -> `Padding<number>`
- `px-<number>` -> `PaddingX<number>`
- `m-<number>` -> `Margin<number>`
- `mx-<number>` -> `MarginX<number>`
- Remaining utilities should follow Tailwind documentation semantics, adapted to Avalonia-safe naming when needed.

## Initial Deliverable
- Package-level resource entry point.
- Spacing token definitions.
- Spacing utility application strategy.
- Clear next target after Spacing MVP.

## Current Reality
- Repository now contains a solution, a packable `Tailwind.Avalonia` library, and a sample Avalonia app.
- Spacing MVP is implemented with generated `StaticResource` keys plus the `tw:Tw.Class` attached-property parser.
- Package resource entry currently lives at `avares://Tailwind.Avalonia/Themes/Tailwind.axaml`.