using UnityEngine;
using UnityEngine.XR;
using System.Collections.Generic;
using Device = UnityEngine.XR.InputDeviceCharacteristics;
using System.Linq;

public class MiniRig : MonoBehaviour
{
    List<XRInputSubsystem> subsystems = new List<XRInputSubsystem>();

    public TrackingOriginModeFlags trackingMode;

    void UpdateTracking()
    {        
        if( ! Application.isPlaying ) return;
        if( subsystems.Count == 0 ) SubsystemManager.GetInstances( subsystems );
        var first_subsystem = subsystems[ 0 ];
        if( subsystems.Count == 0 || first_subsystem == null ) return;

        // Those last two checks only needed when the app is existing , if removed there 
        // will be a null reference error - doesn't like its needed but its here 
        if( first_subsystem.running || first_subsystem.subsystemDescriptor != null ) return;
        
        if( first_subsystem.GetTrackingOriginMode() != trackingMode )
            first_subsystem.TrySetTrackingOriginMode( trackingMode );
    }

    public Transform head;
    public Transform handL;
    public Transform handR;

    const Device DeviceHead = Device.HeadMounted;
    const Device DeviceLeft = Device.Left | Device.Controller;
    const Device DeviceRight = Device.Right | Device.Controller;
    
    
    Dictionary<Device,List<InputDevice>> dict = new Dictionary<Device, List<InputDevice>>() 
    {
        { DeviceHead ,  new List<InputDevice>() },
        { DeviceLeft ,  new List<InputDevice>() },
        { DeviceRight , new List<InputDevice>() }
    };

    bool TryGet( Device device , out InputDevice input )
    {
        input = default;
        if( dict[ device ].Count == 0 ) 
        {
            InputDevices.GetDevicesWithCharacteristics( device, dict[ device ] );
            // var futures = new List<InputFeatureUsage>();
            // if( dict[ device ].Count > 0 && dict[ device ][ 0 ].TryGetFeatureUsages( futures ) )
            //     Debug.Log("Device " + device + " futures: " + string.Join(", ", futures.Select( x => x.name ) ) );
        }
        
        if( dict[ device ].Count > 0 ) input = dict[ device ][ 0 ];

        return dict[ device ].Count > 0;
    }

    void ApplyTransform( InputDevice D , Transform T )
    {
        if( T == null ) return;
        if( D.TryGetFeatureValue( CommonUsages.devicePosition, out Vector3 pos ) )      T.localPosition = pos;
        if( D.TryGetFeatureValue( CommonUsages.deviceRotation, out Quaternion rot ) )   T.localRotation = rot;
    }

    public event System.Action onLeftTrigger;       bool pressedLeft = false;
    public event System.Action onRightTrigger;      bool pressedRight = false;
    
    void UpdateDevices()
    {
        if( TryGet( DeviceHead, out InputDevice H ) )
        {
            ApplyTransform( H, head );
        }

        if( TryGet( DeviceLeft, out InputDevice L ) )
        {
            ApplyTransform( L, handL );

            // simple trigger event 
            if( L.TryGetFeatureValue( CommonUsages.triggerButton, out bool value ) ) {
                if( value && ! pressedLeft ) { pressedLeft = true; onLeftTrigger?.Invoke(); }
                else pressedLeft = value;
            }
        }

        if( TryGet( DeviceRight, out InputDevice R ) )
        {
            ApplyTransform( R, handR );

            // simple trigger event 
            if( R.TryGetFeatureValue( CommonUsages.triggerButton, out bool value ) ) {
                if( value && ! pressedRight ) { pressedRight = true; onRightTrigger?.Invoke(); }
                else pressedRight = value;
            }
        }
    }

    void OnEnable() => Application.onBeforeRender += Update;
    void OnDisable() => Application.onBeforeRender -= Update;

    void Update()
    {
        UpdateTracking();
        UpdateDevices();
    }
}