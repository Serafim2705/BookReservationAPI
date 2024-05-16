using Microsoft.AspNetCore.Mvc;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;
using Swashbuckle.AspNetCore;

namespace Book_Reservation_rest_api.Models
{
    public class Book
    {

        public long Id { get; set; }
        [Required]
        public string? Title { get; set; }
        public string? Author { get; set; }


        public Book(string title, string? author)
        {
            Title = title;

            Author = author;
        }

    }
}
