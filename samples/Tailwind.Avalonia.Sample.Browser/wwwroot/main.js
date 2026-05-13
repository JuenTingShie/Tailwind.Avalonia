import { dotnet } from './_framework/dotnet.js'

const isBrowser = globalThis.window !== undefined
const splash = isBrowser ? globalThis.document.querySelector('.tailwind-avalonia-splash') : null
const splashTitle = splash?.querySelector('.tailwind-avalonia-splash__title')
const splashBody = splash?.querySelector('.tailwind-avalonia-splash__body')

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
    if (!splash) {
        return
    }

    splash.classList.remove('tailwind-avalonia-splash--hidden')
    splash.classList.add('tailwind-avalonia-splash--error')

    if (splashTitle) {
        splashTitle.textContent = 'Browser sample failed to start'
    }

    if (splashBody) {
        const message = error instanceof Error ? error.message : String(error)
        splashBody.textContent = message
    }
}

async function waitForCanvas(container) {
    if (!container) {
        return
    }

    if (container.querySelector('canvas')) {
        await nextFrame()
        await nextFrame()
        return
    }

    await new Promise(resolve => {
        const observer = new MutationObserver(() => {
            if (!container.querySelector('canvas')) {
                return
            }

            observer.disconnect()
            resolve()
        })

        observer.observe(container, { childList: true, subtree: true })
        globalThis.setTimeout(() => {
            observer.disconnect()
            resolve()
        }, 15000)
    })

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