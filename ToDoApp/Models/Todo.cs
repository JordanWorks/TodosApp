using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace ToDoApp.Models
{
    public class Todo
    {
        public int Id { get; set; }

        [Required]
        public string Name { get; set; }

        public bool Visibility { get; set; }

    }
}