using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BookStore.Api.Contracts.Queries;
using BookStore.Api.Contracts.Requests;
using BookStore.Api.Contracts.Responses;
using BookStore.Domain;
using BookStore.Service.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace BookStore.Api.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]
    public class BooksController : ControllerBase
    {
        private readonly IBookService _bookService;

        public BooksController(IBookService bookService)
        {
            _bookService = bookService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<BookResponse>>> GetAll([FromQuery] GetAllBooksQuery query)
        {
            var books = await _bookService.GetAllAsync(query.Category);
            
            return Ok(books.Select(b => new BookResponse(b)).ToArray());
        }

        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<BookResponse>> GetById([FromRoute] Guid id)
        {
            var book = await _bookService.GetByIdAsync(id);

            if (book == null)
                return NotFound();

            return Ok(new BookResponse(book));
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<BookResponse>> Create([FromBody] AddBookRequest request)
        {
            var book = new Book(Guid.NewGuid(), request.Title, request.Category);
            var addedBook = await _bookService.CreateAsync(book);

            if (addedBook == null)
                return BadRequest();

            return CreatedAtAction(nameof(GetById), new { id = addedBook.Id }, new BookResponse(addedBook));
        }

        [HttpPut("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<BookResponse>> Update([FromRoute] Guid id, [FromBody] UpdateBookRequest request)
        {
            var book = new Book(request.Title, request.Category);
            var updatedBook = await _bookService.UpdateAsync(id, book);

            if (updatedBook == null)
                return NotFound();

            return Ok(new BookResponse(updatedBook));
        }

        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Delete([FromRoute] Guid id)
        {
            var deleted = await _bookService.DeleteAsync(id);

            if (deleted)
                return Ok();

            return NotFound();
        }
    }
}
