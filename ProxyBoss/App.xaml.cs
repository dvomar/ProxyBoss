﻿using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows;
using System.Windows.Forms;
using Application = System.Windows.Application;
using Timer = System.Threading.Timer;

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
        private Timer _timer;
        private Stopwatch _stopwatch;
        
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            _proxySwitcher = new ProxySwitcher();

            var window = new MainWindow(_proxySwitcher);
            window.Closing += MainWindow_Closing;
            window.Deactivated += MainWindowOnDeactivated;
            window.SwitchRefresh += WindowOnSwitchRefresh;
            MainWindow = window;
            
            _notifyIcon = new NotifyIcon();
            _notifyIcon.MouseClick += ShowMainWindow;
            _notifyIcon.Visible = true;
            
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
            
            if (_proxySwitcher.RequiredProxyState == ProxyState.Disabled)
                _notifyIcon.ContextMenuStrip.Items.Add("Enable proxy").Click += (s, e) => SwitchProxy();
            else if (_proxySwitcher.RequiredProxyState == ProxyState.Enabled)
                _notifyIcon.ContextMenuStrip.Items.Add("Disable proxy").Click += (s, e) => SwitchProxy();

            _notifyIcon.ContextMenuStrip.Items.Add(new ToolStripSeparator());

            _notifyIcon.ContextMenuStrip.Items.Add("Open ProxyBoss").Click += ShowMainWindow;
            _notifyIcon.ContextMenuStrip.Items.Add("Exit").Click += (s, e) => ExitApplication();
        }

        private void SwitchProxy()
        {
            _proxySwitcher.Switch(_proxySwitcher.ReverseProxyState);

            var window = MainWindow as MainWindow;
            window.SetProxyViewSource();

            _stopwatch.Restart();
            ChangeContextMenuStripItemsByProxyState();
        }

        private void CreateTimer()
        {
            _timer = new Timer(Callback, "Temporarily checking proxy state", TimeSpan.FromSeconds(1), TimeSpan.FromSeconds(2));
            _stopwatch = new Stopwatch();
            _stopwatch.Start();
        }

        private void Callback(object state)
        {
            ProxyState proxyState = ChangeContextMenuStripItemsByProxyState();
            if (proxyState != _proxySwitcher.RequiredProxyState)
            {
                _proxySwitcher.Switch(_proxySwitcher.RequiredProxyState);
                ChangeContextMenuStripItemsByProxyState();
            }
            
            if (_stopwatch.IsRunning && _stopwatch.Elapsed >= TimeSpan.FromMinutes(1))
            {
                _timer?.Dispose();
                _timer = null;
                _stopwatch.Stop();
            }
        }

        private ProxyState ChangeContextMenuStripItemsByProxyState()
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

            if (_timer == null)
                CreateTimer();

            return state;
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