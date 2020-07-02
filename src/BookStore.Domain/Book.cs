using System;

namespace BookStore.Domain
{
    public class Book
    {
        // Normally I would create DDD style domain model with only parameterized constructor and properties with private setters but have to do it this way to support dapper.
        // One way to avoid this is to have separate DAO and Domain models but keeping it simple here since we are doing only simple CRUD operations here
        public Book()
        {

        }

        public Book(string title, string category)
        {
            Title = title;
            Category = category;
        }

        public Book(Guid id, string title, string category):this(title, category)
        {
            Id = id;
        }

        public Guid Id { get; set; }
        public string Title { get; set; }
        public string Category { get; set; }
    }
}
