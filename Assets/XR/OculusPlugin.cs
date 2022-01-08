
#if !(UNITY_EDITOR_WIN || UNITY_STANDALONE_WIN || (UNITY_ANDROID && !UNITY_EDITOR))
#define OVRPLUGIN_UNSUPPORTED_PLATFORM
#endif

using System;
using System.Runtime.InteropServices;

public static class OculusPlugin
{
    public enum FFR { Off = 0, Low = 1, Medium = 2, High = 3, HighTop = 4, EnumSize = 0x7FFFFFFF }

    public static bool ffrSupprted => 0 == DLL.ovrp_GetTiledMultiResSupported( out int supprted ) ? supprted == 1 : false;
    public static FFR ffrValue 
    {
        get => ffrSupprted ? ( 0 == DLL.ovrp_GetTiledMultiResLevel( out FFR value ) ? value : default ) : default ;
        
        set { if( ffrSupprted ) DLL.ovrp_SetTiledMultiResLevel( value ); }
    }

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
    }
}