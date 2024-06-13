using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace api.Models
{
    [Table("Portfolios")]
    public class Portfolio
    {
        public int StockId { get; set; }
        public string AppUserId { get; set; }
        public AppUser AppUser { get; set; } // This is a navigation property
        public Stock Stock { get; set; } // This is a navigation property
    }
}


/*
1. Navigation Properties:

Navigation properties allow you to define relationships between different entities (or tables).
They enable you to access related data. For instance, with the AppUser navigation property in your Portfolio class, you can easily access the user information associated with a specific portfolio entry.


2. AppUser and Stock Navigation Properties:

AppUser: This property refers to an instance of the AppUser class. It represents the user who owns the portfolio. Through this navigation property, you can access all the details of the user (such as username, email, etc.) associated with a specific Portfolio entry.
Stock: This property refers to an instance of the Stock class. It represents the stock included in the portfolio. Through this navigation property, you can access all the details of the stock (such as stock name, price, etc.) associated with a specific Portfolio entry.
*/