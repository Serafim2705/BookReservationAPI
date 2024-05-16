using System.ComponentModel.DataAnnotations;

namespace Book_Reservation_rest_api.Schemes
{

    public class CustomPostChangeStatus
    {
        [Required]
        public long BookId { get; set; }
        public string? Comment { get; set; }

    }
    public class CustomPostCreateBook
    {
        [Required]
        public string? Title { get; set; }

        public string? Author { get; set; }

    }
}
