using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Projects_AdoNet_Books
{
    public partial class Form1 : Form, ICrossDataSync
    {
        DataSet ds;
        BindingSource bsBooks = new BindingSource();
        BindingSource bsReviews = new BindingSource();
        public Form1()
        {
            InitializeComponent();
        }

        private void toolStripButton2_Click(object sender, EventArgs e)
        {
            int id = (int)(this.bsBooks.Current as DataRowView).Row[0];
            new EditBook { BookToEditDelete = id, FormToReload = this }.ShowDialog();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            this.dataGridView1.AutoGenerateColumns = false;
            LoadData();
            BindData();
        }
        public void LoadData()
        {
            ds = new DataSet();
            using (SqlConnection con = new SqlConnection(ConnectionHelper.ConString))
            {
                using (SqlDataAdapter da = new SqlDataAdapter("SELECT * FROM Books", con))
                {
                    da.Fill(ds, "Books");
                    ds.Tables["Books"].Columns.Add(new DataColumn("image", typeof(System.Byte[])));
                    for (var i = 0; i < ds.Tables["Books"].Rows.Count; i++)
                    {
                        ds.Tables["Books"].Rows[i]["image"] = File.ReadAllBytes(Path.Combine(Path.GetFullPath(@"..\..\Pictures"), ds.Tables["Books"].Rows[i]["picture"].ToString()));
                    }
                    da.SelectCommand.CommandText = "SELECT * FROM Reviews";
                    da.Fill(ds, "Reviews");
                    DataRelation rel = new DataRelation("FK_BOOK_Reviews",
                        ds.Tables["Books"].Columns["bookId"],
                        ds.Tables["Reviews"].Columns["bookId"]);
                    ds.Relations.Add(rel);
                    ds.AcceptChanges();
                }
            }
        }
        private void BindData()
        {
            bsBooks.DataSource = ds;
            bsBooks.DataMember = "Books";
            bsReviews.DataSource = bsBooks;
            bsReviews.DataMember = "FK_BOOK_Reviews";
            this.dataGridView1.DataSource = bsReviews;
            lblTitle.DataBindings.Add(new Binding("Text", bsBooks, "title"));
            lblAutor.DataBindings.Add(new Binding("Text", bsBooks, "author"));
            checkBox1.DataBindings.Add(new Binding("Checked", bsBooks, "available"));
            lblDate.DataBindings.Add(new Binding("Text", bsBooks, "published_date",true));
            pictureBox1.DataBindings.Add(new Binding("Image", bsBooks, "image", true));
        }

        public void ReloadData(List<Book> Books)
        {
            foreach (var b in Books)
            {
                DataRow dr = ds.Tables["Books"].NewRow();
                dr[0] = b.BookId;
                dr["title"] = b.Title;
                dr["author"] = b.Author;
                dr["available"] = b.Available;
                dr["published_date"] = b.PublishDate;
                dr["picture"] = b.CoverPage;
                dr["image"] = File.ReadAllBytes(Path.Combine(Path.GetFullPath(@"..\..\Pictures"), b.CoverPage));
                ds.Tables["Books"].Rows.Add(dr);

            }
            ds.AcceptChanges();
            bsBooks.MoveLast();
        }

        public void UpdateBook(Book b)
        {
            for (var i = 0; i < ds.Tables["Books"].Rows.Count; i++)
            {
                if ((int)ds.Tables["books"].Rows[i]["bookId"] == b.BookId)
                {
                    ds.Tables["Books"].Rows[i]["title"] = b.Title;
                    ds.Tables["Books"].Rows[i]["publishdate"] = b.PublishDate;
                    ds.Tables["Books"].Rows[i]["image"] = File.ReadAllBytes(Path.Combine(Path.GetFullPath(@"..\..\Pictures"), b.CoverPage));
                    break;
                }
            }
            ds.AcceptChanges();
        }

        public void RemoveBook(int id)
        {
            for (var i = 0; i < ds.Tables["Books"].Rows.Count; i++)
            {
                if ((int)ds.Tables["Books"].Rows[i]["bookid"] == id)
                {
                    ds.Tables["Books"].Rows.RemoveAt(i);
                    break;
                }
            }
            ds.AcceptChanges();
        }

        private void addToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            new AddBooks { FormToLoad = this }.ShowDialog();
        }

        private void editDeleteToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            int id = (int)(this.bsBooks.Current as DataRowView).Row[0];
            new EditBook { BookToEditDelete = id, FormToReload = this }.ShowDialog();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (bsBooks.Position > 0)
            {
                bsBooks.MovePrevious();
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (bsBooks.Position < bsBooks.Count - 1)
            {
                bsBooks.MoveNext();
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            bsBooks.MoveFirst();
        }

        private void button4_Click(object sender, EventArgs e)
        {
            bsBooks.MoveLast();
        }

        private void addToolStripMenuItem_Click(object sender, EventArgs e)
        {
            new AddReview { OpenerForm = this }.ShowDialog();
        }

        private void editDeleteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            new EditReview { OpenerForm = this }.ShowDialog();
        }

        private void booksToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            new FormBooks().ShowDialog();
        }

        private void booksReviewsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            new FormBooksGroupRpt().ShowDialog();
        }
    }
}
