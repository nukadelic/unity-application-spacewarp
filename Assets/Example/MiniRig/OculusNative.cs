
#if !(UNITY_EDITOR_WIN || UNITY_STANDALONE_WIN || (UNITY_ANDROID && !UNITY_EDITOR))
#define OVRPLUGIN_UNSUPPORTED_PLATFORM
#endif

using System;
using System.Runtime.InteropServices;

public static class OculusNative
{
    public enum FFR { Off = 0, Low = 1, Medium = 2, High = 3, HighTop = 4, EnumSize = 0x7FFFFFFF }

    #if OVRPLUGIN_UNSUPPORTED_PLATFORM

        public static bool IsSupported => false;
        static bool ffrSupprted => false;
        public static bool TryGetFFR( out FFR value ) { value = FFR.Off; return false; }
        public static bool TrySetFFR( FFR value ) => false;
        public static void SetSpaceWrap( bool value ) {}

    #else 
        public static bool IsSupported => UnityEngine.Application.platform == UnityEngine.RuntimePlatform.Android;
        static bool ffrSupprted => 0 == DLL.ovrp_GetTiledMultiResSupported( out int supprted ) ? supprted == 1 : false;

        public static bool TryGetFFR( out FFR value )
        {
            if( ! ffrSupprted ) { value = default; return false; }
            return DLL.ovrp_GetTiledMultiResLevel( out value ) == 0;
        }
        public static bool TrySetFFR( FFR value )
        {
            if( ! ffrSupprted ) return false;
            return DLL.ovrp_SetTiledMultiResLevel( value ) == 0;
        }
        public static void SetSpaceWrap( bool value ) => DLL.SetSpaceWarp( value ? 1 : 0 );

        static class DLL
        {
            [DllImport("OVRPlugin", CallingConvention = CallingConvention.Cdecl)]
            public static extern int ovrp_GetTiledMultiResSupported(out int foveationSupported);

            [DllImport("OVRPlugin", CallingConvention = CallingConvention.Cdecl)]
            public static extern int ovrp_GetTiledMultiResLevel(out FFR level);

            [DllImport("OVRPlugin", CallingConvention = CallingConvention.Cdecl)]
            public static extern int ovrp_SetTiledMultiResLevel(FFR level);

            [DllImport("OVRPlugin", CallingConvention = CallingConvention.Cdecl)]
            public static extern int ovrp_GetGPUUtilSupported(out int gpuUtilSupported);

            [DllImport("OVRPlugin", CallingConvention = CallingConvention.Cdecl)]
            public static extern int ovrp_GetGPUUtilLevel(out float gpuUtil);

            [DllImport("OVRPlugin", CallingConvention = CallingConvention.Cdecl)]
            public static extern int ovrp_GetSystemDisplayFrequency2(out float systemDisplayFrequency);

            [DllImport("OVRPlugin", CallingConvention = CallingConvention.Cdecl)]
            public static extern int ovrp_GetSystemDisplayAvailableFrequencies(IntPtr systemDisplayAvailableFrequencies, ref int numFrequencies);

            [DllImport("OVRPlugin", CallingConvention = CallingConvention.Cdecl)]
            public static extern int ovrp_SetSystemDisplayFrequency(float requestedFrequency);

            [DllImport("OVRPlugin", CallingConvention = CallingConvention.Cdecl)]
            public static extern int ovrp_GetAppAsymmetricFov(out int useAsymmetricFov);

            [DllImport("OculusXRPlugin")]
            public static extern void SetColorScale(float x, float y, float z, float w);

            [DllImport("OculusXRPlugin")]
            public static extern void SetColorOffset(float x, float y, float z, float w);

            [DllImport("OculusXRPlugin")]
            public static extern void SetSpaceWarp( int on );

            [DllImport("OculusXRPlugin")]
            public static extern void SetAppSpacePosition(float x, float y, float z);

            [DllImport("OculusXRPlugin")]
            public static extern void SetAppSpaceRotation(float x, float y, float z, float w);
        }
    #endif
}