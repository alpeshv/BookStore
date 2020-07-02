using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using BookStore.Domain;

namespace BookStore.Service.Interfaces
{
    public interface IBookService
    {
        Task<IEnumerable<Book>> GetAllAsync(string category);
        Task<Book> GetByIdAsync(Guid id);
        Task<Book> CreateAsync(Book book);
        Task<Book> UpdateAsync(Guid id, Book book);
        Task<bool> DeleteAsync(Guid id);
    }
}