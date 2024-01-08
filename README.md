# Avatar Plug-in for Unity : glTFast Plug-in
![](https://img.shields.io/badge/unity3D-2023.2.Xf1-red)
![](https://img.shields.io/badge/glTFast-latest-blue.svg)
![](https://img.shields.io/github/license/saltstack/salt)
![](https://img.shields.io/badge/AREmojiEditor-v6.1.00.X-blue)

<div align="center" style="margin-top:15px;">
 <img width="800" src="https://d3unf4s5rp9dfh.cloudfront.net/AREmoji_doc/overview4.PNG">
</div>
<br>

## Introduce
[glTFast](https://github.com/atteneder/glTFast) is an open-source library for loading and rendering glTF™ (GL Transmission Format) 3D models in Unity.
This is a lightweight and fast implementation compared to other glTF libraries, providing high performance even on mobile devices or low-end computers. This can be integrated with Unity's rendering engine. So we serve the [Avatar Plug-in for Unity](https://github.com/Samsung/avatar-plugin-for-unity), which loads AREmoji via glTFast and provides a variety of additional features. For detailed explanation, please refer to [Samsung Developers](https://developer.samsung.com/galaxy-ar-emoji/partner/overview.html).


<br>
<center>

#### Version Naming Convention
Type | Version |
--- | --- |
Official | 4.X |
Unofficial(LATEST) | 4.0.X |
</center>
<br>

## Why need to use Avatar Plug-in
Avatar provided by Samsung complies with the gltf 2.0 standard specification. However, in order to fully function as an Avatar, additional functions and processes are required.
For detailed explanation, please refer to [Samsung Developers](https://developer.samsung.com/galaxy-ar-emoji/partner/overview.html).


## How To Use
### 1. Project Settings
The Avatar Plug-in for Unity is validated in Unity version 2023.2.Xf1 and may work in some higher versions.
<center>

<img src="https://d3unf4s5rp9dfh.cloudfront.net/AREmoji_doc/CreateProject.png"  width="900" alt="Figure 1: Create Project"/>
</center>
<br>


here is a dependency on [glTFast](https://github.com/Unity-Technologies/com.unity.cloud.gltfast), so you need to import glTFast into your project first. Avatar Plug-in always follows the latest glTFast.
To import the gltfast package into the project, go to Package Manage -> Install package by name.. -> Enter "com.unity.cloud.gltfast"
<center>
<img src="https://d3unf4s5rp9dfh.cloudfront.net/AREmoji_doc/ImportGLTFast3.png" width="800" alt="Figure 3: Import GLTFast Pakage"/>
</center>
<br>


Finally, import the Avatar Plug-in for Unity Package into Proejct. The easiest way to install is to download and open the [Installer Package](https://package-installer.glitch.me/v1/installer/OpenUPM/com.samsung.avatar-plugin-for-unity?registry=https%3A%2F%2Fpackage.openupm.com&scope=com.samsung)
<center>
<img src="https://d3unf4s5rp9dfh.cloudfront.net/AREmoji_doc/avatar-plugin-for-unity4.png" width="800" alt="Figure 3: Import Avatar Plug-in for Unity Pakage"/>
</center>
<br>


The Avatar Plug-in for Unity automatically configures essential project settings when imported into a project. Please refer to the table below and note that project settings will change automatically.
<center>

#### Auto Project Setting Fratures
| Frature | Hierarchy | Value |
|----------|-------------|------|
| Minimum API Level| Player/Iedntification | Android 9.0 'Pie'(API level 28) |
| Normal Map Encoding | Player/Rendering | XYZ |
| Scriipting Backend | Player/Configuration | IL2CPP |
| Target Architectures | Player/Configuration | ARM64 |
| Always Included Shaders | Graphics/Built-in Shader Settings | Add glTF/PbrMetallicRoughness |
</center>
<br>

### 2. Usage
Once the Avatar Plug-in for Unity has been imported into your project, using it is very simple. We provide samples for each feature. You can run each sample and test it out.
<center>

<img src="https://d3unf4s5rp9dfh.cloudfront.net/AREmoji_doc/Samples.png" width="900" alt="Figure 4: Avatar Plug-in for Unity Samples"/>
</center>
<br>

## Open-Source Library

[The Avatar Plug-in for Unity](https://github.com/Samsung/avatar-plugin-for-unity) can be reviewed and modified by anyone to improve it. This allows adding or modifying features required to render glTF models in Unity. Moreover, the library is continually being developed and maintained by the Unity community, allowing users to obtain better results using the latest version of the library. Anyone with update requests for The Avatar Plug-in for Unity can contribute through the Github System. Send a “Pull Request” to [jeoungjukim](https://github.com/jeoungjukim)
