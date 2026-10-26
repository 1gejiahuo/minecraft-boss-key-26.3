using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

internal static class AudioSessions
{
    private const int DeviceStateActive = 0x00000001;
    private const int ClsctxAll = 23;

    public static void SetMute(int processId, bool mute, Dictionary<string, bool> previous, bool remember)
    {
        if (processId == 0) return;
        IMMDeviceEnumerator devices = null;
        IMMDeviceCollection endpoints = null;
        try
        {
            devices = (IMMDeviceEnumerator)new MMDeviceEnumerator();
            devices.EnumAudioEndpoints(0, DeviceStateActive, out endpoints);
            uint deviceCount;
            endpoints.GetCount(out deviceCount);
            for (uint deviceIndex = 0; deviceIndex < deviceCount; deviceIndex++)
            {
                IMMDevice endpoint = null;
                object managerObject = null;
                IAudioSessionEnumerator sessions = null;
                try
                {
                    endpoints.Item(deviceIndex, out endpoint);
                    Guid managerId = typeof(IAudioSessionManager2).GUID;
                    endpoint.Activate(ref managerId, ClsctxAll, IntPtr.Zero, out managerObject);
                    IAudioSessionManager2 manager = (IAudioSessionManager2)managerObject;
                    manager.GetSessionEnumerator(out sessions);
                    int sessionCount;
                    sessions.GetCount(out sessionCount);
                    for (int sessionIndex = 0; sessionIndex < sessionCount; sessionIndex++)
                    {
                        IAudioSessionControl control = null;
                        try
                        {
                            sessions.GetSession(sessionIndex, out control);
                            IAudioSessionControl2 details = (IAudioSessionControl2)control;
                            uint sessionProcessId;
                            details.GetProcessId(out sessionProcessId);
                            if (sessionProcessId != unchecked((uint)processId)) continue;

                            IntPtr instancePointer;
                            details.GetSessionInstanceIdentifier(out instancePointer);
                            string key;
                            try { key = Marshal.PtrToStringUni(instancePointer); }
                            finally { Marshal.FreeCoTaskMem(instancePointer); }
                            if (key == null) continue;

                            ISimpleAudioVolume volume = (ISimpleAudioVolume)control;
                            if (remember)
                            {
                                if (!previous.ContainsKey(key))
                                {
                                    bool wasMuted;
                                    volume.GetMute(out wasMuted);
                                    previous.Add(key, wasMuted);
                                }
                                Guid context = Guid.Empty;
                                volume.SetMute(true, ref context);
                            }
                            else
                            {
                                bool wasMuted;
                                if (previous.TryGetValue(key, out wasMuted))
                                {
                                    Guid context = Guid.Empty;
                                    volume.SetMute(wasMuted, ref context);
                                }
                            }
                        }
                        finally { if (control != null) Marshal.ReleaseComObject(control); }
                    }
                }
                finally
                {
                    if (sessions != null) Marshal.ReleaseComObject(sessions);
                    if (managerObject != null) Marshal.ReleaseComObject(managerObject);
                    if (endpoint != null) Marshal.ReleaseComObject(endpoint);
                }
            }
        }
        finally
        {
            if (endpoints != null) Marshal.ReleaseComObject(endpoints);
            if (devices != null) Marshal.ReleaseComObject(devices);
        }
    }

    [ComImport, Guid("BCDE0395-E52F-467C-8E3D-C4579291692E")]
    private class MMDeviceEnumerator { }

    [ComImport, Guid("A95664D2-9614-4F35-A746-DE8DB63617E6"), InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    private interface IMMDeviceEnumerator
    {
        void EnumAudioEndpoints(int flow, int stateMask, out IMMDeviceCollection devices);
        void GetDefaultAudioEndpoint(int flow, int role, out IMMDevice device);
        void GetDevice([MarshalAs(UnmanagedType.LPWStr)] string id, out IMMDevice device);
        void RegisterEndpointNotificationCallback(IntPtr callback);
        void UnregisterEndpointNotificationCallback(IntPtr callback);
    }

    [ComImport, Guid("0BD7A1BE-7A1A-44DB-8397-CC5392387B5E"), InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    private interface IMMDeviceCollection
    {
        void GetCount(out uint count);
        void Item(uint index, out IMMDevice device);
    }

    [ComImport, Guid("D666063F-1587-4E43-81F1-B948E807363F"), InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    private interface IMMDevice
    {
        void Activate(ref Guid interfaceId, int context, IntPtr activationParams, [MarshalAs(UnmanagedType.IUnknown)] out object instance);
        void OpenPropertyStore(int access, out IntPtr properties);
        void GetId(out IntPtr id);
        void GetState(out int state);
    }

    [ComImport, Guid("77AA99A0-1BD6-484F-8BC7-2C654C9A9B6F"), InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    private interface IAudioSessionManager2
    {
        void GetAudioSessionControl(ref Guid sessionId, uint streamFlags, out IAudioSessionControl control);
        void GetSimpleAudioVolume(ref Guid sessionId, uint streamFlags, out ISimpleAudioVolume volume);
        void GetSessionEnumerator(out IAudioSessionEnumerator sessions);
        void RegisterSessionNotification(IntPtr notification);
        void UnregisterSessionNotification(IntPtr notification);
        void RegisterDuckNotification([MarshalAs(UnmanagedType.LPWStr)] string sessionId, IntPtr notification);
        void UnregisterDuckNotification(IntPtr notification);
    }

    [ComImport, Guid("E2F5BB11-0570-40CA-ACDD-3AA01277DEE8"), InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    private interface IAudioSessionEnumerator
    {
        void GetCount(out int count);
        void GetSession(int index, out IAudioSessionControl control);
    }

    [ComImport, Guid("F4B1A599-7266-4319-A8CA-E70ACB11E8CD"), InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    private interface IAudioSessionControl { }

    [ComImport, Guid("BFB7FF88-7239-4FC9-8FA2-07C950BE9C6D"), InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    private interface IAudioSessionControl2
    {
        void GetState(out int state);
        void GetDisplayName(out IntPtr name);
        void SetDisplayName([MarshalAs(UnmanagedType.LPWStr)] string name, ref Guid context);
        void GetIconPath(out IntPtr path);
        void SetIconPath([MarshalAs(UnmanagedType.LPWStr)] string path, ref Guid context);
        void GetGroupingParam(out Guid grouping);
        void SetGroupingParam(ref Guid grouping, ref Guid context);
        void RegisterAudioSessionNotification(IntPtr events);
        void UnregisterAudioSessionNotification(IntPtr events);
        void GetSessionIdentifier(out IntPtr id);
        void GetSessionInstanceIdentifier(out IntPtr id);
        void GetProcessId(out uint id);
        void IsSystemSoundsSession();
        void SetDuckingPreference([MarshalAs(UnmanagedType.Bool)] bool optOut);
    }

    [ComImport, Guid("87CE5498-68D6-44E5-9215-6DA47EF883D8"), InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    private interface ISimpleAudioVolume
    {
        void SetMasterVolume(float volume, ref Guid context);
        void GetMasterVolume(out float volume);
        void SetMute([MarshalAs(UnmanagedType.Bool)] bool mute, ref Guid context);
        void GetMute([MarshalAs(UnmanagedType.Bool)] out bool muted);
    }
}

