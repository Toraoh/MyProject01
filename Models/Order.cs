using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.Data.Entity;


namespace MyProject01.Models
{
    public class Order
    {
        public int ID { get; set; }

        public string BookId { get; set; }

        public string Title { get; set; }

        public string Category { get; set; }

        public string Author { get; set; }

        public decimal Price { get; set; }

        public string Language { get; set; }

        public string OrderId { get; set; }
    }
}