namespace FakeXieCheng.API.ResourceParameters
{
    public class PaginationResourceParameters
    {
        private int _pageNumber = 1;
        public int PageNumber
        {
            get => _pageNumber;
            set
            {
                if (value >= 1) _pageNumber = value;
            }
        }

        private int _pageSize = 10;
        private const int MaxPageSize = 50;
        public int PageSize
        {
            get => _pageSize;
            set
            {
                if (value >= 1) _pageSize = (value > MaxPageSize) ? MaxPageSize : value;
            }
        }
    }
}