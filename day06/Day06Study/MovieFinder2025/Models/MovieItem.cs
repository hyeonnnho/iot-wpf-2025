using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;

namespace MovieFinder2025.Models
{
    public class MovieItem : ObservableObject
    {
        /*
         * {
                  "adult": false,
                  "backdrop_path": "/l33oR0mnvf20avWyIMxW02EtQxn.jpg",
                  "genre_ids": [12, 18, 878],
                  "id": 157336,
                  "original_language": "en",
                  "original_title": "Interstellar",
                  "overview": "세계 각국의 정부와 경제가 완전히 붕괴된 미래가 다가온다. 지난 20세기에 범한 잘못이 전 세계적인 식량 부족을 불러왔고, NASA도 해체되었다. 나사 소속 우주비행사였던 쿠퍼는 지구에 몰아친 식량난으로 옥수수나 키우며 살고 있다. 거센 황사가 몰아친 어느 날 알 수 없는 힘에 이끌려 딸과 함께 도착한 곳은 인류가 이주할 행성을 찾는 나사의 비밀본부. 이 때 시공간에 불가사의한 틈이 열리고, 이 곳을 탐험해 인류를 구해야 하는 임무를 위해 쿠퍼는 만류하는 딸을 뒤로한 채 우주선에 탑승하는데...",
                  "popularity": 36.3951,
                  "poster_path": "/evoEi8SBSvIIEveM3V6nCJ6vKj8.jpg",
                  "release_date": "2014-11-05",
                  "title": "인터스텔라",
                  "video": false,
                  "vote_average": 8.455,
                  "vote_count": 37092
                },
        */
        private bool _adult;
        private string _backdrop_path;  // 사용안함
        private List<int> _genre_ids;  // 사용안함
        private int _id;
        private string _original_language;
        private string _original_title;
        private string _overview;
        private double _popularity;
        private string _poster_path;
        private DateTime _release_date;
        private string _title;
        private bool _video;
        private double _vote_average;
        private int _vote_count;

        public bool Adult { 
            get => _adult;
            set => SetProperty(ref _adult, value);
        }
        public string Backdrop_path { 
            get => _backdrop_path;
            set => SetProperty(ref _backdrop_path, value);
        }
        public List<int> Genre_ids { 
            get => _genre_ids;
            set => SetProperty(ref _genre_ids, value);
        }
        public int Id { 
            get => _id; 
            set => SetProperty(ref _id, value);
        }
        public string Original_language
        { 
            get => _original_language; 
            set => SetProperty(ref _original_language, value); 
        }
        public string Original_title { 
            get => _original_title;
            set => SetProperty(ref _original_title, value);
        }
        public string Overview { 
            get => _overview; 
            set => SetProperty(ref _overview, value); 
        }
        public double Popularity {
            get => _popularity;
            set => SetProperty(ref _popularity, value);
        }
        public string Poster_path { 
            get => _poster_path;
            set => SetProperty(ref _poster_path, value);
        }
        public DateTime Release_date { 
            get => _release_date;
            set => SetProperty(ref _release_date, value);
        }
        public string Title {
            get => _title; 
            set => SetProperty(ref _title, value);
        }
        public bool Video { 
            get => _video;
            set => SetProperty(ref _video, value);
        }
        public double Vote_average { 
            get => _vote_average;
            set => SetProperty(ref _vote_average, value);
        }
        public int Vote_count {
            get => _vote_count; 
            set => SetProperty(ref _vote_count, value);
        }
    }
}
