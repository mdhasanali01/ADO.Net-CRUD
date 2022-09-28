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
    public partial class AddBooks : Form
    {
        string filePath = "";
        List<Book> books = new List<Book>();
        public AddBooks()
        {
            InitializeComponent();
        }
        public ICrossDataSync FormToLoad { get; set; }

        private void AddBooks_Load(object sender, EventArgs e)
        {
            this.textBox1.Text = this.GetNewBookId().ToString();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            using (SqlConnection con = new SqlConnection(ConnectionHelper.ConString))
            {
                con.Open();
                using (SqlTransaction tran = con.BeginTransaction())
                {

                    using (SqlCommand cmd = new SqlCommand(@"INSERT INTO books 
                                            (bookId, title, author,available,published_date, picture) VALUES
                                            (@i, @t, @a, @av, @p, @pi)", con, tran))
                    {
                        cmd.Parameters.AddWithValue("@i", int.Parse(textBox1.Text));
                        cmd.Parameters.AddWithValue("@t", textBox2.Text);
                        cmd.Parameters.AddWithValue("@a", textBox3.Text);
                        cmd.Parameters.AddWithValue("@av", checkBox1.Checked);
                        cmd.Parameters.AddWithValue("@p", dateTimePicker1.Value);
                        string ext = Path.GetExtension(this.filePath);
                        string fileName = $"{Guid.NewGuid()}{ext}";
                        string savePath = Path.Combine(Path.GetFullPath(@"..\..\Pictures"), fileName);
                        File.Copy(filePath, savePath, true);
                        cmd.Parameters.AddWithValue("@pi", fileName);

                        try
                        {
                            if (cmd.ExecuteNonQuery() > 0)
                            {
                                MessageBox.Show("Data Saved", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                                books.Add(new Book
                                {
                                    BookId = int.Parse(textBox1.Text),
                                    Title = textBox2.Text,
                                    Author = textBox3.Text,
                                    Available = checkBox1.Checked,
                                    PublishDate = dateTimePicker1.Value,
                                    CoverPage = fileName
                                }); ;
                                tran.Commit();
                            }
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show($"Error: {ex.Message}", "Success", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            tran.Rollback();
                        }
                        finally
                        {
                            if (con.State == ConnectionState.Open)
                            {
                                con.Close();
                            }
                        }


                    }
                }
            }
        }
            private int GetNewBookId()
            {
                using (SqlConnection con = new SqlConnection(ConnectionHelper.ConString))
                {
                    using (SqlCommand cmd = new SqlCommand("SELECT ISNULL(MAX(bookid), 0) FROM Books", con))
                    {
                        con.Open();
                        int id = (int)cmd.ExecuteScalar();
                        con.Close();
                        return id + 1;
                    }
                }
            }

        private void button2_Click(object sender, EventArgs e)
        {
            if (this.openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                this.filePath = this.openFileDialog1.FileName;
                this.label7.Text = Path.GetFileName(this.filePath);
                this.pictureBox1.Image = Image.FromFile(this.filePath);
            }
        }

        private void AddBooks_FormClosing(object sender, FormClosingEventArgs e)
        {
            this.FormToLoad.ReloadData(this.books);
        }
    }
}
