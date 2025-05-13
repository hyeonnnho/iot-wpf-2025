using System.Windows;
using System.Windows.Controls;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MahApps.Metro.Controls.Dialogs;
using WpfBookRentalShop01.Helpers;
using WpfBookRentalShop01.Views;

namespace WpfBookRentalShop01.ViewModels
{

    public partial class MainViewModel : ObservableObject
    {
        // MahApps.Metro 형태 다이얼로그 코디네이터
        private readonly IDialogCoordinator dialogCoordinator;

        private string _greeting;

        public string Greeting
        {
            get => _greeting;
            set => SetProperty(ref _greeting, value);
        }

        private string _currentStatus;

        public string CurrentStatus
        {
            get => _currentStatus;
            set => SetProperty(ref _currentStatus, value);
        }

        private UserControl _currentView;

        public UserControl CurrentView
        {
            get => _currentView;
            set => SetProperty(ref _currentView, value);
        }
        public MainViewModel(IDialogCoordinator coordinator)
        {
            this.dialogCoordinator = coordinator; // 다이얼로그 코디네이터 초기화
            Greeting = "BookRentalShop!!";

            Common.LOGGER.Info("책 대여점 프로그램 실행");
        }

        #region '화면 기능(이벤트) 처리'

        [RelayCommand]
        public async void AppExit()
        {
            //MessageBox.Show("종료합니다.");
            //await this.dialogcoordinator.ShowMessageAsync(this, "종료", "정말 종료하시겠습니까?", MessageDialogStyle.AffirmativeAndNegative);
            var result = await this.dialogCoordinator.ShowMessageAsync(this, "종료 확인", "정말 종료하시겠습니까?", MessageDialogStyle.AffirmativeAndNegative);
            if (result == MessageDialogResult.Affirmative)  // ok 누르면
            {
                Application.Current.Shutdown();
            }
            else { return; }
        }
        #endregion

        [RelayCommand]
        public void ShowBookGenre()
        {
            var vm = new BookGenreViewModel(Common.DIALOGCOORDINATOR);
            var v = new BookGenreView()
            {
                DataContext = vm
            };
            CurrentView = v;
            CurrentStatus = "책 장르 화면입니다.";

            Common.LOGGER.Info("책 장르 관리 프로그램 실행");
        }

        [RelayCommand]
        public void ShowBooks()
        {
            var vm = new BooksViewModel(Common.DIALOGCOORDINATOR);
            var v = new BooksView()
            {
                DataContext = vm
            };
            CurrentView = v;
            CurrentStatus = "책 관리 화면입니다.";

            Common.LOGGER.Info("책 관리 프로그램 실행");
        }

        [RelayCommand]
        public void ShowMembers()
        {
            var vm = new MembersViewModel(Common.DIALOGCOORDINATOR);
            var v = new MembersView()
            {
                DataContext = vm
            };
            CurrentView = v;
            CurrentStatus = "회원 관리 화면입니다.";

            Common.LOGGER.Info("회원 관리 프로그램 실행");
        }

        [RelayCommand]
        public void ShowRentals()
        {
            var vm = new RentalsViewModel(Common.DIALOGCOORDINATOR);
            var v = new RentalsView()
            {
                DataContext = vm
            };
            CurrentView = v;
            CurrentStatus = "대여 관리 화면입니다.";

            Common.LOGGER.Info("대여 관리 프로그램 실행");
        }
    }
}
