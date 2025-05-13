using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using MahApps.Metro.Controls.Dialogs;
using MySql.Data.MySqlClient;
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
                Releasedate = string.Empty,
                ISBN = string.Empty,
                Price = string.Empty
            };
            _isUpdate = false;  // 신규 추가를 위해서 초기화
        }

        private async Task LoadGridFromDb()
        {
            try
            {
                string query = "SELECT b.idx, b.division, d.names AS dnames, b.names AS bnames, b.author, b.releaseDate, b.isbn, b.price FROM bookstbl as b JOIN divtbl AS d ON b.division = d.division ORDER BY b.idx";
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
                        var bnames = reader.GetString("bnames");
                        var author = reader.GetString("author");
                        var releasedate = reader.GetString("releasedate");
                        var isbn = reader.GetString("isbn");
                        var price = reader.GetString("price");

                        books.Add(new Book()
                        {
                            Idx = idx,
                            Division = division,
                            DNames = dnames,
                            Names = bnames,
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
