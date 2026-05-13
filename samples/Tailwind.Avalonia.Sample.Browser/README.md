# Tailwind.Avalonia.Sample.Browser

This project hosts the existing Tailwind.Avalonia sample UI in Avalonia Browser so the docs-style sample can be published as a static site, including GitHub Pages.

## Local prerequisites

- .NET 10 SDK
- `wasm-tools` workload for the active SDK band

Install the workload once:

```powershell
dotnet workload install wasm-tools
```

If the machine has a pending reboot after SDK updates, finish the reboot before installing the workload. Avalonia's current WebAssembly troubleshooting guidance also recommends `<WasmBuildNative>true</WasmBuildNative>` for `libSkiaSharp` browser startup failures, which this project already enables.

## Run locally

```powershell
dotnet run --project samples/Tailwind.Avalonia.Sample.Browser/Tailwind.Avalonia.Sample.Browser.csproj
```

## Publish static output

```powershell
dotnet publish samples/Tailwind.Avalonia.Sample.Browser/Tailwind.Avalonia.Sample.Browser.csproj -c Release
```

The deployable static site is emitted to:

```text
samples/Tailwind.Avalonia.Sample.Browser/bin/Release/net10.0-browser/publish/wwwroot
```

That publish output already includes a `.nojekyll` marker so GitHub Pages serves the generated `_framework` assets correctly.

## GitHub Pages

The repository workflow at `.github/workflows/sample-browser-pages.yml` publishes this project's `wwwroot` output directly to GitHub Pages on pushes to `main` and on manual workflow dispatch.