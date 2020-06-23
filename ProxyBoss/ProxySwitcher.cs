using System;
using System.Runtime.InteropServices;
using Microsoft.Win32;

namespace ProxyBoss
{
    public class ProxySwitcher
    {
        [DllImport("wininet.dll")]
        public static extern bool InternetSetOption(IntPtr hInternet, int dwOption, IntPtr lpBuffer, int dwBufferLength);

        public const int INTERNET_OPTION_SETTINGS_CHANGED = 39;
        public const int INTERNET_OPTION_REFRESH = 37;

        public ProxyState Switch()
        {
            RegistryKey registry = Registry.CurrentUser.OpenSubKey("Software\\Microsoft\\Windows\\CurrentVersion\\Internet Settings", true);

            ProxyState result = GetRegistryState(registry);

            if (result == ProxyState.Disabled)
                Enable(registry);
            else if (result == ProxyState.Enabled)
                Disable(registry);

            registry?.Close();

            bool settingsReturn = InternetSetOption(IntPtr.Zero, INTERNET_OPTION_SETTINGS_CHANGED, IntPtr.Zero, 0);
            bool refreshReturn = InternetSetOption(IntPtr.Zero, INTERNET_OPTION_REFRESH, IntPtr.Zero, 0);

            return result;
        }

        public ProxyState GetState()
        {
            RegistryKey registry = Registry.CurrentUser.OpenSubKey("Software\\Microsoft\\Windows\\CurrentVersion\\Internet Settings", false);

            ProxyState result = GetRegistryState(registry);

            registry?.Close();

            return result;
        }

        private ProxyState GetRegistryState(RegistryKey registry)
        {
            if (registry == null)
                return ProxyState.Disabled;
            if ((int)registry.GetValue("ProxyEnable", 0) == 0)
                return ProxyState.Disabled;
            if ((int)registry.GetValue("ProxyEnable", 1) == 1)
                return ProxyState.Enabled;

            return ProxyState.Disabled;
        }

        private void Enable(RegistryKey registry)
        {
            registry.SetValue("ProxyEnable", 1);
        }

        private void Disable(RegistryKey registry)
        {
            registry.SetValue("ProxyEnable", 0);
        }
    }
}