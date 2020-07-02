using System.ComponentModel.DataAnnotations;

namespace BookStore.Api.Contracts.Requests
{
    public class UpdateBookRequest
    {
        [Required]
        public string Title { get; set; }

        [Required]
        public string Category { get; set; }
    }
}