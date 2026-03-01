let deferredPrompt = null;
let installListenerAttached = false;

window.pwa = {
    init: function () {
        window.addEventListener("beforeinstallprompt", (e) => {
            e.preventDefault();
            deferredPrompt = e;
            window.dispatchEvent(new Event("pwa-install-available"));
        });

        window.addEventListener("appinstalled", () => {
            deferredPrompt = null;
            window.dispatchEvent(new Event("pwa-install-available"));
        });
    },

    canInstall: function () {
        return deferredPrompt !== null;
    },

    install: async function () {
        if (!deferredPrompt) return;

        deferredPrompt.prompt();
        await deferredPrompt.userChoice;

        deferredPrompt = null;
        window.dispatchEvent(new Event("pwa-install-available"));
    },

    listenInstallAvailable: function (dotnetRef) {
        if (installListenerAttached) return;
        installListenerAttached = true;

        window.addEventListener("pwa-install-available", () => {
            dotnetRef.invokeMethodAsync("SetInstallAvailable");
        });
    }
};