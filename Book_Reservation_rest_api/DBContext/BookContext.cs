using Book_Reservation_rest_api.Models;
using Microsoft.EntityFrameworkCore;

namespace Book_Reservation_rest_api.DBContext
{
    public class BookContext : DbContext
    {
        public BookContext(DbContextOptions<BookContext> options)
            : base(options)
        {
            ChangeTracker.QueryTrackingBehavior = QueryTrackingBehavior.NoTracking;
        }


        public DbSet<Book> Books { get; set; } = null!;

        public DbSet<StatusBook> StatusOfBooks { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<StatusBook>()
                .HasKey(hnk => new { hnk.Date, hnk.BookId });
        }

    }
}
