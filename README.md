# Unity Application Spacewarp
### Sample project

Project is preconfigured with ASW for the Oculus Quest 1 &amp; 2 using a custom render pipeline 

![screenshot](https://raw.githubusercontent.com/nukadelic/unity-application-spacewarp/master/Img/screenshot.png)

### Installation

Make sure you have Unity 2020.3.22 installed or newer , in this case im using the latest LTS version of the editor which is **2020.3.25** if you wish to download the exact project version to be sure all the versions exactly match , open in new tab the following : `unityhub://2020.3.25f1/9b9180224418` this should launch unity hub with a new editor installation or go to [Unity Archive](https://unity3d.com/get-unity/download/archive) and search for 2020.3.25f1 and make sure to install **Android Build Support**

Clone the project ( or download the zip ) and in unity Hub add the project folder & select Android target platform
![screenshot](https://raw.githubusercontent.com/nukadelic/unity-application-spacewarp/master/Img/hub.png)

Done, now build and install the apk.

**Notes** 
* For some reason I couldn't test the project in play mode on the headset , it shows the opening app but never does launch it. If you know how to fix this lemme know. 
* 120Hz is still in experimental stage , to get it working enable it inside the Quest headset in the settings under experimental features. 
* Unsupported materials will result in tear , make sure to edit your shaders to support motion vectors or make them in shader graph.

### Config 

* The project already contains the oculus plugin version 35.0 ( only contains the VR folder as per the minimal requirments )
* All the project settings are allready setup to be Spacewarp ready 
* Shader graph seems to be working as expected 
* Additional Project settings 
  * Linear , Vulkan 
  * Android 8.0 'Oreo' ( API Level 26 ) 
  * IL2CPP , .NET 4.x  
* The following packages are already included in the project _manifest.json_
``` 
+ https://github.com/nukadelic/asw-shader-graph.git
+ https://github.com/nukadelic/asw-render-pipelines-core.git
+ https://github.com/nukadelic/asw-visual-effect-graph.git
+ https://github.com/nukadelic/asw-render-pipelines-universal.git
```

### Building 

For faster builds you can swap to the Mono Scripting backend in `Project Settings > Player > Other Settings > Configuration > Scripting Backend` , but it is required to have IL2CPP when publishing the application on the store 

### Donate 

Feed me a beer (૭ ◉༬◉)૭⁾⁾⁾⁾ [![Donate](https://img.shields.io/badge/Donate-PayPal-green.svg)](https://www.paypal.me/wad1m)
