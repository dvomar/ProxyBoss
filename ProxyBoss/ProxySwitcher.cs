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

        private ProxyState _proxyState;
        public ProxyState RequiredProxyState => _proxyState;

        public ProxyState ReverseProxyState
        {
            get
            {
                switch (RequiredProxyState)
                {
                    case ProxyState.Disabled:
                        return ProxyState.Enabled;
                    case ProxyState.Enabled:
                        return ProxyState.Disabled;
                    default:
                        return ProxyState.Enabled;
                }
            }
        }

        public ProxySwitcher()
        {
            _proxyState = GetState();
        }

        public void Switch(ProxyState state)
        {
            RegistryKey registry = Registry.CurrentUser.OpenSubKey("Software\\Microsoft\\Windows\\CurrentVersion\\Internet Settings", true);

            //ProxyState result = GetRegistryState(registry);

            switch (state)
            {
                case ProxyState.Disabled:
                    Disable(registry);
                    break;
                case ProxyState.Enabled:
                    Enable(registry);
                    break;
            }

            registry?.Close();

            bool settingsReturn = InternetSetOption(IntPtr.Zero, INTERNET_OPTION_SETTINGS_CHANGED, IntPtr.Zero, 0);
            bool refreshReturn = InternetSetOption(IntPtr.Zero, INTERNET_OPTION_REFRESH, IntPtr.Zero, 0);

            _proxyState = state;
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