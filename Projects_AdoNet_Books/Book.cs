using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Projects_AdoNet_Books
{
    public class Book
    {
        public int BookId { get; set; }
        public string Title { get; set; }
        public string Author { get; set; }
        public bool Available { get; set; }
        public DateTime PublishDate { get; set; }
        public string CoverPage { get; set; }
    }
}
