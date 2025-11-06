namespace nkd.xr
{
    using Device = UnityEngine.XR.InputDeviceCharacteristics;

    public enum MXDevice : uint
    { 
        HEAD = Device.HeadMounted,
        LEFT = Device.Left | Device.Controller,
        RIGHT = Device.Right | Device.Controller

    }
}