using System;
using BookStore.Domain;

namespace BookStore.Api.Contracts.Responses
{
    public class BookResponse
    {
        public BookResponse(Book book)
        {
            Id = book.Id;
            Title = book.Title;
            Category = book.Category;
        }

        public Guid Id { get; }
        public string Title { get; }
        public string Category { get; }
    }
}
