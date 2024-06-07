using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace api.Dtos.Comment
{
    public class CreateCommentDto
    {
        [Required]
        [MinLength(3, ErrorMessage = "Title must be 3 charachters or more")]
        [MaxLength(100, ErrorMessage = "Title cannot be more than 100 characters")]
        public string Title { get; set; } = string.Empty;

        [Required]
        [MaxLength(240, ErrorMessage = "Content cannot be more than 240 characters")]
        public string Content { get; set; } = string.Empty;
    }
}