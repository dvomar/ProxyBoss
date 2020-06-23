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

        public void Switch()
        {
            RegistryKey registry = Registry.CurrentUser.OpenSubKey("Software\\Microsoft\\Windows\\CurrentVersion\\Internet Settings", true);

            if (registry == null)
                return;

            if ((int)registry.GetValue("ProxyEnable", 0) == 0)
                Enable(registry);
            else if ((int)registry.GetValue("ProxyEnable", 1) == 1)
                Disable(registry);

            registry.Close();

            bool settingsReturn = InternetSetOption(IntPtr.Zero, INTERNET_OPTION_SETTINGS_CHANGED, IntPtr.Zero, 0);
            bool refreshReturn = InternetSetOption(IntPtr.Zero, INTERNET_OPTION_REFRESH, IntPtr.Zero, 0);
        }

        public ProxyState GetState()
        {
            ProxyState result = ProxyState.Disabled;
            RegistryKey registry = Registry.CurrentUser.OpenSubKey("Software\\Microsoft\\Windows\\CurrentVersion\\Internet Settings", false);

            if ((int)registry.GetValue("ProxyEnable", 0) == 0)
                result = ProxyState.Disabled;
            else if ((int)registry.GetValue("ProxyEnable", 1) == 1)
                result = ProxyState.Enabled;

            registry.Close();

            return result;
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