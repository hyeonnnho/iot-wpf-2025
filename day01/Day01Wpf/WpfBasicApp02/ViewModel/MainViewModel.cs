using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;

namespace WpfBasicApp02.ViewModel
{
    internal class MainViewModel : INotifyPropertyChanged
    {
        public MainViewModel()
        {
            LoadControlFromDb();
            LoadGridFromDb();
        }

        private void LoadControlFromDb()
        {
            // 1. 연결문자열(DB 연결문자열은 필수)
            string conectionString = "Server=localhost;Database=bookrentalshop;Uid=root;Pwd=12345;Charset=utf8;";
            // 2. 사용쿼리
            string query = "SELECT division, names FROM divtbl";

            // Dictionary나 KeyValuePair 둘다 상관없음
            List<KeyValuePair<string, string>> divisions = new List<KeyValuePair<string, string>>();

            // 3. DB연결, 명령, 리더
            using (MySqlConnection conn = new MySqlConnection(conectionString))
            {
                try
                {
                    conn.Open();
                    MySqlCommand cmd = new MySqlCommand(query, conn);
                    MySqlDataReader reader = cmd.ExecuteReader();   // 데이터 가져올때

                    while (reader.Read())
                    {
                        var division = reader.GetString("division");
                        var names = reader.GetString("names");

                        divisions.Add(new KeyValuePair<string, string>(division, names));
                    }
                }
                catch (MySqlException ex)
                {
                    // later
                }
            }   // conn.Close() 자동으로 호출됨
        }
        private void LoadGridFromDb()
        {
            // 1. 연결문자열(DB 연결문자열은 필수)
            string conectionString = "Server=localhost;Database=bookrentalshop;Uid=root;Pwd=12345;Charset=utf8;";
            // 2. 사용쿼리, 기본쿼리로 먼저 작업 후 필요한 실제 쿼리로 변경
            string query = @"SELECT b.Idx, b.Author, b.Division, b.Names, b.ReleaseDate, b.ISBN, b.Price,
                                    d.Names AS dNames
                               FROM bookstbl as b, divtbl as d
                              WHERE b.Division = d.Division
                              ORDER BY b.Idx";
            // 3. DB연결, 명령, 리더
            using (MySqlConnection conn = new MySqlConnection(conectionString))
            {
                try
                {
                    conn.Open();
                    MySqlDataAdapter adapter = new MySqlDataAdapter(query, conn);
                    DataTable dt = new DataTable();
                    adapter.Fill(dt);   // 데이터 가져올때

                    // 데이터 그리드에 데이터 바인딩
                    GrdBooks.ItemsSource = dt.DefaultView;   // 데이터연동
                }
                catch (MySqlException ex)
                {
                    // later
                }
            }   // conn.Close() 자동으로 호출됨
        }

        public event PropertyChangedEventHandler? PropertyChanged;
    }
}
