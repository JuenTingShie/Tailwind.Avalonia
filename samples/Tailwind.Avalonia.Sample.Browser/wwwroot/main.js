import { dotnet } from './_framework/dotnet.js'

const isBrowser = globalThis.window !== undefined
const splash = isBrowser ? globalThis.document.querySelector('.tailwind-avalonia-splash') : null
const splashTitle = splash?.querySelector('.tailwind-avalonia-splash__title')
const splashBody = splash?.querySelector('.tailwind-avalonia-splash__body')
const splashContainer = isBrowser ? globalThis.document.getElementById('out') : null

if (!isBrowser) {
    throw new Error('Expected Tailwind.Avalonia.Sample.Browser to run in a browser environment.')
}

function nextFrame() {
    return new Promise(resolve => globalThis.requestAnimationFrame(() => resolve()))
}

function hideSplash() {
    if (!splash || splash.classList.contains('tailwind-avalonia-splash--hidden')) {
        return
    }

    splash.classList.add('tailwind-avalonia-splash--hidden')
    splash.addEventListener('transitionend', () => splash.remove(), { once: true })
    globalThis.setTimeout(() => splash.remove(), 300)
}

function showStartupError(error) {
    const splashElement = ensureSplash()
    if (!splashElement) {
        return
    }

    const titleElement = splashElement.querySelector('.tailwind-avalonia-splash__title')
    const bodyElement = splashElement.querySelector('.tailwind-avalonia-splash__body')

    splashElement.classList.remove('tailwind-avalonia-splash--hidden')
    splashElement.classList.add('tailwind-avalonia-splash--error')

    if (titleElement) {
        titleElement.textContent = 'Browser sample failed to start'
    }

    if (bodyElement) {
        const message = error instanceof Error
            ? [error.message, error.stack].filter(Boolean).join('\n\n')
            : String(error)
        bodyElement.textContent = message
    }
}

function ensureSplash() {
    if (!splashContainer) {
        return null
    }

    const currentSplash = splashContainer.querySelector('.tailwind-avalonia-splash')
    if (currentSplash) {
        return currentSplash
    }

    const restoredSplash = globalThis.document.createElement('div')
    restoredSplash.className = 'tailwind-avalonia-splash tailwind-avalonia-splash--error'
    restoredSplash.innerHTML = `
        <p class="tailwind-avalonia-splash__eyebrow">Tailwind.Avalonia</p>
        <h1 class="tailwind-avalonia-splash__title">Browser sample failed to start</h1>
        <p class="tailwind-avalonia-splash__body"></p>
    `

    splashContainer.appendChild(restoredSplash)
    return restoredSplash
}

globalThis.addEventListener('error', event => {
    showStartupError(event.error ?? event.message ?? 'Unknown browser startup error.')
})

globalThis.addEventListener('unhandledrejection', event => {
    showStartupError(event.reason ?? 'Unhandled promise rejection during browser startup.')
})

async function waitForCanvas(container) {
    if (!container) {
        return
    }

    const timeoutAt = (globalThis.performance?.now() ?? 0) + 15000

    while (!container.querySelector('canvas') && !container.querySelector('.avalonia-native-host')) {
        if ((globalThis.performance?.now() ?? timeoutAt) >= timeoutAt) {
            break
        }

        await nextFrame()
    }

    await nextFrame()
    await nextFrame()
}

try {
    const dotnetRuntime = await dotnet
        .withDiagnosticTracing(false)
        .withApplicationArgumentsFromQuery()
        .create()

    const config = dotnetRuntime.getConfig()
    const runMainPromise = dotnetRuntime.runMain(config.mainAssemblyName, [globalThis.location.href])

    await waitForCanvas(globalThis.document.getElementById('out'))
    hideSplash()

    await runMainPromise
} catch (error) {
    showStartupError(error)
    throw error
}