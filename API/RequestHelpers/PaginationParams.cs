namespace API.RequestHelpers
{
    public class PaginationParams
    {
        private const int MaxPageSize = 50; //ค่ำ Default จ ำนวนสูงสุดต่อหน้ำ
        private int _pageSize = 6; //ค่ำ Default จ ำนวนต่อหน้ำ
        public int PageNumber { get; set; } = 1;
        public int PageSize
        {
            get => _pageSize;
            set => _pageSize = value > MaxPageSize ? MaxPageSize : value;
        }

    }
}