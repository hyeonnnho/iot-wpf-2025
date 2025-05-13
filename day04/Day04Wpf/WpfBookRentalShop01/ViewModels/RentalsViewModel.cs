using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using MahApps.Metro.Controls.Dialogs;
using MySql.Data.MySqlClient;
using WpfBookRentalShop01.Helpers;
using WpfBookRentalShop01.Models;

namespace WpfBookRentalShop01.ViewModels
{
    public partial class RentalsViewModel : ObservableObject
    {
        private readonly IDialogCoordinator dialogCoordinator;  // MainViewModel과 동일

        private ObservableCollection<Member> _members;

        public ObservableCollection<Member> Members
        {
            get => _members;
            set => SetProperty(ref _members, value);
        }

        public RentalsViewModel(IDialogCoordinator coordinator)
        {
            this.dialogCoordinator = coordinator; // 다이얼로그 코디네이터 초기화

            LoadGridFromDb();
        }

        private async Task LoadGridFromDb()
        {
            try
            {
                string query = "SELECT idx, names, levels, addr, mobile, email FROM membertbl";
                ObservableCollection<Member> members = new ObservableCollection<Member>();

                using (MySqlConnection conn = new MySqlConnection(Common.CONNSTR))
                {
                    conn.Open();
                    MySqlCommand cmd = new MySqlCommand(query, conn);
                    MySqlDataReader reader = cmd.ExecuteReader();

                    while (reader.Read())
                    {
                        var idx = reader.GetInt32("idx");
                        var names = reader.GetString("names");
                        var levels = reader.GetString("levels");
                        var addr = reader.GetString("addr");
                        var mobile = reader.GetString("mobile");
                        var email = reader.GetString("email");

                        members.Add(new Member()
                        {
                            Idx = idx,
                            Names = names,
                            Levels = levels,
                            Addr = addr,
                            Mobile = mobile,
                            Email = email
                        });
                    }
                }
                Members = members;  // View에 바인딩 필수
            }
            catch (Exception ex)
            {
                Common.LOGGER.Error(ex.Message);
                //MessageBox.Show(ex.Message);
                await this.dialogCoordinator.ShowMessageAsync(this, "DB 오류", ex.Message, MessageDialogStyle.Affirmative);
            }
            Common.LOGGER.Info("DB에서 장르 데이터 로드 성공");
        }
    }
}
