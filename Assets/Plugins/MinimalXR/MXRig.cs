namespace nkd.xr
{
    using UnityEngine;
    using UnityEngine.XR;
    using System.Collections.Generic;
    using Device = UnityEngine.XR.InputDeviceCharacteristics;
    public class MXRig : MonoBehaviour
    {
        #region Tracking

        [HideInInspector] public List<XRInputSubsystem> subsystems = new List<XRInputSubsystem>();

        public TrackingOriginModeFlags trackingMode;

        public Transform trackHead;
        public Transform trackLeft;
        public Transform trackRight;

        bool TryGetFirstSubsystem( out XRInputSubsystem subsystem )
        {
            subsystem = null;

            if (subsystems.Count == 0 || subsystems[0] == null) return false;

            subsystem = subsystems[0]; return true;
        }

        void UpdateTracking()
        {
            if (!Application.isPlaying) return;

    #if UNITY_6000_0_OR_NEWER
            if (subsystems.Count == 0) SubsystemManager.GetSubsystems(subsystems);
    #else   
            if (subsystems.Count == 0) SubsystemManager.GetInstances(subsystems);
    #endif

            if( ! TryGetFirstSubsystem( out XRInputSubsystem first_subsystem ) ) return;

            // This check below is only needed when the app is existing , when removed there 
            // will be a null reference error - doesn't seem like its needed but its here 
            if (first_subsystem.running || first_subsystem.subsystemDescriptor != null) return;

            var active = first_subsystem.GetTrackingOriginMode();

            if (active == trackingMode) return;
            if (first_subsystem.TrySetTrackingOriginMode(trackingMode)) return;

            Debug.LogWarning($"Cannot set tracking origin mode to {trackingMode}, default restored to {active}");
            trackingMode = active;
        }

        #endregion


        #region Devices 

        Dictionary<uint, List<InputDevice>> devices = new Dictionary<uint, List<InputDevice>>()
        {
            { (uint) MXDevice.HEAD  ,  new List<InputDevice>() },
            { (uint) MXDevice.LEFT  ,  new List<InputDevice>() },
            { (uint) MXDevice.RIGHT ,  new List<InputDevice>() }
        };

        public bool TryGet( MXDevice device , out InputDevice input )
        {
            return TryGet( (Device) device, out input );
        }

        bool TryGet(Device device, out InputDevice input)
        {
            var device_value = (uint) device;

            input = default;
            if (devices[device_value].Count == 0)
            {
                InputDevices.GetDevicesWithCharacteristics(device, devices[device_value]);
                // var futures = new List<InputFeatureUsage>();
                // if( dict[ device ].Count > 0 && dict[ device ][ 0 ].TryGetFeatureUsages( futures ) )
                //     Debug.Log("Device " + device + " futures: " + string.Join(", ", futures.Select( x => x.name ) ) );
            }

            if (devices[device_value].Count > 0) input = devices[device_value][0];

            return devices[device_value].Count > 0;
        }

        #endregion

        void ApplyTransform(InputDevice D, Transform T)
        {
            if (T == null) return;

            if (D.TryGetFeatureValue(CommonUsages.devicePosition, out Vector3 pos))

                if (pos != Vector3.zero) T.localPosition = pos;

            if (D.TryGetFeatureValue(CommonUsages.deviceRotation, out Quaternion rot))

                if (rot != Quaternion.identity) T.localRotation = rot;
        }

    
        void UpdateDevices()
        {
            if( TryGet( MXDevice.HEAD, out InputDevice H ) )  ApplyTransform( H, trackHead );
            if( TryGet( MXDevice.LEFT, out InputDevice L ) )  ApplyTransform( L, trackLeft );
            if( TryGet( MXDevice.RIGHT, out InputDevice R ) ) ApplyTransform( R, trackRight );
        }

        void OnEnable() => Application.onBeforeRender += Step;
        void OnDisable() => Application.onBeforeRender -= Step;

        public event System.Action OnStep;

        void Step()
        {
            UpdateTracking();
            UpdateDevices();
        }

        void Update()
        {
            Step();
        }
    }
}