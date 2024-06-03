using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace api.Models
{
    public class Comment
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Content { get; set; } =  string.Empty;
        public DateTime CreatedOn { get; set; } = DateTime.Now;
        public int? StockId { get; set; }

        /*
        - navigation property - allows us to navigate within or model
        - allows us to dot notation into the properties of the related table.
        - usecase examle Stock.CompanyName
        - if you look in the stock model/table, theres a prop called CompanyName
        */
        public Stock? Stock { get; set; }
    }
}