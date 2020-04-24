var version = "v0::5" + "fundamentals";
self.addEventListener("activate", function (event) {
    event.waitUntil(
        caches.keys().then(function (cacheNames) {
            return Promise.all(
                cacheNames.filter(function (cacheName) {
                    console.log(cacheName);
                    if (cacheName != version)
                        return true;
                }).map(function (cacheName) {
                    return caches.delete(cacheName);
                })
            );
        })
    );
});
self.addEventListener("install", function (event) {
    console.log("WORKER: install event in progress.");
    event.waitUntil(
        /* The caches built-in is a promise-based API that helps you cache responses,
             as well as finding and deleting them.
          */
        caches
            /* You can open a cache by name, and this method returns a promise. We use
               a versioned cache name here so that we can remove old cache entries in
               one fell swoop later, when phasing out an older service worker.
            */
            .open(version)
            .then(function (cache) {
                cache.addAll([
                    // levels 11-20
                ]);
                /* After the cache is opened, we can fill it with the offline fundamentals.
                 The method below will add all resources we"ve indicated to the cache,
                 after making HTTP requests for each of them.
              */
                return cache.addAll([
                    // core assets & levels 1-10
                    "bundle.min.js",
                    "bundle.min.css",
                    "fonts/Far_Mitra.woff",
                    "fonts/myriadpro.woff"
                ]);
            })
            .then(function () {
                console.log("WORKER: install completed");
            })
    );
});
// Cache then network
self.addEventListener('fetch', (e) => {
    e.respondWith(
        caches.match(e.request).then((r) => {
            console.log('[Service Worker] Fetching resource: ' + e.request.url);
            return r || fetch(e.request).then((response) => {
                return caches.open(cacheName).then((cache) => {
                    console.log('[Service Worker] Caching new resource: ' + e.request.url);
                    cache.put(e.request, response.clone());
                    return response;
                });
            });
        })
    );
});