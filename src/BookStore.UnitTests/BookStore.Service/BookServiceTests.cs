using BookStore.Domain;
using BookStore.Domain.Interfaces;
using BookStore.Service;
using FluentAssertions;
using Moq;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

namespace BookStore.UnitTests.BookStore.Service
{
    public class BookServiceTests
    {
        private Mock<IBookRepository> _bookRepositoryMock;
        private BookService _bookService;
        
        public BookServiceTests()
        {
            _bookRepositoryMock = new Mock<IBookRepository>();
            _bookService = new BookService(_bookRepositoryMock.Object);
        }

        [Fact]
        public async Task GetAll_ReturnsBooks()
        {
            // Arrange
            const string category = "Computer";

            var dbbooks = new List<Book>()
            {
                new Book("Test", "Test")
            };

            _bookRepositoryMock.Setup(x => x.GetAllAsync(category)).ReturnsAsync(dbbooks);
            
            var books = await _bookService.GetAllAsync(category);

            // Assert
            books.Should().BeEquivalentTo(dbbooks);
        }

        // Not adding tests for rest of the BookService methods because of time limit.
    }
}
