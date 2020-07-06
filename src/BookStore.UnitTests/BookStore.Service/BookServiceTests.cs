using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using BookStore.Domain;
using BookStore.Domain.Interfaces;
using BookStore.Service;
using FluentAssertions;
using Moq;
using Xunit;

namespace BookStore.UnitTests.BookStore.Service
{
    public class BookServiceTests
    {
        private readonly Mock<IBookRepository> _bookRepositoryMock;
        private readonly BookService _bookService;
        private readonly Book _dbBook;

        public BookServiceTests()
        {
            _dbBook = new Book(Guid.NewGuid(), "MongoDB in Action", "Database");

            _bookRepositoryMock = new Mock<IBookRepository>();
            _bookService = new BookService(_bookRepositoryMock.Object);
        }

        [Fact]
        public async Task GetAll_ReturnsBooksFromRepository()
        {
            // Arrange
            const string category = "Computer";

            var dbBooks = new List<Book>()
            {
                _dbBook
            };
            
            _bookRepositoryMock.Setup(x => x.GetAllAsync(category)).ReturnsAsync(dbBooks);
            
            // Act
            var books = await _bookService.GetAllAsync(category);

            // Assert
            books.Should().BeEquivalentTo(dbBooks);
        }

        [Fact]
        public async Task GetById_ReturnsBookFromRepository()
        {
            // Arrange
            var bookId = new Guid();
            
            // Act
            _bookRepositoryMock.Setup(x => x.GetByIdAsync(bookId)).ReturnsAsync(_dbBook);

            var book = await _bookService.GetByIdAsync(bookId);

            // Assert
            book.Should().BeEquivalentTo(_dbBook);
        }

        [Fact]
        public async Task Create_ReturnsBookCreatedByRepository()
        {
            // Arrange
            var newBook = new Book("MongoDB in Action", "Database");
            _bookRepositoryMock.Setup(x => x.CreateAsync(newBook)).ReturnsAsync(_dbBook);

            // Act
            var book = await _bookService.CreateAsync(newBook);

            // Assert
            book.Should().BeEquivalentTo(_dbBook);
        }

        [Fact]
        public async Task Update_ReturnsBookUpdatedByRepository()
        {
            // Arrange
            var bookId = new Guid();
            var updatedBook = new Book("MongoDB in Action", "Database");
            _bookRepositoryMock.Setup(x => x.UpdateAsync(bookId, updatedBook)).ReturnsAsync(_dbBook);

            // Act
            var book = await _bookService.UpdateAsync(bookId, updatedBook);

            // Assert
            book.Should().BeEquivalentTo(_dbBook);
        }

        [Fact]
        public async Task Delete_NoRecordDeletedByRepository_ReturnsFalse()
        {
            // Arrange
            var bookId = new Guid();
            _bookRepositoryMock.Setup(x => x.DeleteAsync(bookId)).ReturnsAsync(0);

            // Act
            var result = await _bookService.DeleteAsync(bookId);

            // Assert
            result.Should().BeFalse();
        }

        [Fact]
        public async Task Delete_ARecordDeletedByRepository_ReturnsTrue()
        {
            // Arrange
            var bookId = new Guid();
            _bookRepositoryMock.Setup(x => x.DeleteAsync(bookId)).ReturnsAsync(1);

            // Act
            var result = await _bookService.DeleteAsync(bookId);

            // Assert
            result.Should().BeTrue();
        }
    }
}
