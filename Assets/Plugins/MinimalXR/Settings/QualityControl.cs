namespace nkd.xr
{
    using UnityEngine;
    using TMPro;
    using UnityEngine.XR;
    using System.Collections.Generic;
    using URP = UnityEngine.Rendering.Universal.UniversalRenderPipelineAsset;
    using UnityEngine.Rendering;
    using UnityEngine.Rendering.Universal;
    //using static UnityEngine.Rendering.DebugUI;

    public class QualityControl : MonoBehaviour
    {
        public Camera mainCamera;
        public TextMeshPro logField;
        public TextMeshPro fpsLabel;

        void ValidateCamera()
        {
            if (mainCamera == null || !mainCamera.enabled || !mainCamera.gameObject.activeInHierarchy)
            {
                mainCamera = Camera.main ?? Camera.current ?? FindAnyObjectByType<Camera>();
            }
        }


        // quality parameters 

        [Header("Override graphics")]

        public bool overrideDefaults = false;

        // 0 = Off, 1 = Low, 2 = Medium, 3 = High, 4 = High Top
        // Read more at : https://developer.oculus.com/documentation/unity/unity-fixed-foveated-rendering/
        [Range(0, 4)] public int FoveationLevel = 0;
        [Range(0.2f, 1f)] public float resolutionScale = 1f;
        public float renderViewportScale = 1f;
        public float DisplayFrequency = 72;
        public bool spaceWrap = false;
        public MsaaQuality MSAA = MsaaQuality.Disabled; // 1,2,4,8


        // [Range(0,4)] float fovZoomFactor;

        int _AAQuality = 0;
        bool _toggleMotionVectors = false;
        bool _tooglePostProcessing = false;
        bool _toggleShadows = false;
        DepthTextureMode _dpethOriginal;
        bool _spaceWrapState = false;

        URP AssetURP => (URP)GraphicsSettings.currentRenderPipeline;

        const DepthTextureMode MotionVectors = DepthTextureMode.MotionVectors | DepthTextureMode.Depth;

        private void OnValidate()
        {
            ValidateCamera();

            if ( ! overrideDefaults )
            {
                var renderer = AssetURP;

                resolutionScale = renderer?.renderScale ?? XRSettings.eyeTextureResolutionScale;

                renderViewportScale = XRSettings.renderViewportScale;

                if (renderViewportScale == 0) renderViewportScale = 1;

                MSAA = renderer ? (MsaaQuality) renderer.msaaSampleCount : MsaaQuality.Disabled;
            }

        }

        // Button click event handle 


        public void OnButtonPress(string value)
        {
            bool split = value.Contains(",");

            var v = split ? value.Split(",") : new string[] { value };
            
            var v_integer = split ? int.Parse(v[1]) : 0;
            var v_float = v_integer / 100f;

            switch (v[0].ToUpper())
            {
                case "PAGE_T":
                    LogHistory.page_offset = 0;
                    return;
                case "PAGE_B":
                    LogHistory.page_offset = LogHistory.history?.Count ?? 1000;
                    return;
                case "PAGE":
                    if (v_integer == 1) PageUp();
                    if (v_integer == -1) PageDown();
                    return;
                case "PAGE_C":
                    ClearLog();
                    return;
            }

            switch (v[0].ToUpper())
            {
                case "FOG": RenderSettings.fog = !RenderSettings.fog; break;
                case "SKY": RenderSettings.skybox = RenderSettings.skybox != default_skybox ? default_skybox : null; break;
                case "FFR": FoveationLevel = v_integer; break;
                case "RES": resolutionScale = v_float; break;
                case "HZ": DisplayFrequency = v_integer; break;
                case "ASW": spaceWrap = !spaceWrap; break;
                case "MSA": MSAA = (MsaaQuality)v_integer; break;
                case "AAQ": _AAQuality = v_integer; break;
                // case "FOV" : fovZoomFactor = vf;        break;
                case "RVP": renderViewportScale = v_float; break;
                case "TMV": _toggleMotionVectors = true; break;
                case "TPP": _tooglePostProcessing = true; break;
                case "TSD": _toggleShadows = true; break;
            }

            if (!IsQuest)
            {
                switch (v[0])
                {
                    case "FFR":
                    case "ASW":
                        Debug.LogWarning("Operation only supported on android devices");
                        break;
                }
            }
        }

        bool IsQuest => OculusDLL.isSupportedPlatform && Application.platform == RuntimePlatform.Android;

        // [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterAssembliesLoaded)]

        Material default_skybox;

        void Awake()
        {
            if (!Application.isPlaying) return;

            default_skybox = RenderSettings.skybox;

            Application.logMessageReceived += MessageRecieved;

        }

        private void OnDestroy()
        {
            if (!Application.isPlaying) return;

            Application.logMessageReceived -= MessageRecieved;
        }

        static class LogHistory
        {
            public static List<string> buffer = new List<string>();

            public static List<string> history = new List<string>();

            public static int page_size = 0;

            public static int page_offset = 0;

            public static string GetLines()
            {
                page_offset = Mathf.Max(0, page_offset);

                int offset = history.Count - page_size;

                offset -= Mathf.FloorToInt(page_offset * (page_size * 0.7f)); // 70% page offset 

                offset = Mathf.Clamp(offset, 0, history.Count - page_size);

                var lines = history.GetRange(offset, page_size);

                return string.Join("\n", lines);
            }

            public static void Format(ref string message, LogType type)
            {
                if (type == LogType.Warning) message = $"<color=\"yellow\">{message}</color>";
                else if (type != LogType.Log) message = $"<color=\"red\">{message}</color>";
            }
        }

        void MessageRecieved(string a, string b, LogType c)
        {
            LogHistory.Format(ref a, c);
            LogHistory.buffer.Add(a);

            if (logField == null) return;

            foreach (var s in LogHistory.buffer)
            {
                LogHistory.history.Add(s);

                if (LogHistory.page_size <= 0)
                {
                    logField.text = string.Join("\n", LogHistory.history);
                    logField.ForceMeshUpdate();

                    // count page size until it becomes truncated 
                    LogHistory.page_size--;
                    if (logField.isTextTruncated)
                        LogHistory.page_size = -LogHistory.page_size - 1;
                }
                else
                {
                    logField.text = LogHistory.GetLines();
                    // logField.ForceMeshUpdate();
                }
            }

            LogHistory.buffer.Clear();
        }

        public void ClearLog()
        {
            LogHistory.page_offset = 0;
            LogHistory.buffer.Clear();
            LogHistory.history.Clear();
            logField.text = "";
        }

        public void PageUp()
        {
            if (LogHistory.page_size <= 0) return;
            LogHistory.page_offset++;
            logField.text = LogHistory.GetLines();
            // logField.ForceMeshUpdate();
        }
        public void PageDown()
        {
            if (LogHistory.page_size <= 0) return;
            LogHistory.page_offset--;
            logField.text = LogHistory.GetLines();
            // logField.ForceMeshUpdate();
        }


        void Start()
        {
            if (!Application.isPlaying) return;

            // Get default values 

            if (!overrideDefaults)
            {
                resolutionScale = XRSettings.eyeTextureResolutionScale;
                renderViewportScale = XRSettings.renderViewportScale;
                MSAA = (MsaaQuality)AssetURP.msaaSampleCount;
            }

            // fovZoomFactor = XRDevice.fovZoomFactor;
            _dpethOriginal = mainCamera.depthTextureMode;
            // cam = Camera.main; if( cam ) cam.stereoTargetEye = StereoTargetEyeMask.None;

            var CD = mainCamera.GetUniversalAdditionalCameraData();

            _AAQuality = ((int)CD.antialiasing) * 10 + ((int)CD.antialiasingQuality);

            // Quest values 

            if (IsQuest)
            {
                if (!overrideDefaults)
                {
                    if(OculusDLL.fixedFoveatedRenderingSupported)
                    {
                        FoveationLevel = (int) OculusDLL.foveatedRenderingLevel;
                    }

                    if (Unity.XR.Oculus.Performance.TryGetDisplayRefreshRate(out float rate))
                        DisplayFrequency = rate;
                }


            }
        }

        int cooldown = 0;
        void Update()
        {
            if (!Application.isPlaying) return;

            ValidateCamera();

            if (cooldown-- > 0) return; cooldown = 10; // run update once per 10 frames 

            if (renderViewportScale != XRSettings.renderViewportScale) //! RVP
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

            if (XRSettings.eyeTextureResolutionScale != resolutionScale) //! RES 
            {
                XRSettings.eyeTextureResolutionScale = resolutionScale;
                AssetURP.renderScale = resolutionScale;
                Debug.Log("Resolution Scale set = " + AssetURP.renderScale.ToString("N2"));
            }

            if ((int)MSAA != AssetURP.msaaSampleCount) //! MSSA
            {
                AssetURP.msaaSampleCount = (int)MSAA;
                Debug.Log("MSAA set = " + (MsaaQuality)AssetURP.msaaSampleCount);
            }
            
            if (IsQuest) //! FFR
            {
                var currentFFR = (OculusDLL.FoveatedRenderingLevel) FoveationLevel;

                if( currentFFR != OculusDLL.foveatedRenderingLevel )
                {
                    // this doesn't work .. 
                    // Unity.XR.Oculus.Utils.EnableDynamicFFR(FoveationLevel > 0);
                    // Unity.XR.Oculus.Utils.SetFoveationLevel(FoveationLevel);
                    // Debug.Log("GetFoveationLevel set = " + Unity.XR.Oculus.Utils.GetFoveationLevel() );

                    if( OculusDLL.OVRP_1_21_0.ovrp_SetTiledMultiResLevel(currentFFR) == OculusDLL.Result.Success )
                    {
                        Debug.Log("GetFoveationLevel set = " + OculusDLL.foveatedRenderingLevel );
                    }

                    // this seems to work , also checkout : OVRManager.fixedFoveatedRenderingSupported
                    // OVRManager.fixedFoveatedRenderingLevel = ( OVRManager.FixedFoveatedRenderingLevel ) FoveationLevel;
                    // Debug.Log("GetFoveationLevel set = " + OVRManager.fixedFoveatedRenderingLevel );
                }
            }


            if (IsQuest) //! HZ
            {
                if (Unity.XR.Oculus.Performance.TryGetDisplayRefreshRate(out float rate))
                {
                    if (rate != DisplayFrequency)
                    {
                        if (Unity.XR.Oculus.Performance.TryGetAvailableDisplayRefreshRates(out float[] rates))
                            Debug.Log("Available refresh rates: " + string.Join(", ", rates));

                        if (!Unity.XR.Oculus.Performance.TrySetDisplayRefreshRate(DisplayFrequency))
                        {
                            Debug.LogWarning($"failed to set freq to {DisplayFrequency.ToString("N1")} current {rate.ToString("N1")}");
                            DisplayFrequency = rate;
                        }
                        else
                        {
                            // if( Oculus.Performance.TryGetDisplayRefreshRate( out float rateNew ) )    
                            //     Debug.Log("DisplayRefreshRate set = " + rateNew.ToString("N1") );
                            // else 
                            Debug.Log("DisplayRefreshRate set = " + DisplayFrequency.ToString("N1"));
                        }

                    }
                }
            }
            else //! FPS
            {
                if (Application.targetFrameRate != (int) DisplayFrequency)
                {
                    Application.targetFrameRate = (int)DisplayFrequency;
                    Debug.Log("Frame Rate set = " + Application.targetFrameRate);
                }
            }

            if (IsQuest) //! ASW
            {
                if (spaceWrap != _spaceWrapState)
                {
                    Debug.Log("Current camera depth texture mode: " + mainCamera.depthTextureMode);

                    mainCamera.depthTextureMode = spaceWrap ? _dpethOriginal | MotionVectors : _dpethOriginal;

                    Debug.Log("New camera depth texture mode: " + mainCamera.depthTextureMode);

                    _spaceWrapState = spaceWrap;

                    OculusDLL.SetSpaceWarp( _spaceWrapState ? 1 : 0 );

                    // OculusXRPlugin.SetSpaceWarp( spaceWrapState ? OVRPlugin.Bool.True : OVRPlugin.Bool.False );
                    // OVRManager.SetSpaceWarp( spaceWrapState );
                    Debug.Log("SpaceWrap set = " + (_spaceWrapState ? "T" : "F"));
                }
            }

            if (_toggleMotionVectors) //! TMV
            {
                _toggleMotionVectors = false;

                mainCamera.depthTextureMode = mainCamera.depthTextureMode == _dpethOriginal ? _dpethOriginal | MotionVectors : _dpethOriginal;

                Debug.Log("Camera depth texture mode set to : " + mainCamera.depthTextureMode);
            }

            var camera_data = mainCamera.GetUniversalAdditionalCameraData();

            if (_tooglePostProcessing) //! TPP
            {
                _tooglePostProcessing = false;

                camera_data.renderPostProcessing = !camera_data.renderPostProcessing;

                Debug.Log("Camera post processing set to : " + camera_data.renderPostProcessing);
            }

            if (_toggleShadows) //! TSD
            {
                _toggleShadows = false;

                camera_data.renderShadows = !camera_data.renderShadows;

                Debug.Log("Camera render shadows set to : " + camera_data.renderShadows);
            }

            //! AAQ

            var current_AA_m = ((int)camera_data.antialiasing) * 10;
            var current_AA_q = (int)camera_data.antialiasingQuality;

            var new_AA_m = Mathf.FloorToInt(_AAQuality / 10f) * 10;
            var new_AA_q = (_AAQuality - new_AA_m);

            if (current_AA_m != new_AA_m || (new_AA_m == 20 && current_AA_q != new_AA_q))
            {
                camera_data.antialiasingQuality = (AntialiasingQuality)new_AA_q;
                camera_data.antialiasing = (AntialiasingMode)(new_AA_m / 10);

                Debug.Log($"New AA : {camera_data.antialiasing} {camera_data.antialiasingQuality}"); // [{current_AA_m},{current_AA_q}/{new_AA_m},{new_AA_q}]");
            }

        }

        [HideInInspector] public float fps_time = 0;
        [HideInInspector] public float fps_value = 0;
        [HideInInspector] public int fps_dt = 0;

        int fps_count = 0;

        void LateUpdate()
        {
            if (!Application.isPlaying) return;

            fps_count++;

            var dt = Time.deltaTime;

            fps_time += dt;

            if (fps_time >= 1)
            {
                fps_value = (fps_count / fps_time);

                fps_time = fps_count = 0;
            }

            fps_dt = (int)(dt * 1000);

            fpsLabel.text = fps_value.ToString("N1") + " FPS"
                + "\n" + (spaceWrap ? fps_value * 2 : fps_value).ToString("N0") + " Hz"
                + "\n" + fps_dt + " ms";
        }
    }

}