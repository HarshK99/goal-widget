# Goal widget artwork

Recorded 2026-09-24. The app now uses `src/GoalWidget/Assets/landscape.png`, an AI-generated replacement; it does not ship the provisional Unsplash photograph.

## Source and use

- Generated with the built-in OpenAI image-generation tool in Phase 3. No photograph was attached or supplied as a reference. The prompt described a new fictional mountain scene, with no text, logo, people or buildings.
- Original output: `C:/Users/acer/.codex/generated_images/01a0d3ea-8601-7ec1-a38a-2d84addb2a17/exec-69e56f99-f901-4342-a82a-acdef242bf09.png`. Copied unchanged into the project, so the app does not depend on that machine-local path.
- PNG, 1254 × 1254 pixels, 1,965,691 bytes. SHA-256: `25321379AAE4A067C2320BE53E4E3AA58BBFDCFE5B4000715BCA0D75FC28B50E`.
- Permission basis: the [OpenAI Terms of Use, Content](https://openai.com/policies/terms-of-use/) assign OpenAI's output rights to the user to the extent permitted by law. Use remains subject to the applicable account agreement. This is generated project artwork, not a stock-photo sublicense or a claim of exclusive copyright. A source notice is included as app content.
- The standalone image and native compact card were viewed. The user accepts the current visual treatment for now; formal contrast and fit checks remain pending.

## Tray icon

`src/GoalWidget/Assets/goal.ico` (added 2026-10-04) is original project artwork drawn from plain shapes by a script: a rounded pale-blue card, two mountains in the app's navy and slate blue, and a warm sun. It contains 16, 20, 24, 32, 48, 64 and 256 pixel sizes and uses no third-party image. Windows loaded the file at 16–32 pixels; its appearance in the real tray has not been observed.

## Generation brief

Square photographic landscape for a calm desktop goal widget: pale cloudy pearl-blue sky across the upper 40%, snow-covered pyramidal peak slightly right of center, layered muted slate-blue mountains in the lower half, darker foreground slopes, cool mist and quiet warm dawn light from the left. Subtle realistic rock/snow textures, desaturated air, low contrast behind future text. No UI, text, frame, rounded corners, people, buildings, logos or watermarks. Requested an original fictional scene, not reproduction of a specific photograph.

## Composition in the app

The square image fills the square card without a deliberate crop, at 0.83 opacity over a solid pale frost base. Pale frost remains strong through the text region, including long goals. Warm diagonal light, cool surface reflection, a fine outer rim and an inset hairline supply depth without animation. Since the 2026-10-05 WPF rebuild the app draws its own 21-unit corners (the preview's 30 at card scale) and its own soft shadow; live background blur is no longer used.

Georgia remains a system font, not a bundled font file. The compact card uses a 27.3–16.1 font-size range with shared text measurement; the earlier -44 character spacing is gone because WPF has no letter-spacing. Explicit line breaks and centered alignment remain. A window capture showed the card, artwork and centered goal; final contrast and varied-goal fit checks remain pending.

## Historical reference

`preview/index.html` and `preview/landscape.jpg` remain unchanged as the accepted design reference. The original photo URL is recorded in `preview/README.md`; searches did not establish the photographer reliably, and direct retrieval of the Unsplash search page returned HTTP 401. No redistribution clearance is claimed for that preview image. Do not include the preview folder in a release.
