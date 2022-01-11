# Application Spacewarp Template Project
  
  
Project is preconfigured with ASW for the Oculus Quest 1 &amp; 2 using a custom render pipeline 

![screenshot](https://raw.githubusercontent.com/nukadelic/unity-application-spacewarp/master/Img/screenshot.png)

### APK File 

Try the build : https://github.com/nukadelic/unity-application-spacewarp/releases/tag/apk

### Installation

Make sure you have Unity 2020.3.22 installed or newer , in this case im using the latest LTS version of the editor which is **2020.3.25** if you wish to download the exact project version to be sure all the versions exactly match , open in new tab the following : `unityhub://2020.3.25f1/9b9180224418` this should launch unity hub with a new editor installation or go to [Unity Archive](https://unity3d.com/get-unity/download/archive) and search for 2020.3.25f1 and make sure to install **Android Build Support**

Clone the project ( or download the zip ) and in unity Hub add the project folder & select Android target platform  
  
![screenshot](https://raw.githubusercontent.com/nukadelic/unity-application-spacewarp/master/Img/hub.png)

Done, now build and install the apk.

**Notes** 
* 120Hz is still in experimental stage , to get it working enable it inside the Quest headset in the settings under experimental features. 
* Unsupported materials will result in tear , make sure to edit your shaders to support motion vectors or make them in shader graph.
* [Fixed](https://github.com/Oculus-VR/Unity-Graphics/issues/3) <s>Lit shaders will cause wild tearing ( see below ) </s>

### Config 

* The scene in the screen shot can be found in `Assets/Example/Scenes/SampleScene.unity`
* The project already contains the oculus plugin version 35.0 ( only contains the VR folder as per the minimal requirments )
* All the project settings are allready setup to be Spacewarp ready 
* Shader graph seems to be working as expected 
* Additional Project settings 
  * Linear , Vulkan 
  * Android 8.0 'Oreo' ( API Level 26 ) 
  * IL2CPP , .NET 4.x  
  * Texture Compression : ASTC 
* The following packages are already included in the project _manifest.json_
``` 
+ https://github.com/nukadelic/asw-shader-graph.git
+ https://github.com/nukadelic/asw-render-pipelines-core.git
+ https://github.com/nukadelic/asw-visual-effect-graph.git
+ https://github.com/nukadelic/asw-render-pipelines-universal.git
```

### Building 

For faster builds you can swap to the Mono Scripting backend in `Project Settings > Player > Other Settings > Configuration > Scripting Backend` , but it is required to have IL2CPP when publishing the application on the store 
  
### Links 

* UPM packages are from the oculus [ASW Render pipeline](https://github.com/Oculus-VR/Unity-Graphics/tree/2020.3/oculus-app-spacewarp) , from the following branch: [50a6799](https://github.com/Oculus-VR/Unity-Graphics/tree/50a6799b67a17ed743ab121147eabcdfc8a131a4), view point of fork from Unity URP 10.x to oculus-asw here : [git-compare](https://github.com/Unity-Technologies/Graphics/compare/10.x.x/release...Oculus-VR:2020.3/oculus-app-spacewarp)
* ASW Developer Guide : https://developer.oculus.com/documentation/unity/unity-asw
* Video Lecture [Asynchronous Timewarp & Spacewarp for VR | Bonnie Mathew 2017](https://www.youtube.com/watch?v=gqVIJtRjtr8)

### Donate 

Feed me a beer (૭ ◉༬◉)૭⁾⁾⁾⁾ [![Donate](https://img.shields.io/badge/Donate-PayPal-green.svg)](https://www.paypal.me/wad1m)
