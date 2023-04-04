
using UnityEngine;
using TMPro;
using UnityEngine.XR;
using System.Collections.Generic;
using URP = UnityEngine.Rendering.Universal.UniversalRenderPipelineAsset;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class GiveMeMoreFPS : MonoBehaviour
{
    // on screen debug log variables 
    List<string> logLines = new List<string>();
    public Camera mainCamera;
    public TextMeshPro logField;

    public TextMeshPro fpsLabel;


    // quality parameters 

    
    // 0 = Off, 1 = Low, 2 = Medium, 3 = High, 4 = High Top
    // Read more at : https://developer.oculus.com/documentation/unity/unity-fixed-foveated-rendering/
    [Range(0,4)] int FoveationLevel = 0;
    [Range(0.2f,1f)] float resolutionScale = 1f;
    float DisplayFrequency = 120;
    bool spaceWrap = true;
    MsaaQuality MSAA; // 1,2,4,8
    float renderViewportScale;
    // [Range(0,4)] float fovZoomFactor;
    bool spaceWrapState = false;
    URP AssetURP => (URP) GraphicsSettings.currentRenderPipeline ;
    DepthTextureMode dpethOriginal;
    const DepthTextureMode MotionVectors = DepthTextureMode.MotionVectors | DepthTextureMode.Depth;
    
    bool toggleMotionVectors = false;
    bool tooglePostProcessing = false;
    bool toggleShadows = false;

    int AAQuality = 0;


    // Button click event handle 


    public void OnButtonPress( MiniButton button )
    {
        switch( button.valueStr )
        {
            case "FFR" : FoveationLevel = button.valueInt;              break;
            case "RES" : resolutionScale = button.valueInt / 100f;      break;
            case "HZ"  : DisplayFrequency = button.valueInt;            break;
            case "ASW" : spaceWrap = ! spaceWrap;                       break;
            case "MSA" : MSAA = (MsaaQuality) button.valueInt;          break; 
            case "AAQ" : AAQuality = button.valueInt;                   break;
            // case "FOV" : fovZoomFactor = button.data / 100f;        break;
            case "RVP" : renderViewportScale = button.valueInt / 100f;  break;
            case "TMV" : toggleMotionVectors = true;                    break;
            case "TPP" : tooglePostProcessing = true;                   break;
            case "TSD" : toggleShadows = true;                          break;
        }

        if( Application.platform != RuntimePlatform.Android )
        {
            switch( button.valueStr )
            {
                case "FFR" : case "ASW" : 
                Debug.LogWarning("Operation only supported on android devices");
                break;
            }
        }
    }

    void Awake()
    {
        // Show debug log message on the wall 

        Application.logMessageReceived += ( a, b, c ) => {
            if( c == LogType.Warning ) a = $"<color=\"yellow\">{a}</color>";
            else if( c != LogType.Log )a = $"<color=\"red\">{a}</color>";
            logLines.Add( a );
            logField.text = string.Join("\n", logLines);logField.ForceMeshUpdate();
            if( logField.isTextTruncated )
            for( var i = 0; i < 5 ; ++i ) {
                logField.text = string.Join("\n", logLines);logField.ForceMeshUpdate();
                if( logField.isTextTruncated && logLines.Count > 0 )
                    logLines.RemoveAt( 0 );
            }
        };
    }

    void Start()
    {
        Debug.Log("Build : URP v12 - April 18 , 2022");

        // Get default values 
        
        resolutionScale = XRSettings.eyeTextureResolutionScale;
        renderViewportScale = XRSettings.renderViewportScale;
        // fovZoomFactor = XRDevice.fovZoomFactor;
        dpethOriginal = mainCamera.depthTextureMode;
        // cam = Camera.main; if( cam ) cam.stereoTargetEye = StereoTargetEyeMask.None;
        MSAA = (MsaaQuality) AssetURP.msaaSampleCount;

        var CD = mainCamera.GetUniversalAdditionalCameraData();

        AAQuality = ( (int) CD.antialiasing) * 10 + ( (int) CD.antialiasingQuality );

        // Quest values 

        if ( OculusNative.IsSupported )
        {
            if( OculusNative.TryGetFFR( out OculusNative.FFR ffr ) )
                FoveationLevel = ( int ) ffr;

            if( Unity.XR.Oculus.Performance.TryGetDisplayRefreshRate( out float rate ) )
                    DisplayFrequency = rate;
        }
    }

    int cooldown = 0;

    void Update()
    {
        if( cooldown-- > 0 ) return; cooldown = 10; // run update once per 10 frames 

        if( renderViewportScale != XRSettings.renderViewportScale ) //! RVP
        {
            Debug.Log($"Render View Port Scale: {XRSettings.renderViewportScale:N3} -> {renderViewportScale:N3}");
            XRSettings.renderViewportScale = renderViewportScale;
            renderViewportScale = XRSettings.renderViewportScale;
        }

        // if( fovZoomFactor != XRDevice.fovZoomFactor )
        // {
        //     // if( cam ) cam.fieldOfView = 60 * fovZoomFactor;

        //     XRDevice.fovZoomFactor = fovZoomFactor;
        //     Debug.Log("FOV Zoom Factor set = " + fovZoomFactor.ToString("N2") );
        // }

        if( XRSettings.eyeTextureResolutionScale != resolutionScale ) //! RES 
        {
            XRSettings.eyeTextureResolutionScale = resolutionScale;
            AssetURP.renderScale = resolutionScale;
            Debug.Log("Resolution Scale set = " + AssetURP.renderScale.ToString("N2") );
        }

        if( (int) MSAA != AssetURP.msaaSampleCount ) //! MSSA
        {
            AssetURP.msaaSampleCount = (int) MSAA;
            Debug.Log("MSAA set = " + (MsaaQuality) AssetURP.msaaSampleCount );
        }

        if( OculusNative.IsSupported ) //! FFR
        {
            var currentFFR = ( OculusNative.FFR ) FoveationLevel;   

            if( OculusNative.TryGetFFR( out OculusNative.FFR ffr1 ) && ffr1 != currentFFR )
            {
                // this doesn't work .. 
                // Unity.XR.Oculus.Utils.EnableDynamicFFR(FoveationLevel > 0);
                // Unity.XR.Oculus.Utils.SetFoveationLevel(FoveationLevel);
                // Debug.Log("GetFoveationLevel set = " + Unity.XR.Oculus.Utils.GetFoveationLevel() );

                if( OculusNative.TrySetFFR( ( OculusNative.FFR ) FoveationLevel ) )
                {
                    if( OculusNative.TryGetFFR( out OculusNative.FFR ffr2 ) )
                    {
                        Debug.Log("GetFoveationLevel set = " + ffr2 );
                    }
                }

                // this seems to work , also checkout : OVRManager.fixedFoveatedRenderingSupported
                // OVRManager.fixedFoveatedRenderingLevel = ( OVRManager.FixedFoveatedRenderingLevel ) FoveationLevel;
                // Debug.Log("GetFoveationLevel set = " + OVRManager.fixedFoveatedRenderingLevel );
            }
        }


        if( OculusNative.IsSupported ) //! HZ
        {
            if( Unity.XR.Oculus.Performance.TryGetDisplayRefreshRate( out float rate ) )
            {
                if( rate != DisplayFrequency )
                {
                    if( Unity.XR.Oculus.Performance.TryGetAvailableDisplayRefreshRates( out float[] rates ) )
                        Debug.Log("Available refresh rates: " + string.Join( ", " , rates ) );

                    if( ! Unity.XR.Oculus.Performance.TrySetDisplayRefreshRate( DisplayFrequency ) )
                    {
                        Debug.LogWarning($"failed to set freq to {DisplayFrequency.ToString("N1")} current {rate.ToString("N1")}");
                        DisplayFrequency = rate;
                    }
                    else 
                    {
                        // if( Oculus.Performance.TryGetDisplayRefreshRate( out float rateNew ) )    
                        //     Debug.Log("DisplayRefreshRate set = " + rateNew.ToString("N1") );
                        // else 
                            Debug.Log("DisplayRefreshRate set = " + DisplayFrequency.ToString("N1") );
                    }

                } 
            }
        }
        else //! FPS
        {
            if( Application.targetFrameRate != (int) DisplayFrequency )
            {
                Application.targetFrameRate = ( int ) DisplayFrequency;
                Debug.Log("Frame Rate set = " + Application.targetFrameRate );
            }
        }

        if( OculusNative.IsSupported ) //! ASW
        {
            if( spaceWrap != spaceWrapState )
            {
                spaceWrapState = spaceWrap;
                OculusNative.SetSpaceWrap( spaceWrapState );
                // OculusXRPlugin.SetSpaceWarp( spaceWrapState ? OVRPlugin.Bool.True : OVRPlugin.Bool.False );
                // OVRManager.SetSpaceWarp( spaceWrapState );
                Debug.Log("SpaceWrap set = " + ( spaceWrapState ? "T" : "F" ) );

                Debug.Log("Current camera depth texture mode: " + mainCamera.depthTextureMode );

                mainCamera.depthTextureMode = spaceWrap ? dpethOriginal | MotionVectors : dpethOriginal;

                Debug.Log("New camera depth texture mode: " + mainCamera.depthTextureMode );
            }
        }

        if( toggleMotionVectors ) //! TMV
        {
            toggleMotionVectors = false;

            mainCamera.depthTextureMode = mainCamera.depthTextureMode == dpethOriginal ? dpethOriginal | MotionVectors : dpethOriginal ;

            Debug.Log("Camera depth texture mode set to : " + mainCamera.depthTextureMode);
        }

        var camera_data = mainCamera.GetUniversalAdditionalCameraData();

        if ( tooglePostProcessing ) //! TPP
        {
            tooglePostProcessing = false;

            camera_data.renderPostProcessing = ! camera_data.renderPostProcessing;

            Debug.Log("Camera post processing set to : " + camera_data.renderPostProcessing );
        }

        if( toggleShadows ) //! TSD
        {
            toggleShadows = false;

            camera_data.renderShadows = ! camera_data.renderShadows;

            Debug.Log("Camera render shadows set to : " + camera_data.renderShadows );
        }

        //! AAQ

        var current_AA_m = ( (int) camera_data.antialiasing ) * 10 ;
        var current_AA_q = (int) camera_data.antialiasingQuality;

        var new_AA_m = Mathf.FloorToInt( AAQuality / 10f ) * 10;
        var new_AA_q = ( AAQuality - new_AA_m );

        if ( current_AA_m != new_AA_m || ( new_AA_m == 20 && current_AA_q != new_AA_q ) ) 
        {
            camera_data.antialiasingQuality = ( AntialiasingQuality ) new_AA_q ;
            camera_data.antialiasing = ( AntialiasingMode ) ( new_AA_m / 10 );

            Debug.Log($"New AA : {camera_data.antialiasing} {camera_data.antialiasingQuality}"); // [{current_AA_m},{current_AA_q}/{new_AA_m},{new_AA_q}]");
        }

    }

    float fps_time = 0;
    float fps_value = 0;

    int fps_count = 0;

    void LateUpdate()
    {
        fps_count ++ ;

        var dt = Time.deltaTime;

        fps_time += dt;

        if( fps_time >= 1 )
        {
            fps_value = ( fps_count / fps_time );

            fps_time = fps_count = 0;
        }

        fpsLabel.text = fps_value.ToString("N1") + " FPS"
            + "\n" + ( spaceWrap ? fps_value * 2 : fps_value ).ToString("N0") + " Hz"
            + "\n" + ( (int) ( dt * 1000 ) ) + " ms";
    }
}
