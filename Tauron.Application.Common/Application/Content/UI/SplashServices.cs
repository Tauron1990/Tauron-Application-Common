#region

using JetBrains.Annotations;

#endregion

namespace Tauron.Application
{
    public sealed class SplashMessageListener : ObservableObject
    {
        public SplashMessageListener() => CurrentListner = this;

        public void ReceiveMessage([NotNull] string message) => Message = message;

        private string _message;
        private object _splashContent;
        private double _height = 236;
        private object _mainLabelForeground = "Black";
        private object _mainLabelBackground = "White";
        private double _width = 414;

        [NotNull]
        public static SplashMessageListener CurrentListner { get; set; } = new SplashMessageListener();

        public double Height
        {
            get => _height;

            set
            {
                _height = value;
                OnPropertyChanged();
            }
        }

        [NotNull]
        public object MainLabelForeground
        {
            get => _mainLabelForeground;

            set
            {
                _mainLabelForeground = value;
                OnPropertyChanged();
            }
        }

        [NotNull]
        public object MainLabelBackground
        {
            get => _mainLabelBackground;

            set
            {
                _mainLabelBackground = value;
                OnPropertyChanged();
            }
        }

        [NotNull]
        public string Message
        {
            get => _message;

            set
            {
                _message = value;
                OnPropertyChanged();
            }
        }

        [NotNull]
        public object SplashContent
        {
            get => _splashContent;

            set
            {
                _splashContent = value;
                OnPropertyChanged();
            }
        }

        public double Width
        {
            get => _width;

            set
            {
                _width = value;
                OnPropertyChanged();
            }
        }
    }

    [PublicAPI]
    public interface ISplashService
    {
        [NotNull]
        SplashMessageListener Listner { get; }

        void CloseSplash();

        void ShowSplash();
    }
}