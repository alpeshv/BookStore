using System;
using System.Collections.Generic;
using BookStore.Domain;

namespace BookStore.Api.Contracts.Responses
{
    public class BookResponse
    {
        public BookResponse(Book book, IEnumerable<Link> links)
        {
            Id = book.Id;
            Title = book.Title;
            Category = book.Category;
            Links = links;
        }

        public Guid Id { get; }
        public string Title { get; }
        public string Category { get; }
        public IEnumerable<Link> Links { get; set; }
    }
}
