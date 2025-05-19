using System.Windows;
using System.Windows.Input;

namespace WpfSmartHomeApp
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void Border_MouseDown(object sender, MouseButtonEventArgs e)
        {

        }

        private void Border_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {

        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown(); // 앱 완전 종료
        }

        private void window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            // 제목표시줄  X버튼 누를 때, Alt+F4 누를 때 발생하는 이벤트
            e.Cancel = true;    // 앱 종료를 막는 기능
        }
    }
}