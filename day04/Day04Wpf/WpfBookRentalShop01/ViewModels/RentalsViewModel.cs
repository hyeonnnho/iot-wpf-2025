using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MahApps.Metro.Controls.Dialogs;
using MySql.Data.MySqlClient;
using NLog.LayoutRenderers;
using WpfBookRentalShop01.Helpers;
using WpfBookRentalShop01.Models;

namespace WpfBookRentalShop01.ViewModels
{
    public partial class RentalsViewModel : ObservableObject
    {
        private readonly IDialogCoordinator dialogCoordinator;  // MainViewModel과 동일

        private ObservableCollection<Rental> _rentals;
        public ObservableCollection<Rental> Rentals
        {
            get => _rentals;
            set => SetProperty(ref _rentals, value);
        }

        private Rental _selectedRental;
        public Rental SelectedRental
        {
            get => _selectedRental;
            set
            {
                SetProperty(ref _selectedRental, value);
                _isUpdate = true;  //수정
            }
        }

        private bool _isUpdate;

        public RentalsViewModel(IDialogCoordinator coordinator)
        {
            this.dialogCoordinator = coordinator; // 다이얼로그 코디네이터 초기화
            InitVariable();
            LoadGridFromDb();
        }
        private void InitVariable()
        {
            SelectedRental = new Rental
            {
                Idx = 0,
                MemberIdx = 0,
                BookIdx = 0,
                BookNames = string.Empty,
                MemberName = string.Empty,
                RentalDate = DateTime.Now,
                ReturnDate = DateTime.Now
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
                        query = @"UPDATE rentaltbl
                                    SET memberidx = @memberidx,
                                        bookidx = @bookidx,
                                        rentaldate = @rentaldate,
                                        returndate = @returndate
                                    WHERE idx = @idx;";
                    }
                    else  // 신규 추가
                    {
                        query = @"INSERT INTO rentaltbl (memberidx, bookidx, rentaldate, returndate)
                                                VALUES (@memberidx, @bookidx, @rentaldate, @returndate);";
                    }

                    MySqlCommand cmd = new MySqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@memberidx", SelectedRental.MemberIdx);
                    cmd.Parameters.AddWithValue("@bookidx", SelectedRental.BookIdx);
                    cmd.Parameters.AddWithValue("@rentaldate", SelectedRental.RentalDate);
                    cmd.Parameters.AddWithValue("@returndate", SelectedRental.ReturnDate);
                    if (_isUpdate)  // 업데이트일때만 @idx가  필요함
                    {
                        cmd.Parameters.AddWithValue("@idx", SelectedRental.Idx);
                    }
                    int resultCnt = cmd.ExecuteNonQuery();
                    if (resultCnt > 0)
                    {
                        Common.LOGGER.Info("대여 데이터 저장 성공");
                        await this.dialogCoordinator.ShowMessageAsync(this, "저장 성공", "대여 데이터 저장 성공", MessageDialogStyle.Affirmative);
                    }
                    else
                    {
                        Common.LOGGER.Warn("대여 데이터 저장 실패");
                        await this.dialogCoordinator.ShowMessageAsync(this, "저장 실패", "대여 데이터 저장 실패", MessageDialogStyle.Affirmative);
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
                string query = "DELETE FROM rentaltbl WHERE idx = @idx";

                using (MySqlConnection conn = new MySqlConnection(Common.CONNSTR))
                {
                    conn.Open();
                    MySqlCommand cmd = new MySqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@idx", SelectedRental.Idx);
                    int resultCnt = cmd.ExecuteNonQuery();
                    if (resultCnt > 0)
                    {
                        Common.LOGGER.Info($"대여 데이터 {SelectedRental.Idx} 삭제 성공");
                        await this.dialogCoordinator.ShowMessageAsync(this, "삭제 성공", "대여 데이터 삭제 성공", MessageDialogStyle.Affirmative);
                    }
                    else
                    {
                        Common.LOGGER.Warn("대여 데이터 삭제 실패");
                        await this.dialogCoordinator.ShowMessageAsync(this, "삭제 실패", "대여 데이터 삭제 실패", MessageDialogStyle.Affirmative);
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
                string query = @"SELECT r.idx, r.memberidx, r.bookidx, b.names as booknames, m.names as membername, r.rentaldate, r.returndate
                                   FROM bookstbl as b, rentaltbl as r, membertbl as m 
                                  WHERE b.idx = r.bookidx AND m.idx = r.memberidx
                               ORDER BY r.idx";

                ObservableCollection<Rental> rentals = new ObservableCollection<Rental>();

                using (MySqlConnection conn = new MySqlConnection(Common.CONNSTR))
                {
                    conn.Open();
                    MySqlCommand cmd = new MySqlCommand(query, conn);
                    MySqlDataReader reader = cmd.ExecuteReader();

                    while (reader.Read())
                    {
                        var idx = reader.GetInt32("idx");
                        var memberidx = reader.GetInt32("memberidx");
                        var bookidx = reader.GetInt32("bookidx");
                        var booknames = reader.GetString("booknames");
                        var membername = reader.GetString("membername");
                        var rentaldate = reader.GetDateTime("rentaldate");
                        var returndate = reader.IsDBNull(reader.GetOrdinal("returndate"))
                                        ? DateTime.Now  // 기본값
                                        : reader.GetDateTime("returndate");

                        rentals.Add(new Rental()
                        {
                            Idx = idx,
                            MemberIdx = memberidx,
                            MemberName = membername,
                            BookIdx = bookidx,
                            BookNames = booknames,
                            RentalDate = rentaldate,
                            ReturnDate = returndate
                        });
                    }
                }
                Rentals = rentals;  // View에 바인딩 필수
            }
            catch (Exception ex)
            { 
                Common.LOGGER.Error(ex.Message);
                await this.dialogCoordinator.ShowMessageAsync(this, "DB 오류", ex.Message, MessageDialogStyle.Affirmative);
            }
            Common.LOGGER.Info("대여 데이터 로드 성공");
        }
    }
}
