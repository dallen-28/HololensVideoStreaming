# HololensVideoStreaming
A Unity Windows Mixed Reality project which streams video from various medical devices and displays it through virtual screens. Compatible with the Hololens 2. The repository contains a number of scenes which can deployed as standalone applications to the Hololens 2 or run directly in tbe unity editor.

## Scenes

### GestureTutorialScene
Features a number of levels a user can go through to learn the various hand gesture used for interacting with virtual objects in the Hololens 2.
### VirtualDisplayScene
Enables a network-based streaming infrastructure through PLUS and OpenIGTLink-Unity. Suitable for deploying directly to Hololens 2.
### VirtualDisplaySceneNoPlus
Enables video streaming directly through a framegrabber, not suitable for deploying to the Hololens.
### CTVisualizationScene
Visualization of CT slices in virtual panels, interaction enabled by virtual sliders.
