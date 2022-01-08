
using UnityEngine;
using TMPro;
using UnityEngine.XR;
#if UNITY_ANDROID
using Oculus = Unity.XR.Oculus;
#endif
using System.Collections.Generic;
using URP = UnityEngine.Rendering.Universal.UniversalRenderPipelineAsset;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class GiveMeMoreFPS : MonoBehaviour
{
    // on screen debug log variables 
    List<string> logLines = new List<string>();
    public TextMeshPro label;


    // quality parameters 

    
    // 0 = Off, 1 = Low, 2 = Medium, 3 = High, 4 = High Top
    // Read more at : https://developer.oculus.com/documentation/unity/unity-fixed-foveated-rendering/
    [Range(0,4)] public int FoveationLevel = 0;
    [Range(0.2f,1f)] public float resolutionScale = 1f;
    public float DisplayFrequency = 72;
    public bool spaceWrap = false;
    public MsaaQuality MSAA; // 1,2,4,8
    public float renderViewportScale;
    // [Range(0,4)] public float fovZoomFactor;
    bool spaceWrapState = false;
    public URP AssetURP => (URP) GraphicsSettings.currentRenderPipeline ;
    Camera cam;

    // Button click event handle 


    public void OnButtonPress( MiniButton button )
    {
        switch( button.type )
        {
            case "FFR" : FoveationLevel = button.data;              break;
            case "RES" : resolutionScale = button.data / 100f;      break;
            case "HZ"  : DisplayFrequency = button.data;            break;
            case "ASW" : spaceWrap = ! spaceWrap;                   break;
            case "MSA" : MSAA = (MsaaQuality) button.data;          break; 
         // case "FOV" : fovZoomFactor = button.data / 100f;        break;
            case "RVP" : renderViewportScale = button.data / 100f;  break;
        }

        if( Application.platform != RuntimePlatform.Android )
        {
            switch( button.type )
            {
                case "FFR" : case "ASW" : case "HZ" :
                Debug.LogWarning("Operation only supported on android devices");
                break;
            }
        }
    }

    void Start()
    {

        // Show debug log message on the wall 

        Application.logMessageReceived += ( a, b, c ) => {
            if( c == LogType.Warning ) a = $"<color=\"yellow\">{a}</color>";
            else if( c != LogType.Log )a = $"<color=\"red\">{a}</color>";
            logLines.Add( a );
            label.text = string.Join("\n", logLines);label.ForceMeshUpdate();
            if( label.isTextTruncated )
            for( var i = 0; i < 5 ; ++i ) {
                label.text = string.Join("\n", logLines);label.ForceMeshUpdate();
                if( label.isTextTruncated && logLines.Count > 0 )
                    logLines.RemoveAt( 0 );
            }
        };

        // Get default values 
        
        resolutionScale = XRSettings.eyeTextureResolutionScale;
        renderViewportScale = XRSettings.renderViewportScale;
        // fovZoomFactor = XRDevice.fovZoomFactor;
        // cam = Camera.main; if( cam ) cam.stereoTargetEye = StereoTargetEyeMask.None;
        MSAA = (MsaaQuality) AssetURP.msaaSampleCount;

        // Quest values 

        #if UNITY_ANDROID
        if( Application.platform == RuntimePlatform.Android )
        {
            FoveationLevel = Oculus.Utils.GetFoveationLevel();
            if( Oculus.Performance.TryGetDisplayRefreshRate( out float rate ) )
                DisplayFrequency = rate;
        }
        #endif


    }

    int cooldown = 0;

    void Update()
    {
        if( cooldown-- > 0 ) return; cooldown = 10; // run update once per 10 frames 

        if( renderViewportScale != XRSettings.renderViewportScale )
        {
            XRSettings.renderViewportScale = renderViewportScale;
            Debug.Log("Render View Port Scale set = " + renderViewportScale.ToString("N2") );
        }

        // if( fovZoomFactor != XRDevice.fovZoomFactor )
        // {
        //     // if( cam ) cam.fieldOfView = 60 * fovZoomFactor;

        //     XRDevice.fovZoomFactor = fovZoomFactor;
        //     Debug.Log("FOV Zoom Factor set = " + fovZoomFactor.ToString("N2") );
        // }

        if( XRSettings.eyeTextureResolutionScale != resolutionScale )
        {
            XRSettings.eyeTextureResolutionScale = resolutionScale;
            AssetURP.renderScale = resolutionScale;
            Debug.Log("Resolution Scale set = " + resolutionScale.ToString("N2") );
        }

        if( (int) MSAA != AssetURP.msaaSampleCount )
        {
            AssetURP.msaaSampleCount = (int) MSAA;
            Debug.Log("MSAA set = " + (MsaaQuality) AssetURP.msaaSampleCount );
        }

        #if UNITY_ANDROID

        if( Application.platform == RuntimePlatform.Android )
        {
            if( Oculus.Utils.GetFoveationLevel() != FoveationLevel )
            {
                // this doesn't work .. 
                // Unity.XR.Oculus.Utils.EnableDynamicFFR(FoveationLevel > 0);
                // Unity.XR.Oculus.Utils.SetFoveationLevel(FoveationLevel);
                // Debug.Log("GetFoveationLevel set = " + Unity.XR.Oculus.Utils.GetFoveationLevel() );

                // this seems to work , also checkout : OVRManager.fixedFoveatedRenderingSupported
                OVRManager.fixedFoveatedRenderingLevel = ( OVRManager.FixedFoveatedRenderingLevel ) FoveationLevel;
                Debug.Log("GetFoveationLevel set = " + OVRManager.fixedFoveatedRenderingLevel );
            }
        }

        if( Application.platform == RuntimePlatform.Android )
        {
            if( Oculus.Performance.TryGetDisplayRefreshRate( out float rate ) )
            {
                if( rate != DisplayFrequency )
                {
                    if( Oculus.Performance.TryGetAvailableDisplayRefreshRates( out float[] rates ) )
                        Debug.Log("Available refresh rates: " + string.Join( ", " , rates ) );

                    if( ! Oculus.Performance.TrySetDisplayRefreshRate( DisplayFrequency ) )
                    {
                        Debug.LogWarning($"failed to set freq to {DisplayFrequency.ToString("N1")} current {rate.ToString("N1")}");
                        DisplayFrequency = rate;
                    }

                    else if( Oculus.Performance.TryGetDisplayRefreshRate( out float rateNew ) )    
                        Debug.Log("DisplayRefreshRate set = " + rateNew.ToString("N1") );
                } 
            }
        }


        if( Application.platform == RuntimePlatform.Android )
        {
            if( spaceWrap != spaceWrapState )
            {
                spaceWrapState = spaceWrap;
                OculusXRPlugin.SetSpaceWarp( spaceWrapState ? OVRPlugin.Bool.True : OVRPlugin.Bool.False );
                // OVRManager.SetSpaceWarp( spaceWrapState );
                Debug.Log("SpaceWrap set = " + ( spaceWrap ? "T" : "F" ) );
            }
        }

        #endif
    }
}
