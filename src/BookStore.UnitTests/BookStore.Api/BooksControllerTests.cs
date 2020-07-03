using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BookStore.Api.Contracts.Queries;
using BookStore.Api.Contracts.Requests;
using BookStore.Api.Contracts.Responses;
using BookStore.Api.Controllers;
using BookStore.Api.Helpers;
using BookStore.Domain;
using BookStore.Service.Interfaces;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Serilog;
using Xunit;

namespace BookStore.UnitTests.BookStore.Api
{
    public class BooksControllerTests
    {
        private readonly Mock<IBookService> _bookServiceMock;
        private readonly Mock<ILinkGenerator> _linkGeneratorMock;

        private readonly BooksController _booksController;

        private const string Category = "Computer";
        private const string BookTitle = "Unit Testing";
        private readonly Guid _bookId = Guid.NewGuid();
        private readonly List<Link> _links;

        public BooksControllerTests()
        {
            var loggerMock = new Mock<ILogger>();
            _bookServiceMock = new Mock<IBookService>();

            _linkGeneratorMock = new Mock<ILinkGenerator>();
            _links = new List<Link>()
            {
                new Link("/api/v1/book/1", "self", "GET"),
                new Link("/api/v1/book/1", "update", "PUT")
            };
            _linkGeneratorMock.Setup(x => x.CreateLinks(It.IsAny<HttpContext>(), It.IsAny<string>(), It.IsAny<string>(),
                It.IsAny<string>(), It.IsAny<Guid>())).Returns(_links);

            _booksController = new BooksController(loggerMock.Object, _bookServiceMock.Object, _linkGeneratorMock.Object);
        }

        [Fact]
        public async Task GetAll_WithoutAnyBooks_ReturnsEmptyResponse()
        {
            // Arrange
            _bookServiceMock.Setup(x => x.GetAllAsync(string.Empty)).ReturnsAsync(new List<Book>());

            // Act
            var response = await _booksController.GetAll(new GetBooksQuery()
            {
                Category = string.Empty
            });

            // Assert
            response.Result.Should().BeOfType<OkObjectResult>();

            var value = ((ObjectResult)response.Result).Value;
            value.Should().BeOfType<BookResponse[]>();

            var data = (IEnumerable<BookResponse>)value;
            data.Should().HaveCount(0);

            VerifyLinkGeneratorHasNeverBeenCalled();
        }
        
        [Fact]
        public async Task GetAll_WithBooks_ReturnsBooksResponse()
        {
            // Arrange
            _bookServiceMock.Setup(x => x.GetAllAsync(Category)).ReturnsAsync(new List<Book>()
            {
                new Book(_bookId, BookTitle, "Computer"),
                new Book(Guid.NewGuid(), "C# in Nutshell", "Computer"),
            });

            // Act
            var response = await _booksController.GetAll(new GetBooksQuery()
            {
                Category = Category
            });

            // Assert
            response.Result.Should().BeOfType<OkObjectResult>();

            var value = ((ObjectResult)response.Result).Value;
            value.Should().BeOfType<BookResponse[]>();

            var books = ((IEnumerable<BookResponse>)value).ToArray();
            books.Should().HaveCount(2);

            books.FirstOrDefault(x => x.Id == _bookId && x.Title == BookTitle && x.Category == Category).Should().NotBeNull();

            VerifyLinkGeneratorHasBeenCalledFor(Times.Exactly(2));
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
            VerifyLinkGeneratorHasNeverBeenCalled();
        }

        [Fact]
        public async Task GetById_WhenBookFound_ReturnsBookResponse()
        {
            // Arrange
            _bookServiceMock.Setup(x => x.GetByIdAsync(_bookId)).ReturnsAsync(new Book(_bookId, BookTitle, Category));

            // Act
            var response = await _booksController.GetById(_bookId);

            // Assert
            response.Result.Should().BeOfType<OkObjectResult>();
            
            AssertBookResponseValue(response, _bookId, BookTitle, Category);
            VerifyLinkGeneratorHasBeenCalledFor(Times.Once(), _bookId);
        }
        
        [Fact]
        public async Task Create_InvalidPayload_ReturnsBadRequestResult()
        {
            // Act
            _bookServiceMock.Setup(x => x.CreateAsync(It.IsAny<Book>())).ReturnsAsync((Book)null);
            var response = await _booksController.Create(new AddBookRequest());

            // Assert
            response.Result.Should().BeOfType<BadRequestResult>();
            VerifyLinkGeneratorHasNeverBeenCalled();
        }

