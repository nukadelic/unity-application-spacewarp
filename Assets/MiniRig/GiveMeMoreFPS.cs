
using UnityEngine;
using TMPro;
using UnityEngine.XR;
using Oculus = Unity.XR.Oculus;
using System.Collections.Generic;
using URP = UnityEngine.Rendering.Universal.UniversalRenderPipelineAsset;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class GiveMeMoreFPS : MonoBehaviour
{
    List<string> logLines = new List<string>();
    public TextMeshPro label;
    
    // 0 = Off, 1 = Low, 2 = Medium, 3 = High, 4 = High Top
    // Read more at : https://developer.oculus.com/documentation/unity/unity-fixed-foveated-rendering/
    [Range(0,4)] public int FoveationLevel = 0;
    [Range(0.2f,1f)] public float resolutionScale = 1f;
    public float DisplayFrequency = 72;
    public bool spaceWrap = false;
    public MsaaQuality MSAA; // 1,2,4,8

    bool spaceWrapState = false;
    
    public URP AssetURP => (URP) GraphicsSettings.currentRenderPipeline ;
    
    public void OnButtonPress( MiniButton button )
    {
        switch( button.type )
        {
            case "FFR" : FoveationLevel = button.data; break;
            case "RES" : resolutionScale = button.data / 100f; break;
            case "HZ"  : DisplayFrequency = button.data; break;
            case "ASW" : spaceWrap = ! spaceWrap; break;
            case "MSA" : MSAA = (MsaaQuality) button.data; break; 
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
        Application.logMessageReceived += ( a, b, c ) => {
            if( c == LogType.Warning ) a = $"<color=\"yellow\">{a}</color>";
            else if( c != LogType.Log )a = $"<color=\"red\">{a}</color>";
            logLines.Add( a );
            label.text = string.Join("\n", logLines);
            if( label.isTextTruncated )
            for( var i = 0; i < 5 ; ++i )
            {
                label.text = string.Join("\n", logLines);
                if( label.isTextTruncated && logLines.Count > 0 )
                    logLines.RemoveAt( 0 );
            }
        };

        MSAA = (MsaaQuality) AssetURP.msaaSampleCount;
        FoveationLevel = Oculus.Utils.GetFoveationLevel();
        resolutionScale = XRSettings.eyeTextureResolutionScale;
        if( Oculus.Performance.TryGetDisplayRefreshRate( out float rate ) )
            DisplayFrequency = rate;
    }

    void Update()
    {
        if( XRSettings.eyeTextureResolutionScale != resolutionScale )
        {
            XRSettings.eyeTextureResolutionScale = resolutionScale;
            AssetURP.renderScale = resolutionScale;

            Debug.Log("Resolution Scale set = " + resolutionScale.ToString("N2") );
        }

        if( Application.platform == RuntimePlatform.Android )
        {
            if( Oculus.Utils.GetFoveationLevel() != FoveationLevel )
            {
                // this doesn't work .. 
                // Oculus.Utils.EnableDynamicFFR(FoveationLevel > 0);
                // Oculus.Utils.SetFoveationLevel(FoveationLevel);
                // Debug.Log("GetFoveationLevel set = " + Oculus.Utils.GetFoveationLevel() );

                // this seems to work , also checkout : OVRManager.fixedFoveatedRenderingSupported
                OVRManager.fixedFoveatedRenderingLevel = ( OVRManager.FixedFoveatedRenderingLevel ) FoveationLevel;
                Debug.Log("GetFoveationLevel set = " + OVRManager.fixedFoveatedRenderingLevel );
            }
        }

        if( Oculus.Performance.TryGetDisplayRefreshRate( out float rate ) )
        {
            if( rate != DisplayFrequency )
            {
                if( ! Oculus.Performance.TrySetDisplayRefreshRate( DisplayFrequency ) )
                {
                    Debug.LogWarning($"failed to set freq to {DisplayFrequency.ToString("N1")} current {rate.ToString("N1")}");

                    if( Oculus.Performance.TryGetAvailableDisplayRefreshRates( out float[] rates ) )
                        Debug.Log("Available refresh rates: " + string.Join( ", " , rates ) );
                    
                    DisplayFrequency = rate;
                }
                else if( Oculus.Performance.TryGetDisplayRefreshRate( out float rateNew ) )
                {
                    Debug.Log("DisplayRefreshRate set = " + rateNew.ToString("N1") );
                }
            } 
        }

        if( Application.platform == RuntimePlatform.Android )
        {
            if( spaceWrap != spaceWrapState )
            {
                spaceWrapState = spaceWrap;

                // OVRManager.SetSpaceWarp( spaceWrapState );
                
                OculusXRPlugin.SetSpaceWarp( spaceWrapState ? OVRPlugin.Bool.True : OVRPlugin.Bool.False );

                Debug.Log("SpaceWrap set = " + ( spaceWrap ? "T" : "F" ) );
            }
        }


        if( (int) MSAA != AssetURP.msaaSampleCount )
        {
            AssetURP.msaaSampleCount = (int) MSAA;
            Debug.Log("MSAA set = " + (MsaaQuality) AssetURP.msaaSampleCount );
        }
    }
}
