using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BookStore.Api.Contracts.Queries;
using BookStore.Api.Contracts.Requests;
using BookStore.Api.Contracts.Responses;
using BookStore.Api.Helpers;
using BookStore.Domain;
using BookStore.Service.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Serilog;

namespace BookStore.Api.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]
    public class BooksController : ControllerBase
    {
        
        private readonly ILogger _logger;
        private readonly IBookService _bookService;
        private readonly ILinkGenerator _linkGenerator;

        public BooksController(ILogger logger, IBookService bookService, ILinkGenerator linkGenerator) 
        {
            _logger = logger;
            _bookService = bookService;
            _linkGenerator = linkGenerator;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<BookResponse>>> GetAll([FromQuery] GetBooksQuery query)
        {
            _logger.Debug("GET api/v1/books");

            var books = await _bookService.GetAllAsync(query.Category);
            
            return Ok(books.Select(b => new BookResponse(b, CreateLinks(b.Id))).ToArray());
        }

        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<BookResponse>> GetById([FromRoute] Guid id)
        {
            _logger.Debug($"GET api/v1/books/{id}");

            var book = await _bookService.GetByIdAsync(id);

            if (book == null)
                return NotFound();

            return Ok(new BookResponse(book, CreateLinks(book.Id)));
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<BookResponse>> Create([FromBody] AddBookRequest request)
        {
            _logger.Debug($"POST api/v1/books");

            var book = new Book(Guid.NewGuid(), request.Title, request.Category);
            var addedBook = await _bookService.CreateAsync(book);

            if (addedBook == null)
                return BadRequest();

            return CreatedAtAction(nameof(GetById), new {id = addedBook.Id}, new BookResponse(addedBook, CreateLinks(addedBook.Id)));
        }

        [HttpPut("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<BookResponse>> Update([FromRoute] Guid id, [FromBody] UpdateBookRequest request)
        {
            _logger.Debug($"PUT api/v1/books/{id}");

            var book = new Book(request.Title, request.Category);
            var updatedBook = await _bookService.UpdateAsync(id, book);

            if (updatedBook == null)
                return NotFound();

            return Ok(new BookResponse(updatedBook, CreateLinks(updatedBook.Id)));
        }

        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Delete([FromRoute] Guid id)
        {
            _logger.Debug($"DELETE api/v1/books/{id}");

            var deleted = await _bookService.DeleteAsync(id);

            if (deleted)
                return Ok();

            return NotFound();
        }

        private IEnumerable<Link> CreateLinks(Guid id)
        {
            return _linkGenerator.CreateLinks(HttpContext, nameof(GetById), nameof(Update), nameof(Delete), id);
        }
    }
}
