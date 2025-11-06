Feed me a beer (૭ ◉༬◉)૭⁾⁾⁾⁾ [![Donate](https://img.shields.io/badge/Donate-PayPal-green.svg)](https://www.paypal.me/wad1m)
  
# Application Spacewarp Template Project
  
Preconfigured project to support application space warp 

## Changelog: 

**Novermber-2025**
* Upgrade to unity 6000 , purged old project , merged [DevDunk's project settings](https://github.com/smitdylan2001/Unity-6-URP-XR-Base) with latest [oculus URP fork](https://github.com/Oculus-VR/Unity-Graphics/commit/99353c21090bd40a239c619036beb3b605121919)
* ⚠ Version specific bug : [Gizmos don't work](https://github.com/Oculus-VR/Unity-Graphics/issues/55) - tried several 6000.x subversion and they all seem to be effected . 

**Novermber-2023**
* TextMeshPro motion vector support - should work great for free floating / fast moving text items like subtitles  ( blending is messed up , use the "UnlitTemplate w AlphaClipping" template instead ) 

**October-2023**
* Created branch for 2023.2.Beta ⚠ for URP v16.0.3 [try it at your own risk](https://github.com/nukadelic/unity-application-spacewarp/tree/2023.2)

**April-2023**
+ Bump minimum required version to 2021.3.21 ( thank you [tswierkot](https://github.com/tswierkot) - his [fork](https://github.com/tswierkot/unity-application-spacewarp/tree/2021.3.21) )
+ Post processing now running at 60fps & 120Hz at Max-Foveation level & 75% render resolution fairly stable 

**Jan-2023**
+ Patch Alpha Clipping Issue [42dbf6a](https://gin.g-node.org/FloppyDisk/asw-render-pipelines-universal/commit/42dbf6a25b33099b1249bcd03ccffc223224818e)
+ Patch Allow Material Override in shader graph [5a42db6](https://gin.g-node.org/FloppyDisk/asw-render-pipelines-universal/commit/5a42db665706440403125170c379deb6b998aff5)

**Apr-2022**
Updated to latest commit [30e14a2](https://github.com/Oculus-VR/Unity-Graphics/tree/30e14a2ca18f7c4c9903767895c1ca15d1af6c76)
Unity version 2021.2+ [URPv12](https://github.com/Oculus-VR/Unity-Graphics/commit/4f6daf0a988e86df35739c5fddbf6fe9bf9bb773)

**Jul-2022**
Added sample hlsl shader , see : Assets/Shaders/UnlitTemplate.shader

#### Links 
* ASW Developer Guide : https://developer.oculus.com/documentation/unity/unity-asw
* Video Lecture [Asynchronous Timewarp & Spacewarp for VR | Bonnie Mathew 2017](https://www.youtube.com/watch?v=gqVIJtRjtr8)
* [how to diagnose and fix common issues with ASW-enabled apps](https://github.com/oculus-samples/Unity-AppSpaceWarp)
