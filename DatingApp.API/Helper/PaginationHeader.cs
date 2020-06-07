namespace DatingApp.API.Helper
{
    public class PaginationHeader
    {
        public int CurrentPage { get; set; }
        public int ItemsPerPage { get; set; }
        public int TotalItems { get; set; }
        public int TotalPages { get; set; }

        public PaginationHeader(int currentpage, int itemPerPage, int totalItems, int totalPages)
        {
            this.CurrentPage=currentpage;
            this.ItemsPerPage=itemPerPage;
            this.TotalItems=totalItems;
            this.TotalPages=totalPages;
        }
    }
}