using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MahApps.Metro.Controls.Dialogs;
using MySql.Data.MySqlClient;
using WpfBookRentalShop01.Helpers;
using WpfBookRentalShop01.Models;

namespace WpfBookRentalShop01.ViewModels
{
    public partial class MembersViewModel : ObservableObject
    {
        private readonly IDialogCoordinator dialogCoordinator;  // MainViewModel과 동일

        private ObservableCollection<Member> _members;
        public ObservableCollection<Member> Members
        {
            get => _members;
            set => SetProperty(ref _members, value);
        }

        private Member _selectedMember;
        public Member SelectedMember
        {
            get => _selectedMember;
            set {
                SetProperty(ref _selectedMember, value);
                _isUpdate = true;  //수정
            }
        }
        private bool _isUpdate;

        public MembersViewModel(IDialogCoordinator coordinator)
        {
            this.dialogCoordinator = coordinator; // 다이얼로그 코디네이터 초기화
            InitVariable();
            LoadGridFromDb();
        }
        private void InitVariable()
        {
            SelectedMember = new Member
            {
                Idx = 0,
                Names = string.Empty,
                Levels = string.Empty,
                Addr = string.Empty,
                Mobile = string.Empty,
                Email = string.Empty
            };
            _isUpdate = false;  // 신규 추가를 위해서 초기화
        }

        [RelayCommand]
        public void SetInit()
        {
            InitVariable();
        }

        [RelayCommand]
        public async void SaveData()
        {
            try
            {
                string query = string.Empty;

                using (MySqlConnection conn = new MySqlConnection(Common.CONNSTR))
                {
                    conn.Open();
                    if (_isUpdate)  // 수정
                    {
                        query = @"UPDATE membertbl
                                     SET names = @names,
                                         levels = @levels,
                                         addr = @addr,
                                         mobile = @mobile,
                                         email = @email
                                   WHERE idx = @idx";
                    }
                    else  // 신규 추가
                    {
                        query = @"INSERT INTO membertbl (names, levels, addr, mobile, email) 
                                                 VALUES (@names, @levels, @addr, @mobile, @email);";
                    }

                    MySqlCommand cmd = new MySqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@names", SelectedMember.Names);
                    cmd.Parameters.AddWithValue("@levels", SelectedMember.Levels);
                    cmd.Parameters.AddWithValue("@addr", SelectedMember.Addr);
                    cmd.Parameters.AddWithValue("@mobile", SelectedMember.Mobile);
                    cmd.Parameters.AddWithValue("@email", SelectedMember.Email);
                    if (_isUpdate)  // 업데이트일때만 @idx가  필요함
                    {
                        cmd.Parameters.AddWithValue("@idx", SelectedMember.Idx);
                    }
                    int resultCnt = cmd.ExecuteNonQuery();
                    if (resultCnt > 0)
                    {
                        Common.LOGGER.Info("회원 데이터 저장 성공");
                        await this.dialogCoordinator.ShowMessageAsync(this, "저장 성공", "회원 데이터 저장 성공", MessageDialogStyle.Affirmative);
                    }
                    else
                    {
                        Common.LOGGER.Warn("회원 데이터 저장 실패");
                        await this.dialogCoordinator.ShowMessageAsync(this, "저장 실패", "회원 데이터 저장 실패", MessageDialogStyle.Affirmative);
                    }
                }
            }
            catch (Exception ex)
            {
                Common.LOGGER.Error(ex.Message);
                await this.dialogCoordinator.ShowMessageAsync(this, "DB 오류", ex.Message, MessageDialogStyle.Affirmative);
            }
            LoadGridFromDb();  // DB에서 다시 로드
        }

        [RelayCommand]
        public async void DelData()
        {
            if (!_isUpdate)
            {
                await this.dialogCoordinator.ShowMessageAsync(this, "삭제 오류", "삭제할 데이터를 선택하세요", MessageDialogStyle.Affirmative);
                return;
            }
            var result = await this.dialogCoordinator.ShowMessageAsync(this, "삭제 확인", "정말 삭제하시겠습니까?", MessageDialogStyle.AffirmativeAndNegative);
            if (result == MessageDialogResult.Negative) return; // Cancel 했으면 메서드 빠져나감

            try
            {
                string query = "DELETE FROM membertbl WHERE idx = @idx";

                using (MySqlConnection conn = new MySqlConnection(Common.CONNSTR))
                {
                    conn.Open();
                    MySqlCommand cmd = new MySqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@idx", SelectedMember.Idx);
                    int resultCnt = cmd.ExecuteNonQuery();
                    if (resultCnt > 0)
                    {
                        Common.LOGGER.Info($"회원 데이터 {SelectedMember.Idx} / {SelectedMember.Names} 삭제 성공");
                        await this.dialogCoordinator.ShowMessageAsync(this, "삭제 성공", "회원 데이터 삭제 성공", MessageDialogStyle.Affirmative);
                    }
                    else
                    {
                        Common.LOGGER.Warn("회원 데이터 삭제 실패");
                        await this.dialogCoordinator.ShowMessageAsync(this, "삭제 실패", "회원 데이터 삭제 실패", MessageDialogStyle.Affirmative);
                    }
                }
            }
            catch (Exception ex)
            {
                Common.LOGGER.Error(ex.Message);
                await this.dialogCoordinator.ShowMessageAsync(this, "DB 오류", ex.Message, MessageDialogStyle.Affirmative);
            }
            LoadGridFromDb();  // DB에서 다시 로드
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
            Common.LOGGER.Info("멤버 데이터 로드 성공");
        }
    }
}
