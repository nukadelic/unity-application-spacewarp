namespace nkd.xr
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using UnityEngine.Events;
    using Device = UnityEngine.XR.InputDeviceCharacteristics;
    using UnityEngine.XR;
    using UnityEngine.XR.Hands;

    public class MXControllersConnect : MonoBehaviour
    {
        public static bool printLogs = true;

        #region Events 

        [System.Serializable] public struct UnityEvents
        {

            public event System.Action<TrackingEvent> OnTrackingChanged;
            
            public UnityEvent<TrackingEvent> OnTrackingChange;

            [Header("LEFT - CONTROLLER")]
            public UnityEvent OnLeftControllerEnabled;
            public UnityEvent OnLeftControllerDisabled;
            [Header("LEFT - HAND")]
            public UnityEvent OnLeftHandEnabled;
            public UnityEvent OnLeftHandDisabled;
            [Header("RIGHT - CONTROLLER")]
            public UnityEvent OnRightControllerEnabled;
            public UnityEvent OnRightControllerDisabled;
            [Header("RIGHT - HAND")]
            public UnityEvent OnRightHandEnabled;
            public UnityEvent OnRightHandDisabled;
            
            public void Emit( TrackingEvent te )
            {
                OnTrackingChange?.Invoke( te );

                OnTrackingChanged?.Invoke( te );

                if ( te.trackingPrev == ActiveTracking.Controller )
                {
                    if( te.handedness == Handedness.Left ) OnLeftControllerDisabled? .Invoke();
                    if( te.handedness == Handedness.Right) OnRightControllerDisabled?.Invoke();

                    if(printLogs) Debug.Log( "Controller disabled " + te.handedness);
                }
                if( te.trackingPrev == ActiveTracking.Hand )
                {
                    if( te.handedness == Handedness.Left ) OnLeftHandDisabled? .Invoke();
                    if( te.handedness == Handedness.Right) OnRightHandDisabled?.Invoke();

                    if (printLogs) Debug.Log("Hand disabled " + te.handedness);
                }
                if( te.tracking == ActiveTracking.Controller )
                {
                    if( te.handedness == Handedness.Left ) OnLeftControllerEnabled? .Invoke();
                    if( te.handedness == Handedness.Right) OnRightControllerEnabled?.Invoke();

                    if (printLogs) Debug.Log("Controller enabled " + te.handedness);
                }
                if( te.tracking == ActiveTracking.Hand )
                {
                    if( te.handedness == Handedness.Left ) OnLeftHandEnabled? .Invoke();
                    if( te.handedness == Handedness.Right) OnRightHandEnabled?.Invoke();

                    if (printLogs) Debug.Log("Hand enabled " + te.handedness);
                }
            }
        }

        public UnityEvents events;

        public struct TrackingEvent
        {
            public Handedness handedness;
            public ActiveTracking tracking;
            public ActiveTracking trackingPrev;
        }

        void EmitTrackingChange( TrackingEvent data ) 
        {
            events.Emit( data );
        }

        #endregion

        public float scanDelaySec = 0.15f;

        #region Scanner 

        Coroutine scanCoroutine;

        private void OnEnable()
        {
            scanCoroutine = StartCoroutine( ScanCoroutine() );
        }

        IEnumerator ScanCoroutine()
        {
            while( enabled )
            {
                yield return new WaitForEndOfFrame();

                if( scanDelaySec > 0 ) yield return new WaitForSeconds( scanDelaySec );

                ScanBothControllers();

                ScanBothHands();

                ScanActive();
            }
        }

        private void OnDisable()
        {
            if (handSystem != null)
            {
                handtracking_unsubscribe(handSystem);
                handSystem = null;
            }

            if (scanCoroutine != null)
            {
                StopCoroutine(scanCoroutine);
            }

            scanCoroutine = null;

        }

        #endregion

        public enum ActiveTracking { None , Controller , Hand }

        public ActiveTracking left = ActiveTracking.None;
        public ActiveTracking right = ActiveTracking.None;

        void ScanActive()
        {
            ActiveTracking prev_left = left;
            ActiveTracking prev_right = right;

            // ---

            if (handTrackingL) left = ActiveTracking.Hand;
            else if (controllerConnectedL) left = ActiveTracking.Controller;
            else left = ActiveTracking.None;

            if (handTrackingR) right = ActiveTracking.Hand;
            else if (controllerConnectedR) right = ActiveTracking.Controller;
            else right = ActiveTracking.None;

            // --- 

            if( prev_left != left )
            {
                EmitTrackingChange( new TrackingEvent { 
                    handedness = Handedness.Left , 
                    tracking = left , 
                    trackingPrev = prev_left } 
                );
            }

            if( prev_right != right )
            {
                EmitTrackingChange(new TrackingEvent {
                    handedness = Handedness.Right,
                    tracking = right,
                    trackingPrev = prev_right }
                );
            }
        }


        #region Controllers 

        [HideInInspector] public bool controllerConnectedL = false;
        [HideInInspector] public bool controllerConnectedR = false;

        InputDevice controllerL;
        InputDevice controllerR;
        
        void ScanBothControllers()
        {
            if( ScanController( ref controllerL, Device.Left | Device.Controller, controllerConnectedL ) )
            {
                controllerConnectedL = ! controllerConnectedL; // swap state 
            }

            if( ScanController( ref controllerR , Device.Right | Device.Controller, controllerConnectedR ) )
            {
                controllerConnectedR = ! controllerConnectedR;
            }
        }

        static readonly List<InputDevice> list_device = new List<InputDevice>();

        bool ScanController( ref InputDevice device , Device chars , bool state )
        {
            list_device.Clear();

            InputDevices.GetDevicesWithCharacteristics( chars, list_device );

            bool is_connected = list_device.Count > 0;

            if( list_device.Count > 0 )
            {
                device = list_device[ 0 ];
            }
            else device = default;
                
            return is_connected != state; // true if device state needs updating
        }

        #endregion

        #region Hands 


        XRHandSubsystem handSystem;

        [HideInInspector] public bool handTrackingL = false;
        [HideInInspector] public bool handTrackingR = false;

        //XRHand handXR_L, handXR_R;

        static readonly List<XRHandSubsystem> list_hss = new List<XRHandSubsystem>();

        void ScanBothHands()
        {
            if (handSystem == null || !handSystem.running)
            {
                if (handSystem != null) handtracking_unsubscribe(handSystem);

                SubsystemManager.GetSubsystems(list_hss);

                for (var i = 0; i < list_hss.Count; ++i)
                {
                    if (list_hss[i].running)
                    {
                        //handXR_L = hss[i].leftHand;
                        //handXR_R = hss[i].rightHand;

                        handtracking_subscribe(list_hss[i]);

                        handSystem = list_hss[i];

                        break;
                    }
                }
            }
        }

        void handtracking_subscribe(XRHandSubsystem hss)
        {
            hss.trackingAcquired += OnTrackingAcquired;
            hss.trackingLost += OnTrackingLost;
        }

        void handtracking_unsubscribe(XRHandSubsystem hss)
        {
            hss.trackingAcquired -= OnTrackingAcquired;
            hss.trackingLost -= OnTrackingLost;
        }

        void OnTrackingAcquired(XRHand x)
        {
            if (x.handedness == Handedness.Right) handTrackingR = true;
            if (x.handedness == Handedness.Left) handTrackingL = true;
        }
        void OnTrackingLost(XRHand x)
        {
            if (x.handedness == Handedness.Right) handTrackingR = false;
            if (x.handedness == Handedness.Left) handTrackingL = false;
        }

        #endregion
    }
}

