using Book_Reservation_rest_api.DBContext;
using Book_Reservation_rest_api.Models;
using Book_Reservation_rest_api.Schemes;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

namespace Book_Reservation_rest_api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StatusBooksController : ControllerBase
    {
        private readonly BookContext _context;

        public StatusBooksController(BookContext context)
        {
            _context = context;
        }

        // GET: api/StatusBooks
        /// <summary>
        /// Get list of available/reserved books.
        /// </summary>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<GetBookResponse>>> GetStatusOfBooks([FromQuery] bool reserved = false)
        {

            var isReservedParam = new Microsoft.Data.Sqlite.SqliteParameter("IsReservedParam", reserved);


            string sqlQueryStr = $@"WITH temp_tbl as
            (
	            SELECT
		            StatusOfBooks.BookId as BookId,
		            MAX(StatusOfBooks.Date) as max_date,
		            StatusOfBooks.IsReserved as IsReserved,
		            StatusOfBooks.Comment as Comment
	            FROM
		            StatusOfBooks
	            GROUP by
		            StatusOfBooks.BookId
            )

            SELECT
	            Books.Author as Author,
	            Books.Title as Title,
	            Books.Id as BookId,
	            temp_tbl.comment as Comment,
	            temp_tbl.max_date as Date,
	            temp_tbl.IsReserved as IsReserved
            FROM 
	            Books
            JOIN temp_tbl ON Books.Id=temp_tbl.BookId
            WHERE temp_tbl.IsReserved = @IsReservedParam";


            var result = await _context.StatusOfBooks
                .FromSqlRaw(sqlQueryStr, isReservedParam)
                .Include(p => p.Book)
                .ToListAsync();


            return Ok(result);

        }

        // GET: api/StatusBooks/5
        /// <summary>
        /// Get history of specific book's status.
        /// </summary>
        [HttpGet("{id}")]
        public async Task<ActionResult<IEnumerable<GetHistoryResponse>>> GetStatusBook(long id)
        {
            var statuses_Book = await _context.StatusOfBooks
                //.Include(p=>p.Book)
                .Where(m => m.BookId == id)
                .OrderByDescending(m => m.Date)
                .ToListAsync();

            if (statuses_Book.IsNullOrEmpty())
            {
                return NotFound();
            }

            IEnumerable<GetHistoryResponse> ResponseBody = statuses_Book.Select(a => new GetHistoryResponse
            {
                // Тут вы можете скопировать данные из объекта ClassA в новый объект ClassB
                Date = a.Date,
                Comment = a.Comment,
                IsReserved = a.IsReserved
            });

            return Ok(ResponseBody);
        }

        // POST: api/StatusBooksCreate

        /// <summary>
        /// Create a new reservation record.
        /// </summary>
        [HttpPost]
        [Route("Create")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<StatusBook>> PostStatusBook(CustomPostChangeStatus request)
        {
            if (!ModelState.IsValid || request.BookId == 0)
            {
                return BadRequest(ModelState);
            }
            Console.WriteLine(request.Comment);
            var book = await _context.Books.FindAsync(request.BookId);

            if (book == null)
            {
                return NotFound();
            }

            var cur_status = _context.StatusOfBooks.Where(m => m.BookId == request.BookId).OrderByDescending(m => m.Date).FirstOrDefault();

            if (cur_status == null)
            {
                return StatusCode(500, "Внутренняя ошибка сервера");

            }
            if (cur_status.IsReserved)
            {
                return BadRequest("Книга уже зарезервирована");
            }


            var new_status = new StatusBook(request.BookId, DateTime.Now, true, request.Comment);

            _context.StatusOfBooks.Add(new_status);

            await _context.SaveChangesAsync();
            Console.WriteLine(new_status.BookId);

            return CreatedAtAction("GetStatusBook", new { Id = new_status.BookId, Date = new_status.Date }, new_status);
        }



        // POST: api/PostStatusBookCancelation

        /// <summary>
        /// Create book cancellation request.
        /// </summary>
        [HttpPost]
        [Route("Cancel")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<StatusBook>> PostStatusBookCancelation(CustomPostChangeStatus request)
        {
            if (!ModelState.IsValid || request.BookId == 0)
            {

                return BadRequest(ModelState);
            }


            var book = await _context.Books.FindAsync(request.BookId);

            if (book == null)
            {
                return NotFound();
            }

            var cur_status = _context.StatusOfBooks.Where(m => m.BookId == request.BookId).OrderByDescending(m => m.Date).FirstOrDefault();

            if (cur_status == null)
            {
                Console.WriteLine("Вероятно нарушена целостность данных");
                return StatusCode(500, "Внутренняя ошибка сервера");

            }
            if (!cur_status.IsReserved)
            {
                return BadRequest("Книга не зарезервирована!");
            }


            var new_status = new StatusBook(request.BookId, DateTime.Now, false, request.Comment);

            _context.StatusOfBooks.Add(new_status);

            await _context.SaveChangesAsync();

            return CreatedAtAction("GetStatusBook", new { Id = new_status.BookId, Date = new_status.Date }, new_status);
        }

    }
}
