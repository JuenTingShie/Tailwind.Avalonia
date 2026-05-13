import { dotnet } from './_framework/dotnet.js'

const isBrowser = typeof window !== 'undefined'

if (!isBrowser) {
    throw new Error('Expected Tailwind.Avalonia.Sample.Browser to run in a browser environment.')
}

const dotnetRuntime = await dotnet
    .withDiagnosticTracing(false)
    .withApplicationArgumentsFromQuery()
    .create()

const config = dotnetRuntime.getConfig()

await dotnetRuntime.runMain(config.mainAssemblyName, [globalThis.location.href])