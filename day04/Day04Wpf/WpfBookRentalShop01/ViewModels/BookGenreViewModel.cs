using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MahApps.Metro.Controls.Dialogs;
using MySql.Data.MySqlClient;
using WpfBookRentalShop01.Helpers;
using WpfBookRentalShop01.Models;

namespace WpfBookRentalShop01.ViewModels
{
    public partial class BookGenreViewModel : ObservableObject
    {
        private readonly IDialogCoordinator dialogCoordinator;  // MainViewModel과 동일

        private ObservableCollection<Genre> _genres;
        public ObservableCollection<Genre> Genres
        {
            get => _genres;
            set => SetProperty(ref _genres, value);
        }

        private Genre _selectedGenre;
        public Genre SelectedGenre
        {
            get => _selectedGenre;
            set
            {
                SetProperty(ref _selectedGenre, value);
                _isUpdate = true;  //수정
            }
        }
        private bool _isUpdate;

        public BookGenreViewModel(IDialogCoordinator coordinator)
        {
            this.dialogCoordinator = coordinator; // 다이얼로그 코디네이터 초기화
            InitVariable();
            LoadGridFromDb();
        }

        private void InitVariable()
        {
            SelectedGenre = new Genre(); // 신규 추가를 위해서 초기화
            SelectedGenre.Names = string.Empty;
            SelectedGenre.Division = string.Empty;
            // 순서가 중요!
            _isUpdate = false;  //신규
        }

        private async Task LoadGridFromDb()
        {
            try
            {
                //string connectionString = "Server=localhost;Database=bookrentalshop;Uid=root;Pwd=12345;Charset=utf8;";
                string query = "SELECT division, names FROM divtbl";

                ObservableCollection<Genre> genres = new ObservableCollection<Genre>();
                using (MySqlConnection conn = new MySqlConnection(Common.CONNSTR))
                {
                    conn.Open();
                    MySqlCommand cmd = new MySqlCommand(query, conn);
                    MySqlDataReader reader = cmd.ExecuteReader();


                    while (reader.Read())
                    {
                        var division = reader.GetString("division");
                        var names = reader.GetString("names");

                        genres.Add(new Genre()
                        {
                            Division = division,
                            Names = names
                        });
                    }
                }
                Genres = genres;
            }
            catch (Exception ex)
            {
                Common.LOGGER.Error(ex.Message);
                //MessageBox.Show(ex.Message);
                await this.dialogCoordinator.ShowMessageAsync(this, "DB 오류", ex.Message, MessageDialogStyle.Affirmative);
            }
            Common.LOGGER.Info("DB에서 장르 데이터 로드 성공");
        }

        // SetInitCommand, SaveDataCommand, DeleteDataCommand
        [RelayCommand]
        public void SetInit()
        {
            // selectedGenre = null; // 위험한 행동
            InitVariable();
        }

        [RelayCommand]
        public async Task SaveData()
        {
            // 신규 추가 + 기존 데이터 수정
            //Debug.WriteLine(SelectedGenre.Names);
            //Debug.WriteLine(SelectedGenre.Division);
            //Debug.WriteLine(_isUpdate);

            try
            {
                //string connectionString = "Server=localhost;Database=bookrentalshop;Uid=root;Pwd=12345;Charset=utf8;";
                string query = string.Empty;

                using (MySqlConnection conn = new MySqlConnection(Common.CONNSTR))
                {
                    conn.Open();

                    if (_isUpdate) { query = "UPDATE divtbl SET names = @names WHERE division = @division"; }   // 기존 데이터 수정
                    else { query = "INSERT INTO divtbl VALUES (@division, @names)"; }                   // 신규 데이터 추가

                    MySqlCommand cmd = new MySqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@division", SelectedGenre.Division);
                    cmd.Parameters.AddWithValue("@names", SelectedGenre.Names);
                    int resultCnt = cmd.ExecuteNonQuery(); // 1건 추가가 되면 resultCnt = 1, 추가 실패 시 0
                    if (resultCnt > 0)
                    {
                        Common.LOGGER.Info("DB에 데이터 저장 성공");
                        //MessageBox.Show("추가 성공");
                        await this.dialogCoordinator.ShowMessageAsync(this, "DB 저장", "추가 성공", MessageDialogStyle.Affirmative);
                    }
                    else
                    {
                        Common.LOGGER.Warn("DB에 데이터 저장 실패");
                        await this.dialogCoordinator.ShowMessageAsync(this, "DB 저장", "추가 실패", MessageDialogStyle.Affirmative);
                        //MessageBox.Show("추가 실패");
                    }
                }
            }
            catch (Exception ex)
            {
                Common.LOGGER.Error(ex.Message);
                //MessageBox.Show(ex.Message);
                await this.dialogCoordinator.ShowMessageAsync(this, "DB 오류", ex.Message, MessageDialogStyle.Affirmative);
            }

            await LoadGridFromDb();   // 저장 후 다시 DB내용을 그리드에 그리기
        }

        [RelayCommand]
        public async void DelData()
        {
            if (_isUpdate == false)
            {
                await this.dialogCoordinator.ShowMessageAsync(this, "삭제", "삭제할 데이터를 선택하세요", MessageDialogStyle.Affirmative);
                //MessageBox.Show("삭제할 데이터를 선택하세요.");
                return;
            }

            var result = await this.dialogCoordinator.ShowMessageAsync(this, "삭제 여부", "정말 삭제하시겠습니까?", MessageDialogStyle.AffirmativeAndNegative);
            if (result == MessageDialogResult.Negative)  // cancel 누르면
            {
                return;
            }

            try
            {
                //string connectionString = "Server=localhost;Database=bookrentalshop;Uid=root;Pwd=12345;Charset=utf8;";
                string query = "DELETE FROM divtbl WHERE division = @division";

                using (MySqlConnection conn = new MySqlConnection(Common.CONNSTR))
                {
                    conn.Open();
                    MySqlCommand cmd = new MySqlCommand(query, conn);

                    cmd.Parameters.AddWithValue("@division", SelectedGenre.Division);
                    int resultCnt = cmd.ExecuteNonQuery(); // 1건 삭제가 되면 resultCnt = 1, 삭제 실패 시 0

                    if (resultCnt > 0)
                    {
                        Common.LOGGER.Info($"책 장르 데이터 {SelectedGenre.Division} 삭제 성공");
                        await this.dialogCoordinator.ShowMessageAsync(this, "삭제", "삭제 성공", MessageDialogStyle.Affirmative);
                        //MessageBox.Show("삭제 성공");
                    }
                    else
                    {
                        Common.LOGGER.Warn("책 장르 데이터 삭제 성공");
                        await this.dialogCoordinator.ShowMessageAsync(this, "삭제", "삭제 실패", MessageDialogStyle.Affirmative);
                        //MessageBox.Show("삭제 실패");
                    }
                }
            }
            catch (Exception ex)
            {
                Common.LOGGER.Error(ex.Message);
                await this.dialogCoordinator.ShowMessageAsync(this, "DB 오류", ex.Message, MessageDialogStyle.Affirmative);
                //MessageBox.Show(ex.Message);
            }

            await LoadGridFromDb();   // 삭제 후 다시 DB내용을 그리드에 그리기
        }
    }
}
