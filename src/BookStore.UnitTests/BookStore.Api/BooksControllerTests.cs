using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using BookStore.Api.Contracts.Queries;
using BookStore.Api.Contracts.Requests;
using BookStore.Api.Contracts.Responses;
using BookStore.Api.Controllers;
using BookStore.Domain;
using BookStore.Service.Interfaces;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;

namespace BookStore.UnitTests.BookStore.Api
{
    public class BooksControllerTests
    {
        private readonly Mock<IBookService> _bookServiceMock;
        private readonly BooksController _booksController;

        const string Category = "Computer";
        const string BookTitle = "Unit Testing";
        readonly Guid bookId = Guid.NewGuid();

        public BooksControllerTests()
        {
            _bookServiceMock = new Mock<IBookService>();
            _booksController = new BooksController(_bookServiceMock.Object);
        }

        [Fact]
        public async Task GetAll_WithoutAnyBooks_ReturnsEmptyResponse()
        {
            // Arrange
            _bookServiceMock.Setup(x => x.GetAllAsync(string.Empty)).ReturnsAsync(new List<Book>());

            // Act
            var response = await _booksController.GetAll(new GetAllBooksQuery()
            {
                Category = string.Empty
            });

            // Assert
            response.Result.Should().BeOfType<OkObjectResult>();

            var value = ((ObjectResult)response.Result).Value;
            value.Should().BeOfType<BookResponse[]>();

            var data = (IEnumerable<BookResponse>)value;
            data.Should().HaveCount(0);
        }

        [Fact]
        public async Task GetAll_WithBooks_ReturnsBooksResponse()
        {
            // Arrange
            _bookServiceMock.Setup(x => x.GetAllAsync(Category)).ReturnsAsync(new List<Book>()
            {
                new Book(bookId, BookTitle, "Computer"),
                new Book(Guid.NewGuid(), "C# in Nutshell", "Computer"),
            });

            // Act
            var response = await _booksController.GetAll(new GetAllBooksQuery()
            {
                Category = Category
            });

            // Assert
            response.Result.Should().BeOfType<OkObjectResult>();

            var value = ((ObjectResult)response.Result).Value;
            value.Should().BeOfType<BookResponse[]>();

            var books = ((IEnumerable<BookResponse>)value).ToArray();
            books.Should().HaveCount(2);

            books.FirstOrDefault(x => x.Id == bookId && x.Title == BookTitle && x.Category == Category).Should().NotBeNull();
        }

        [Fact]
        public async Task GetById_WhenBookNotFound_ReturnsNotFoundResponse()
        {
            // Arrange
            var guid = Guid.NewGuid();
            _bookServiceMock.Setup(x => x.GetByIdAsync(guid)).ReturnsAsync((Book)null);

            // Act
            var response = await _booksController.GetById(guid);

            // Assert
            response.Result.Should().BeOfType<NotFoundResult>();
        }

        [Fact]
        public async Task GetById_WhenBookFound_ReturnsBookResponse()
        {
            // Arrange
            _bookServiceMock.Setup(x => x.GetByIdAsync(bookId)).ReturnsAsync(new Book(bookId, BookTitle, Category));

            // Act
            var response = await _booksController.GetById(bookId);

            // Assert
            response.Result.Should().BeOfType<OkObjectResult>();

            AssertBookResponseValue(response, bookId, BookTitle, Category);
        }

        private static void AssertBookResponseValue(ActionResult<BookResponse> response, Guid bookId, string bookTitle, string category)
        {
            var value = ((ObjectResult)response.Result).Value;
            value.Should().BeOfType<BookResponse>();

            var book = (BookResponse)value;
            AssertBookResponse(book, bookId, bookTitle, category);
        }

        [Fact]
        public async Task Create_InvalidPayload_ReturnsBadRequestResult()
        {
            // Act
            _bookServiceMock.Setup(x => x.CreateAsync(It.IsAny<Book>())).ReturnsAsync((Book)null);
            var response = await _booksController.Create(new AddBookRequest());

            // Assert
            response.Result.Should().BeOfType<BadRequestResult>();
        }

        [Fact]
        public async Task Create_ValidPayload_ReturnsBookResponse()
        {
            // Arrange
            _bookServiceMock.Setup(x => x.CreateAsync(It.IsAny<Book>())).ReturnsAsync(new Book(bookId, BookTitle, Category)).Callback<Book>(
                (book) =>
                {
                    book.Title.Should().Be(BookTitle);
                    book.Category.Should().Be(Category);
                });

            // Act
            var response = await _booksController.Create(new AddBookRequest()
            {
                Title = BookTitle,
                Category = Category
            });

            // Assert
            response.Result.Should().BeOfType<CreatedAtActionResult>();

            AssertBookResponseValue(response, bookId, BookTitle, Category);
        }

        [Fact]
        public async Task Update_WhenBookNotFound_ReturnsNotFoundResult()
        {
            // Act
            _bookServiceMock.Setup(x => x.UpdateAsync(bookId, It.IsAny<Book>())).ReturnsAsync((Book)null);
            var response = await _booksController.Update(bookId, new UpdateBookRequest());

            // Assert
            response.Result.Should().BeOfType<NotFoundResult>();
        }

        [Fact]
        public async Task Update_WhenBookFound_ReturnsBookResponse()
        {
            // Arrange
            _bookServiceMock.Setup(x => x.UpdateAsync(bookId, It.IsAny<Book>())).ReturnsAsync(new Book(bookId, BookTitle, Category)).Callback<Guid, Book>(
                (id, book) =>
                {
                    id.Should().Be(bookId);
                    book.Title.Should().Be(BookTitle);
                    book.Category.Should().Be(Category);
                });

            // Act
            var response = await _booksController.Update(bookId, new UpdateBookRequest()
            {
                Title = BookTitle,
                Category = Category
            });

            // Assert
            response.Result.Should().BeOfType<OkObjectResult>();

            AssertBookResponseValue(response, bookId, BookTitle, Category);
        }

        [Fact]
        public async Task Delete_WhenBookNotFound_ReturnsNotFoundResult()
        {
            // Act
            _bookServiceMock.Setup(x => x.DeleteAsync(bookId)).ReturnsAsync(false);
            var response = await _booksController.Delete(bookId);

            // Assert
            response.Should().BeOfType<NotFoundResult>();
        }

        [Fact]
        public async Task Delete_WhenBookFound_ReturnsOkResult()
        {
            // Act
            _bookServiceMock.Setup(x => x.DeleteAsync(bookId)).ReturnsAsync(true);
            var response = await _booksController.Delete(bookId);

            // Assert
            response.Should().BeOfType<OkResult>();
        }

        private static void AssertBookResponse(BookResponse book, Guid bookId, string bookTitle, string category)
        {
            book.Id.Should().Be(bookId);
            book.Title.Should().Be(bookTitle);
            book.Category.Should().Be(category);
        }
    }
}
