using Book_Reservation_rest_api.DBContext;
using Book_Reservation_rest_api.Models;
using Book_Reservation_rest_api.Schemes;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Book_Reservation_rest_api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BooksController : ControllerBase
    {
        private readonly BookContext _context;

        public BooksController(BookContext context)
        {
            _context = context;
        }

        // GET: api/Books
        /// <summary>
        /// Get all Books.
        /// </summary>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Book>>> GetBooks()
        {
            return await _context.Books.ToListAsync();
        }

        // GET: api/Books/5
        /// <summary>
        /// Get Book at id.
        /// </summary>
        [HttpGet("{id}")]
        public async Task<ActionResult<Book>> GetBook(long id)
        {
            var book = await _context.Books.FindAsync(id);

            if (book == null)
            {
                return NotFound();
            }

            return book;
        }

        // PUT: api/Books/5
        /// <summary>
        /// Update Book at id.
        /// </summary>
        [HttpPut("{id}")]
        public async Task<IActionResult> PutBook(long id, Book book)
        {
            if (id != book.Id)
            {
                return BadRequest();
            }

            _context.Entry(book).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!BookExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/Books

        /// <summary>
        /// Create a new Book.
        /// </summary>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<Book>> PostBook(CustomPostCreateBook request)
        {

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (request.Title == null)
            {
                return BadRequest("Пропущено название книги");
            }

            var new_book = new Book(request.Title, request.Author);
            _context.Books.Add(new_book);
            await _context.SaveChangesAsync();
            var status_for_new_book = new StatusBook(new_book.Id, DateTime.Now, false, "Добавлена новая книга");
            _context.StatusOfBooks.Add(status_for_new_book);
            //new StatusBook(book.Id, DateTime.Now, false, "some comment")
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetBook", new { id = new_book.Id }, new_book);
        }

        // DELETE: api/Books/5
        /// <summary>
        /// Delete  Book at id.
        /// </summary>
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteBook(long id)
        {
            var book = await _context.Books.FindAsync(id);
            if (book == null)
            {
                return NotFound();
            }

            _context.Books.Remove(book);
            var recordsToDelete = _context.StatusOfBooks.Where(m => m.BookId == id).ToList();
            foreach (var record in recordsToDelete)
            {
                _context.StatusOfBooks.Remove(record);
            }

            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool BookExists(long id)
        {
            return _context.Books.Any(e => e.Id == id);
        }
    }
}
