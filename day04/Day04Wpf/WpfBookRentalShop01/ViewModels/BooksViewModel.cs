using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MahApps.Metro.Controls.Dialogs;
using MySql.Data.MySqlClient;
using System.Collections.ObjectModel;
using WpfBookRentalShop01.Helpers;
using WpfBookRentalShop01.Models;

namespace WpfBookRentalShop01.ViewModels
{
    public partial class BooksViewModel : ObservableObject
    {
        private readonly IDialogCoordinator dialogCoordinator;  // MainViewModel과 동일

        private ObservableCollection<Book> _books;
        public ObservableCollection<Book> Books
        {
            get => _books;
            set => SetProperty(ref _books, value);
        }

        private Book _selectedBook;
        public Book SelectedBook
        {
            get => _selectedBook;
            set
            {
                SetProperty(ref _selectedBook, value);
                _isUpdate = true;  //수정
            }
        }

        private bool _isUpdate;

        public BooksViewModel(IDialogCoordinator coordinator)
        {
            this.dialogCoordinator = coordinator; // 다이얼로그 코디네이터 초기화
            InitVariable();
            LoadGridFromDb();
        }
        private void InitVariable()
        {
            SelectedBook = new Book
            {
                Idx = 0,
                Division = string.Empty,
                DNames = string.Empty,
                Names = string.Empty,
                Author = string.Empty,
                Releasedate = DateTime.Now,
                ISBN = string.Empty,
                Price = 0
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
                        query = @"UPDATE bookstbl
                                    SET division = @division,
                                        names = @names,
                                        author = @author,
                                        releaseDate = @releaseDate,
                                        isbn = @isbn,
                                        price = @price
                                    WHERE idx = @idx;";
                    }
                    else  // 신규 추가
                    {
                        query = @"INSERT INTO bookstbl (division, names, author, releaseDate, isbn, price)
                                                VALUES (@division, @names, @author, @releaseDate, @isbn, @price);";
                    }

                    MySqlCommand cmd = new MySqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@division", SelectedBook.Division);
                    cmd.Parameters.AddWithValue("@names", SelectedBook.Names);
                    cmd.Parameters.AddWithValue("@author", SelectedBook.Author);
                    cmd.Parameters.AddWithValue("@releasedate", SelectedBook.Releasedate);
                    cmd.Parameters.AddWithValue("@isbn", SelectedBook.ISBN);
                    cmd.Parameters.AddWithValue("@price", SelectedBook.Price);
                    if (_isUpdate)  // 업데이트일때만 @idx가  필요함
                    {
                        cmd.Parameters.AddWithValue("@idx", SelectedBook.Idx);
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
                string query = "DELETE FROM bookstbl WHERE idx = @idx";

                using (MySqlConnection conn = new MySqlConnection(Common.CONNSTR))
                {
                    conn.Open();
                    MySqlCommand cmd = new MySqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@idx", SelectedBook.Idx);
                    int resultCnt = cmd.ExecuteNonQuery();
                    if (resultCnt > 0)
                    {
                        Common.LOGGER.Info($"도서 데이터 {SelectedBook.Idx} / {SelectedBook.Names} 삭제 성공");
                        await this.dialogCoordinator.ShowMessageAsync(this, "삭제 성공", "도서 데이터 삭제 성공", MessageDialogStyle.Affirmative);
                    }
                    else
                    {
                        Common.LOGGER.Warn("도서 데이터 삭제 실패");
                        await this.dialogCoordinator.ShowMessageAsync(this, "삭제 실패", "도서 데이터 삭제 실패", MessageDialogStyle.Affirmative);
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
                //string query = "SELECT b.idx, b.division, d.names AS dnames, b.names AS bnames, b.author, b.releaseDate, b.isbn, b.price FROM bookstbl as b, divtbl AS d JOIN divtbl AS d ON b.division = d.division ORDER BY b.idx";
                string query = @"SELECT idx, author, b.division as division, d.names as dnames, b.names, releaseDate, isbn, price
                                   FROM bookstbl as b, divtbl as d
                                  WHERE b.division = d.division
                               ORDER BY idx";
                ObservableCollection<Book> books= new ObservableCollection<Book>();

                using (MySqlConnection conn = new MySqlConnection(Common.CONNSTR))
                {
                    conn.Open();
                    MySqlCommand cmd = new MySqlCommand(query, conn);
                    MySqlDataReader reader = cmd.ExecuteReader();

                    while (reader.Read())
                    {
                        var idx = reader.GetInt32("idx");
                        var division = reader.GetString("division");
                        var dnames = reader.GetString("dnames");
                        var names = reader.GetString("names");
                        var author = reader.GetString("author");
                        var releasedate = reader.GetDateTime("releasedate");
                        var isbn = reader.GetString("isbn");
                        var price = reader.GetInt32("price");

                        books.Add(new Book()
                        {
                            Idx = idx,
                            Division = division,
                            DNames = dnames,
                            Names = names,
                            Author = author,
                            Releasedate = releasedate,
                            ISBN = isbn,
                            Price = price,
                        });
                    }
                }
                Books = books;  // View에 바인딩 필수
            }
            catch (Exception ex)
            {
                Common.LOGGER.Error(ex.Message);
                await this.dialogCoordinator.ShowMessageAsync(this, "DB 오류", ex.Message, MessageDialogStyle.Affirmative);
            }
            Common.LOGGER.Info("도서 데이터 로드 성공");
        }
    }
}
