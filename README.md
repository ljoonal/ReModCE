# ReMod LJ

A minimal fork of [ReMod Community Edition](https://github.com/RequiDev/ReModCE) for paranoid users.
This adds linux build support and removes some features which contact non-vrchat remote servers.

## Description

This is essentially a public version of RequiDev's invite-only VRChat mod. It's a cut-down version with no connection to the ReMod server and with it's security measures removed.  

If you want a feature or have a bug report, try the original version.

## Installation & Usage

Build the dlls and put them in your VRC folder, with the loader going into the Mods folder.  

The loader has been stripped of automated updating features completely.
No builds will be provided, build from source.
If you are trusting enough for random .dll files, just use the original version.

The loader is still required though, since this fork aims to stay close to the upstream to make merging changes easier.

## Features

* Recently Used Avatars/Avatar History (Remembers up to 25 avatars)
* Global Dynamic Bones with advanced settings so you have full control over where colliders go
* Fly/Noclip (Not usable in Game/Club worlds)
* ESP/Player Highlighting (Not usable in Game/Club worlds)
* Wireframe ESP (Players, Pickups, World) (Not usable in Game/Club worlds)
* Third Person (Not usable in Game/Club worlds)
* Small UI adjustments like adding a "Paste" button to input popups.
* FBT Calibration Saver as already seen at [RequiDev/FBTSaver](https://github.com/RequiDev/FBTSaver)

## Hotkeys

* CTRL + F = Noclip
* CTRL + T = Thirdperson

## Credits

[loukylor](https://github.com/loukylor) - For selected VRChat function reflections and his button mover code which I am using  
[knah](https://github.com/knah) - For his usage of GitHub Action which I've looked at to figure out on how to do CI on GitHub

## Notable Mentions

[emmVRC](https://github.com/emmVRC) - They inspired me to create this project in the first place. This would not exist without them  