        [Fact]
        public async Task Create_ValidPayload_ReturnsBookResponse()
        {
            // Arrange
            _bookServiceMock.Setup(x => x.CreateAsync(It.IsAny<Book>())).ReturnsAsync(new Book(_bookId, BookTitle, Category)).Callback<Book>(
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

            AssertBookResponseValue(response, _bookId, BookTitle, Category);
            VerifyLinkGeneratorHasBeenCalledFor(Times.Once(), _bookId);
        }

        [Fact]
        public async Task Update_WhenBookNotFound_ReturnsNotFoundResult()
        {
            // Act
            _bookServiceMock.Setup(x => x.UpdateAsync(_bookId, It.IsAny<Book>())).ReturnsAsync((Book)null);
            var response = await _booksController.Update(_bookId, new UpdateBookRequest());

            // Assert
            response.Result.Should().BeOfType<NotFoundResult>();
            VerifyLinkGeneratorHasNeverBeenCalled();
        }

        [Fact]
        public async Task Update_WhenBookFound_ReturnsBookResponse()
        {
            // Arrange
            _bookServiceMock.Setup(x => x.UpdateAsync(_bookId, It.IsAny<Book>())).ReturnsAsync(new Book(_bookId, BookTitle, Category)).Callback<Guid, Book>(
                (id, book) =>
                {
                    id.Should().Be(_bookId);
                    book.Title.Should().Be(BookTitle);
                    book.Category.Should().Be(Category);
                });

            // Act
            var response = await _booksController.Update(_bookId, new UpdateBookRequest()
            {
                Title = BookTitle,
                Category = Category
            });

            // Assert
            response.Result.Should().BeOfType<OkObjectResult>();

            AssertBookResponseValue(response, _bookId, BookTitle, Category);
            VerifyLinkGeneratorHasBeenCalledFor(Times.Once(), _bookId);
        }

        [Fact]
        public async Task Delete_WhenBookNotFound_ReturnsNotFoundResult()
        {
            // Act
            _bookServiceMock.Setup(x => x.DeleteAsync(_bookId)).ReturnsAsync(false);
            var response = await _booksController.Delete(_bookId);

            // Assert
            response.Should().BeOfType<NotFoundResult>();
        }

        [Fact]
        public async Task Delete_WhenBookFound_ReturnsOkResult()
        {
            // Act
            _bookServiceMock.Setup(x => x.DeleteAsync(_bookId)).ReturnsAsync(true);
            var response = await _booksController.Delete(_bookId);

            // Assert
            response.Should().BeOfType<OkResult>();
        }

        private void AssertBookResponse(BookResponse book, Guid bookId, string bookTitle, string category)
        {
            book.Id.Should().Be(bookId);
            book.Title.Should().Be(bookTitle);
            book.Category.Should().Be(category);
            book.Links.Should().BeEquivalentTo(_links);
        }

        private void AssertBookResponseValue(ActionResult<BookResponse> response, Guid bookId, string bookTitle, string category)
        {
            var value = ((ObjectResult)response.Result).Value;
            value.Should().BeOfType<BookResponse>();

            var book = (BookResponse)value;
            AssertBookResponse(book, bookId, bookTitle, category);
        }

        private void VerifyLinkGeneratorHasNeverBeenCalled()
        {
            _linkGeneratorMock.Verify(x => x.CreateLinks(It.IsAny<HttpContext>(), It.IsAny<string>(), It.IsAny<string>(),
                It.IsAny<string>(), It.IsAny<Guid>()), Times.Never);
        }

        private void VerifyLinkGeneratorHasBeenCalledFor(Times times)
        {
            _linkGeneratorMock.Verify(x => x.CreateLinks(It.IsAny<HttpContext>(), nameof(BooksController.GetById), nameof(BooksController.Update),
                nameof(BooksController.Delete), It.IsAny<Guid>()), times);
        }

        private void VerifyLinkGeneratorHasBeenCalledFor(Times times, Guid guid)
        {
            _linkGeneratorMock.Verify(x => x.CreateLinks(It.IsAny<HttpContext>(), nameof(BooksController.GetById), nameof(BooksController.Update),
                nameof(BooksController.Delete), guid), times);
        }
    }
}
