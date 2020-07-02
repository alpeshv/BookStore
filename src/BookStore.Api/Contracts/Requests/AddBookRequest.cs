using System.ComponentModel.DataAnnotations;

namespace BookStore.Api.Contracts.Requests
{
    public class AddBookRequest
    {
        [Required]
        public string Title { get; set; }

        [Required]
        public string Category { get; set; }
    }
}