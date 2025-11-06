using UnityEngine;

#if UNITY_6000_0_OR_NEWER

using UnityEngine.XR.OpenXR;
using UnityEngine.XR.OpenXR.Features;

#endif

// only needed for Unity version 6000.1.13f1 and above , see : 
// https://docs.unity3d.com/Packages/com.unity.xr.openxr@1.15/manual/features/spacewarp/spacewarp-workflow.html

public class OpenXRAppSW : MonoBehaviour
{

    // Project Settings -> XR Plug-in Management -> OpenXR

#if UNITY_6000_0_OR_NEWER

    private SpaceWarpFeature feature = null;

    public SpaceWarpFeature GetSWF()
    {
        if( feature == null)
            feature = OpenXRSettings.Instance.GetFeature<SpaceWarpFeature>();
        return feature;
    }

    public bool CheckIfEnabled() => GetSWF()?.enabled ?? false;


    public void SetSpaceWarp( bool value )
    {
        SpaceWarpFeature.SetSpaceWarp(value);

        featureIsRunning = CheckIfEnabled();
    }

    public void ToggleSpaceWarp()
    {
        Debug.Log($"OpenXR : Attempting to set AppSW = {!featureIsRunning}");

        SetSpaceWarp( !featureIsRunning );

        Debug.Log($"OpenXR : AppSW is active = { featureIsRunning }");
    }

    void Start()
    {
        if( enableOnStart ) SetSpaceWarp( true );
    }

    
    void Update()
    {
        // Update SpaceWarp with camera position and rotation
        // if it is enabled in Project Settings.
        // Note, Depending on the headset, SpaceWarp may not need
        // to be updated with the main camera’s current position
        // or rotation. Refer to the headset’s specifications to
        // determine if it’s required. If the headset *does not*
        // require SpaceWarp to be updated with the main camera's
        // current position and rotation, then comment out the
        // following code.

        if ( featureIsRunning )
        {
            SpaceWarpFeature.SetAppSpacePosition( transform.position );
            SpaceWarpFeature.SetAppSpaceRotation( transform.rotation );
        }
    }

#else

    public void SetSpaceWarp(bool value) { Debug.LogWarning("Unsupported"); }
    public void ToggleSpaceWarp() { Debug.LogWarning("Unsupported"); }
    public bool CheckIfEnabled() => false;

#endif

    bool featureIsRunning = false;

    public bool enableOnStart = false;

}
