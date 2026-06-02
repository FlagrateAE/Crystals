using System.Runtime.InteropServices;

namespace Crystals.Core.Utilities;

public enum MLAPI_Status : int
{
    MLAPI_OK = 0,
    MLAPI_ERROR = -1,
    MLAPI_TIMEOUT = -2,
    MLAPI_NO_IMPLEMENTED = -3,
    MLAPI_NOT_INITIALIZED = -4,
    MLAPI_INVALID_ARGUMENT = -101,
    MLAPI_DEVICE_NOT_FOUND = -102,
    MLAPI_NOT_SUPPORTED = -103
}

public static class MysticLightNative
{
    private const string DLL_NAME = "MysticLight_SDK_x64.dll";

    [DllImport(DLL_NAME, SetLastError = true, CallingConvention = CallingConvention.Cdecl,
        EntryPoint = "MLAPI_Initialize")]
    public static extern MLAPI_Status Initialize();

    [DllImport(DLL_NAME, SetLastError = true, CallingConvention = CallingConvention.Cdecl,
        EntryPoint = "MLAPI_Release")]
    public static extern MLAPI_Status Release();

    [DllImport(DLL_NAME, SetLastError = true, CallingConvention = CallingConvention.Cdecl,
        EntryPoint = "MLAPI_GetErrorMessage")]
    public static extern MLAPI_Status GetErrorMessage(int errorCode,
        [MarshalAs(UnmanagedType.BStr)] out string desc);

    [DllImport(DLL_NAME, SetLastError = true, CallingConvention = CallingConvention.Cdecl,
        EntryPoint = "MLAPI_GetDeviceInfo")]
    public static extern MLAPI_Status GetDeviceInfo(
        [MarshalAs(UnmanagedType.SafeArray, SafeArraySubType = VarEnum.VT_BSTR)]
        out string[] pDevType,
        [MarshalAs(UnmanagedType.SafeArray, SafeArraySubType = VarEnum.VT_BSTR)]
        out string[] pLedCount);

    [DllImport(DLL_NAME, SetLastError = true, CallingConvention = CallingConvention.Cdecl,
        EntryPoint = "MLAPI_GetLedName")]
    public static extern MLAPI_Status GetLedName(
        [MarshalAs(UnmanagedType.BStr)] string type,
        [MarshalAs(UnmanagedType.SafeArray, SafeArraySubType = VarEnum.VT_BSTR)]
        out string[] ledName);

    [DllImport(DLL_NAME, SetLastError = true, CallingConvention = CallingConvention.Cdecl,
        EntryPoint = "MLAPI_SetLedStyle")]
    public static extern MLAPI_Status SetLedStyle(
        [MarshalAs(UnmanagedType.BStr)] string type,
        int index,
        [MarshalAs(UnmanagedType.BStr)] string style);

    [DllImport(DLL_NAME, SetLastError = true, CallingConvention = CallingConvention.Cdecl,
        EntryPoint = "MLAPI_SetLedColorsSync")]
    public static extern MLAPI_Status SetLedColorsSync(
        [MarshalAs(UnmanagedType.BStr)] string type,
        [MarshalAs(UnmanagedType.SafeArray, SafeArraySubType = VarEnum.VT_BSTR)]
        ref string[] ledName,
        int[] r,
        int[] g,
        int[] b);
}