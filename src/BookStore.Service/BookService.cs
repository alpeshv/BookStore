using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using BookStore.Domain;
using BookStore.Domain.Interfaces;
using BookStore.Service.Interfaces;

namespace BookStore.Service
{
    public class BookService : IBookService
    {
        private readonly IBookRepository _bookRepository;

        public BookService(IBookRepository bookRepository)
        {
            _bookRepository = bookRepository;
        }

        public async Task<IEnumerable<Book>> GetAllAsync(string category)
        {
            return await _bookRepository.GetAllAsync(category);
        }

        public async Task<Book> GetByIdAsync(Guid id)
        {
            return await _bookRepository.GetByIdAsync(id);
        }

        public async Task<Book> CreateAsync(Book book)
        {
            return await _bookRepository.CreateAsync(book);
        }

        public async Task<Book> UpdateAsync(Guid id, Book book)
        {
            return await _bookRepository.UpdateAsync(id, book);
        }

        public async Task<bool> DeleteAsync(Guid id)
        {
            var deletedRowsCount = await _bookRepository.DeleteAsync(id);

            return deletedRowsCount > 0;
        }
    }
}
