# Server-Side-Rendering (SSR) with Next.js

We used Next.js as a framework to create our SSR -based frontend.

**Task:** Explain shortly why our use-case is not a good fit for build-time rendering

Build-time rendering is also alled Static-Site-Generation (SSG) and as this term implies, the site is static. It won't automatically update a change in values. To update the page, it has to be built again.
Since our application is based on a data stream and therefore, more or less, real-life data, our website should update automatically. Hence, SSG is not suitable for our use-case.

