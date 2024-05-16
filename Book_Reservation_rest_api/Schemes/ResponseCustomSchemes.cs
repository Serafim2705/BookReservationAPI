namespace Book_Reservation_rest_api.Schemes
{

    public class GetBookResponse
    {
        public long Id { get; set; }
        public string? Title { get; set; }
        public string? Author { get; set; }
        public DateTime Date { get; set; }
        public bool IsReserved { get; set; }
        public string? Comment { get; set; }

        public GetBookResponse(long id, string? title, string? author, DateTime date, bool isReserved, string? comment)
        {
            Id = id;
            Title = title;
            Author = author;
            Date = date;
            IsReserved = isReserved;
            Comment = comment;
        }
    }

    public class GetHistoryResponse
    {
        public DateTime Date { get; set; }
        public string? Comment { get; set; }

        public bool IsReserved { get; set; }


    }

}
