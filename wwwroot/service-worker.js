importScripts('./service-worker-assets.js');

const CACHE_NAME = 'pdf-it-yourself-' + self.assetsManifest.version;

self.addEventListener('install', event => {
    event.waitUntil(
        caches.open(CACHE_NAME).then(cache => {
            return cache.addAll(self.assetsManifest.assets.map(a => new URL(a.url, self.location).toString()));
        })
    );
    self.skipWaiting();
});

self.addEventListener('activate', event => {
    event.waitUntil(
        caches.keys().then(keys => Promise.all(keys.filter(k => k !== CACHE_NAME).map(k => caches.delete(k))))
    );
    self.clients.claim();
});

self.addEventListener('fetch', event => {
    if (event.request.method !== 'GET') return;

    event.respondWith(
        caches.match(event.request).then(cached => cached || fetch(event.request))
    );
});