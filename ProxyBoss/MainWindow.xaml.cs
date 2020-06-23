using System;
using System.Windows;
using System.Windows.Data;
using ControlzEx.Theming;
using MahApps.Metro.Controls;
using ProxyBoss.Models;

namespace ProxyBoss
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : MetroWindow
    {
        private readonly ProxySwitcher _proxySwitcher;
        private readonly ButtonContent _buttonContent = new ButtonContent();

        public event EventHandler SwitchRefresh;
        
        public MainWindow()
        {
            InitializeComponent();

            ThemeManager.Current.ChangeTheme(this, "Dark.Blue");

            _proxySwitcher = new ProxySwitcher();

            var proxyViewSource = (CollectionViewSource)FindResource("ProxyViewSource");
            proxyViewSource.Source = new object[] { _buttonContent };

            SetProxyViewSource();
        }

        private void SetWindowAppearance()
        {
            WindowIconHelper.RemoveIcon(this);

            Rect desktopWorkingArea = SystemParameters.WorkArea;
            WindowButtonCommands.Visibility = Visibility.Hidden;
            ResizeMode = ResizeMode.NoResize;
            Topmost = true;
            ShowTitleBar = false;

            WindowStartupLocation = WindowStartupLocation.Manual;
            Top = desktopWorkingArea.Bottom - ActualHeight * 3;
            Left = desktopWorkingArea.Left;
        }

        protected override void OnSourceInitialized(EventArgs e)
        {
            base.OnSourceInitialized(e);

            SetWindowAppearance();
        }

        private void SwitchProxyButton_OnClick(object sender, RoutedEventArgs e)
        {
            _proxySwitcher.Switch();

            SetProxyViewSource();

            SwitchRefresh?.Invoke(this, new EventArgs());
        }

        private void RefreshButton_OnClick(object sender, RoutedEventArgs e)
        {
            SetProxyViewSource();

            SwitchRefresh?.Invoke(this, new EventArgs());
        }

        public void SetProxyViewSource()
        {
            ProxyState state = _proxySwitcher.GetState();

            if (state == ProxyState.Enabled)
            {
                _buttonContent.Text = "Proxy Enabled";
                _buttonContent.ButtonText = "Disable";
                _buttonContent.TextColor = ButtonContent.BrushChartreuse;
                _buttonContent.ButtonTextColor = ButtonContent.BrushCrimson;
            }
            else if (state == ProxyState.Disabled)
            {
                _buttonContent.Text = "Proxy Disabled";
                _buttonContent.ButtonText = "Enable";
                _buttonContent.TextColor = ButtonContent.BrushCrimson;
                _buttonContent.ButtonTextColor = ButtonContent.BrushChartreuse;
            }
        }
    }
}