using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using BookStore.Domain;
using BookStore.Domain.Interfaces;
using Dapper;

namespace BookStore.DataAccess
{
    public class BookRepository : IBookRepository
    {
        private readonly IDbConnection _dbConnection;

        public BookRepository(IDbConnection dbConnection)
        {
            _dbConnection = dbConnection;
        }

        public async Task<IEnumerable<Book>> GetAllAsync(string category)
        {
            var query = "SELECT * from Books";

            if (!string.IsNullOrWhiteSpace(category))
            {
                query = $"{query} WHERE Category = @category";
            }

            return await _dbConnection.QueryAsync<Book>(query, new {category});
        }

        public async Task<Book> GetByIdAsync(Guid id)
        {
            return await _dbConnection.QueryFirstOrDefaultAsync<Book>(
                @"  
                    SELECT 
                        Id, 
                        Title,
                        Category
                    FROM 
                        Books 
                    WHERE 
                        Id = @Id
                 ", 
                new
                {
                    Id = id
                });
        }

        public async Task<Book> CreateAsync(Book book)
        {
            var query = @"
                    INSERT INTO 
                        Books 
                    VALUES
                    (
                        @Id, 
                        @Title, 
                        @Category
                    )
                ";

            var addedCount = await _dbConnection.ExecuteAsync(query, book);

            if (addedCount > 0)
            {
                return await GetByIdAsync(book.Id);
            }

            return null;
        }

        public async Task<Book> UpdateAsync(Guid id, Book book)
        {
            var query = @"
                    UPDATE 
                        Books 
                    SET 
                        Title = @title,
                        Category = @Category
                    WHERE 
                        Id = @Id
                ";

            var updatedCount = await _dbConnection.ExecuteAsync(
                query, 
                new { 
                    Id = id, 
                    book.Title, 
                    book.Category
                });

            if (updatedCount > 0)
            {
                return await GetByIdAsync(id);
            }

            return null;
        }

        public async Task<int> DeleteAsync(Guid id)
        {
            var query = @"  
                    DELETE
                    FROM 
                        Books 
                    WHERE 
                        Id = @Id
                 ";

            return await _dbConnection.ExecuteAsync(
                query,
                new
                {
                    Id = id
                });
        }
    }
}
