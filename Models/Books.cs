//------------------------------------------------------------------------------
// <auto-generated>
//     此代码已从模板生成。
//
//     手动更改此文件可能导致应用程序出现意外的行为。
//     如果重新生成代码，将覆盖对此文件的手动更改。
// </auto-generated>
//------------------------------------------------------------------------------

namespace MyProject01.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class Books
    {
        public int ID { get; set; }
        public System.Guid BookId { get; set; }
        public string Title { get; set; }
        public string Category { get; set; }
        public string Author { get; set; }
        public decimal Price { get; set; }
        public string Language { get; set; }
    }
}