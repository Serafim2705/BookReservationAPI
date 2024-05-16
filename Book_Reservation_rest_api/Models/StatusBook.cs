using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Book_Reservation_rest_api.Models
{
    public class StatusBook
    {

        [Required]
        [ForeignKey("Book")]
        public long BookId { get; set; }
        public Book Book { get; set; }
        [Required]
        public DateTime Date { get; set; }
        [Required]
        public bool IsReserved { get; set; }

        public string? Comment { get; set; }
        public StatusBook() { }

        public StatusBook(long book_id, DateTime date, bool is_reserved, string? comment)
        {

            BookId = book_id;
            Date = date;
            IsReserved = is_reserved;
            Comment = comment;

        }

    }
}
