using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Projects_AdoNet_Books
{
    public static class ConnectionHelper
    {
        public static string ConString
        {
            get
            {
                string dbPath = Path.Combine(Path.GetFullPath(@"..\..\"), "BooksDb.mdf");
                return $@"Data Source=(localdb)\mssqllocaldb;AttachDbFilename={dbPath};Initial Catalog=BooksDb;Trusted_Connection=True";
            }
        }
    }
}
