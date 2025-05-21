using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Windows;
using System.Windows.Threading;

namespace WpfSmartHomeApp.ViewModels
{
    public partial class MainViewModel : ObservableObject
    {
        // 온도 습성
        private double _homeTemp;
        private double _homeHumid;

        private string _detectResult;
        private bool _isDetectOn;
        private string _rainResult;
        private bool _isRainOn;
        private string _airconResult;
        private bool _isAirConOn;
        private string _lightResult;
        private bool _isLightOn;
        private string? _currDateTime;

        private readonly DispatcherTimer _timer;

        public MainViewModel()
        {
            CurrDateTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            _isDetectOn = false;
            _timer = new DispatcherTimer();
            _timer.Interval= TimeSpan.FromSeconds(1);
            _timer.Tick += (sender, e) =>
            {
                CurrDateTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            };
            _timer.Start();
        }

        public string? CurrDateTime
        {
            get => _currDateTime;
            set => SetProperty(ref _currDateTime, value);
        }

        public double HomeTemp
        {
            get => _homeTemp;
            set => SetProperty(ref _homeTemp, value);
        }

        // 습도 습성
        public double HomeHumid
        {
            get => _homeHumid;
            set => SetProperty(ref _homeHumid, value);
        }

        // 사람 인지
        public string DetectResult
        {
            get => _detectResult;
            set => SetProperty(ref _detectResult, value);
        }

        // 사람 인지 여부
        public bool IsDetectOn
        {
            get => _isDetectOn;
            set => SetProperty(ref _isDetectOn, value);
        }

        // 

        public string RainResult
        {
            get => _rainResult;
            set => SetProperty(ref _rainResult, value);
        }

        public bool IsRainOn
        {
            get => _isRainOn;
            set => SetProperty(ref _isRainOn, value);
        }

        public string AirConResult
        {
            get => _airconResult;
            set => SetProperty(ref _airconResult, value);
        }

        public bool IsAirConOn
        {
            get => _isAirConOn;
            set => SetProperty(ref _isAirConOn, value);
        }
        
        public string LightResult
        {
            get => _lightResult;
            set => SetProperty(ref _lightResult, value);
        }

        public bool IsLightOn
        {
            get => _isLightOn;
            set => SetProperty(ref _isLightOn, value);
        }

        // LoadedCommand 앞에 On이 붙고 Command는 삭제
        [RelayCommand]
        public void OnLoaded()
        {
            HomeTemp = 27;
            HomeHumid = 43.2;

            DetectResult = "Detected Human!";
            IsDetectOn = true;

            RainResult = "Rainging";
            IsRainOn = true;
            AirConResult = "AirCon On!";
            IsAirConOn = true;
            LightResult = "Light On~";
            IsLightOn = true;
        }
    }
}
