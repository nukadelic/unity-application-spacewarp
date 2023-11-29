Feed me a beer (૭ ◉༬◉)૭⁾⁾⁾⁾ [![Donate](https://img.shields.io/badge/Donate-PayPal-green.svg)](https://www.paypal.me/wad1m)
  
# Application Spacewarp Template Project
  
Project is preconfigured with ASW for the Oculus Quest 1 &amp; 2 using a custom render pipeline  
  
> ⚠ If you having trouble with dependencies on a freshly cloned project , make sure the editor have your account signed in so the package manager can download the dependencies , if you still can't open then : enter safe mode -> exit safe -> re import all assets .  
> * Starting from Jul-2023 the oculus store apk's must target Android 12L 
  
![screenshot](https://raw.githubusercontent.com/nukadelic/unity-application-spacewarp/master/Img/screenshot.png)  

### Update (Novermber-2023)

* TextMeshPro motion vector support - should work great for free floating / fast moving text items like subtitles 

### Update (October-2023)

* Created branch for 2023.2.Beta ⚠ for URP v16.0.3 [try it at your own risk](https://github.com/nukadelic/unity-application-spacewarp/tree/2023.2)

### Update (April-2023)

+ Bump minimum required version to 2021.3.21 ( thank you [tswierkot](https://github.com/tswierkot) - his [fork](https://github.com/tswierkot/unity-application-spacewarp/tree/2021.3.21) )
+ Post processing now running at 60fps & 120Hz at Max-Foveation level & 75% render resolution fairly stable 

### Update (Jan-2023)

+ Patch Alpha Clipping Issue [42dbf6a](https://gin.g-node.org/FloppyDisk/asw-render-pipelines-universal/commit/42dbf6a25b33099b1249bcd03ccffc223224818e)
+ Patch Allow Material Override in shader graph [5a42db6](https://gin.g-node.org/FloppyDisk/asw-render-pipelines-universal/commit/5a42db665706440403125170c379deb6b998aff5)

### Update (Apr-2022)

Updated to latest commit [30e14a2](https://github.com/Oculus-VR/Unity-Graphics/tree/30e14a2ca18f7c4c9903767895c1ca15d1af6c76)

Unity version 2021.2+ [URPv12](https://github.com/Oculus-VR/Unity-Graphics/commit/4f6daf0a988e86df35739c5fddbf6fe9bf9bb773)

### Update (Jul-2022)

Added sample hlsl shader , see : Assets/Shaders/UnlitTemplate.shader

### APK File 

Try the build : https://github.com/nukadelic/unity-application-spacewarp/releases/tag/apk

### Notes

* 120Hz is still in experimental stage , to get it working enable it inside the Quest headset in the settings under experimental features. 
* Making materials in shader graph will support MotionVectors by default, custom hlsl shaders needs to be edited manually othewise they will jitter when in motion.

### Config 

* The scene in the screen shot can be found in `Assets/Example/Scenes/SampleScene.unity`
* Completely **removed** Oculus SDK plugin , now using only the package manager provided dll's  
* All the project settings are allready setup to be Spacewarp ready 
* Shader graph seems to be working as expected 
* Additional Project settings 
  * Linear , Vulkan 
  * Android 12L ( API Level 31 ) 
  * IL2CPP , .NET 4.x  
  * Texture Compression : ASTC 

### Building 

For faster builds you can swap to the Mono Scripting backend in `Project Settings > Player > Other Settings > Configuration > Scripting Backend` , but it is required to have IL2CPP when publishing the application on the store 
  
### Links 

* ASW Developer Guide : https://developer.oculus.com/documentation/unity/unity-asw
* Video Lecture [Asynchronous Timewarp & Spacewarp for VR | Bonnie Mathew 2017](https://www.youtube.com/watch?v=gqVIJtRjtr8)
* [how to diagnose and fix common issues with ASW-enabled apps](https://github.com/oculus-samples/Unity-AppSpaceWarp)
