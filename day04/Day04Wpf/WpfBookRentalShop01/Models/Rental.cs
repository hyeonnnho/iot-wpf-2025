using CommunityToolkit.Mvvm.ComponentModel;

namespace WpfBookRentalShop01.Models
{
    public class Rental : ObservableObject
    {
        private int _idx;
        private int _memberidx;
        private int _bookidx;
        private string _booknames;
        private string _membername;
        private DateTime _rentaldate;
        private DateTime _returndate;

        public int Idx
        {
            get => _idx;
            set => SetProperty(ref _idx, value);
        }
        
        public int MemberIdx
        {
            get => _memberidx;
            set => SetProperty(ref _memberidx, value);
        }

        public int BookIdx
        {
            get => _bookidx;
            set => SetProperty(ref _bookidx, value);
        }

        public string BookNames
        {
            get => _booknames;
            set => SetProperty(ref _booknames, value);
        }

        public string MemberName
        {
            get => _membername;
            set => SetProperty(ref _membername, value);
        }

        public DateTime RentalDate
        {
            get => _rentaldate;
            set => SetProperty(ref _rentaldate, value);
        }

        public DateTime ReturnDate
        {
            get => _returndate;
            set => SetProperty(ref _returndate, value);
        }

    }
}
