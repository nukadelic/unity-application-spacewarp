# Unity Application Spacewarp
### Sample project

Project is preconfigured with ASW for the Oculus Quest 1 &amp; 2 using a custom render pipeline 

### Installation

Make sure you have Unity 2020.3.22 installed or newer , in this case im using the latest LTS version of the editor which is **2020.3.25** if you wish to download the exact project version to be sure all the versions exactly match , open in new tab the following : `unityhub://2020.3.25f1/9b9180224418` this should launch unity hub with a new editor installation or go to [Unity Archive](https://unity3d.com/get-unity/download/archive) and search for 2020.3.25f1 and make sure to install **Android Build Support**



### Config 

* The project already contains the oculus plugin version 35.0 
* All the project settings are allready setup to be space swap ready 
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
