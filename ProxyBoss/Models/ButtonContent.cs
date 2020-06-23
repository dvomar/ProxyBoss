using System.ComponentModel;
using System.Windows.Media;
using Brush = System.Windows.Media.Brush;

namespace ProxyBoss.Models
{
    public class ButtonContent : INotifyPropertyChanged
    {
        public static Brush BrushCrimson = new SolidColorBrush(Color.FromArgb(System.Drawing.Color.Crimson.A, System.Drawing.Color.Crimson.R, System.Drawing.Color.Crimson.G, System.Drawing.Color.Crimson.B));
        public static Brush BrushChartreuse = new SolidColorBrush(Color.FromArgb(System.Drawing.Color.Chartreuse.A, System.Drawing.Color.Chartreuse.R, System.Drawing.Color.Chartreuse.G, System.Drawing.Color.Chartreuse.B));

        private string _text = "Proxy Disabled";
        private string _buttonText = "Enable";

        private Brush _textColor = BrushCrimson;
        private Brush _buttonTextColor = BrushChartreuse;

        public string Text
        {
            get => _text;
            set
            {
                if (_text != value)
                {
                    _text = value;
                    NotifyPropertyChanged("Text");
                }
            }
        }
        
        public Brush TextColor
        {
            get => _textColor;
            set
            {
                if (_textColor != value)
                {
                    _textColor = value;
                    NotifyPropertyChanged("TextColor");
                }
            }
        }

        public string ButtonText
        {
            get => _buttonText;
            set
            {
                if (_buttonText != value)
                {
                    _buttonText = value;
                    NotifyPropertyChanged("ButtonText");
                }
            }
        }

        public Brush ButtonTextColor
        {
            get => _buttonTextColor;
            set
            {
                if (_buttonTextColor != value)
                {
                    _buttonTextColor = value;
                    NotifyPropertyChanged("ButtonTextColor");
                }
            }
        }

        public void NotifyPropertyChanged(string propName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propName));
        }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}