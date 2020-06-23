using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Forms;
using Application = System.Windows.Application;

namespace ProxyBoss
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private NotifyIcon _notifyIcon;
        private bool _isExit;
        private ProxySwitcher _proxySwitcher;

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            var window = new MainWindow();
            window.Closing += MainWindow_Closing;
            window.Deactivated += MainWindowOnDeactivated;
            window.SwitchRefresh += WindowOnSwitchRefresh;
            MainWindow = window;

            _notifyIcon = new NotifyIcon();
            _notifyIcon.MouseClick += ShowMainWindow;
            _notifyIcon.Visible = true;

            _proxySwitcher = new ProxySwitcher();
            
            CreateContextMenu();
            ChangeContextMenuStripItemsByProxyState();
        }

        private void WindowOnSwitchRefresh(object sender, EventArgs e)
        {
            ChangeContextMenuStripItemsByProxyState();
        }

        private void CreateContextMenu()
        {
            _notifyIcon.ContextMenuStrip = new ContextMenuStrip();

            ProxyState state = _proxySwitcher.GetState();
            if (state == ProxyState.Disabled)
                _notifyIcon.ContextMenuStrip.Items.Add("Enable proxy").Click += (s, e) => SwitchProxy();
            else if (state == ProxyState.Enabled)
                _notifyIcon.ContextMenuStrip.Items.Add("Disable proxy").Click += (s, e) => SwitchProxy();

            _notifyIcon.ContextMenuStrip.Items.Add(new ToolStripSeparator());

            _notifyIcon.ContextMenuStrip.Items.Add("Open ProxyBoss").Click += ShowMainWindow;
            _notifyIcon.ContextMenuStrip.Items.Add("Exit").Click += (s, e) => ExitApplication();
        }

        private void SwitchProxy()
        {
            _proxySwitcher.Switch();

            var window = MainWindow as MainWindow;
            window.SetProxyViewSource();

            ChangeContextMenuStripItemsByProxyState();
        }

        private void ChangeContextMenuStripItemsByProxyState()
        {
            ProxyState state = _proxySwitcher.GetState();
            if (state == ProxyState.Disabled)
            {
                _notifyIcon.ContextMenuStrip.Items[0].Text = "Enable proxy";
                _notifyIcon.Icon = ProxyBoss.Properties.Resources.AppIconDisabled;
            }
            else if (state == ProxyState.Enabled)
            {
                _notifyIcon.ContextMenuStrip.Items[0].Text = "Disable proxy";
                _notifyIcon.Icon = ProxyBoss.Properties.Resources.AppIconEnabled;
            }
        }

        private void ExitApplication()
        {
            _isExit = true;
            MainWindow.Close();
            _notifyIcon.Dispose();
            _notifyIcon = null;
        }

        private void ShowMainWindow(object sender, EventArgs eventArgs)
        {
            if (eventArgs is MouseEventArgs mouseEventArgs)
                if (mouseEventArgs.Button != MouseButtons.Left)
                    return;

            if (MainWindow.IsVisible)
            {
                if (MainWindow.WindowState == WindowState.Minimized)
                    MainWindow.WindowState = WindowState.Normal;
                MainWindow.Activate();
            }
            else
            {
                MainWindow.Show();
                MainWindow.Activate();
            }
        }

        private void MainWindow_Closing(object sender, CancelEventArgs e)
        {
            if (!_isExit)
            {
                e.Cancel = true;
                MainWindow.Hide(); // A hidden window can be shown again, a closed one not
            }
        }

        private void MainWindowOnDeactivated(object sender, EventArgs e)
        {
            MainWindow.Hide();
        }
    }
}