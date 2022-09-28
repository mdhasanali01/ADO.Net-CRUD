using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Projects_AdoNet_Books
{
    public interface ICrossDataSync
    {
        void ReloadData(List<Book> books);
        void UpdateBook(Book book);
        void RemoveBook(int id);
    }
}
