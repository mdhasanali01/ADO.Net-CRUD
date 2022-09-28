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
    public partial class EditBook : Form
    {
        string filePath, oldFile, fileName;
        string action = "Edit";
        Book book;
        public EditBook()
        {
            InitializeComponent();
        }
        public int BookToEditDelete { get; set; }
        public ICrossDataSync FormToReload { get; set; }

        private void button1_Click(object sender, EventArgs e)
        {
            this.action = "Edit";
            using (SqlConnection con = new SqlConnection(ConnectionHelper.ConString))
            {
                con.Open();
                using (SqlTransaction tran = con.BeginTransaction())
                {

                    using (SqlCommand cmd = new SqlCommand(@"UPDATE  Books  
                                            SET title=@t, author=@a, available=@av,published_date=@p,picture=@pi 
                                            WHERE bookId=@i", con, tran))
                    {
                        cmd.Parameters.AddWithValue("@i", int.Parse(textBox1.Text));
                        cmd.Parameters.AddWithValue("@t", textBox2.Text);
                        cmd.Parameters.AddWithValue("@a", textBox3.Text);
                        cmd.Parameters.AddWithValue("@av", checkBox1.Checked);
                        cmd.Parameters.AddWithValue("@p", dateTimePicker1.Value);


                        if (!string.IsNullOrEmpty(this.filePath))
                        {
                            string ext = Path.GetExtension(this.filePath);
                            fileName = $"{Guid.NewGuid()}{ext}";
                            string savePath = Path.Combine(Path.GetFullPath(@"..\..\Pictures"), fileName);
                            File.Copy(filePath, savePath, true);
                            cmd.Parameters.AddWithValue("@pi", fileName);
                        }
                        else
                        {
                            cmd.Parameters.AddWithValue("@pi", oldFile);
                        }


                        try
                        {
                            if (cmd.ExecuteNonQuery() > 0)
                            {
                                MessageBox.Show("Data Saved", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                                book = new Book
                                {
                                    BookId = int.Parse(textBox1.Text),
                                    Title = textBox2.Text,
                                    Author = textBox3.Text,
                                    Available = checkBox1.Checked,
                                    PublishDate = dateTimePicker1.Value,
                                    CoverPage = filePath == "" ? oldFile : fileName
                                };
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

        private void button3_Click(object sender, EventArgs e)
        {
            ShowData();
        }

        private void EditBook_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (this.action == "edit")
                this.FormToReload.UpdateBook(book);
            else
                this.FormToReload.RemoveBook(Int32.Parse(this.textBox1.Text));
        }

        private void button4_Click(object sender, EventArgs e)
        {

            this.action = "Delete";
            using (SqlConnection con = new SqlConnection(ConnectionHelper.ConString))
            {
                con.Open();
                using (SqlTransaction tran = con.BeginTransaction())
                {

                    using (SqlCommand cmd = new SqlCommand(@"DELETE  Books  
                                            WHERE bookId=@i", con, tran))
                    {
                        cmd.Parameters.AddWithValue("@i", int.Parse(textBox1.Text));



                        try
                        {
                            if (cmd.ExecuteNonQuery() > 0)
                            {
                                MessageBox.Show("Data Saved", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);

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

        private void button2_Click(object sender, EventArgs e)
        {
            if (this.openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                this.filePath = this.openFileDialog1.FileName;
                this.label7.Text = Path.GetFileName(this.filePath);
                this.pictureBox1.Image = Image.FromFile(this.filePath);
            }
        }

        private void EditBook_Load(object sender, EventArgs e)
        {
            ShowData();
        }

        private void ShowData()
        {
            using (SqlConnection con = new SqlConnection(ConnectionHelper.ConString))
            {
                using (SqlCommand cmd = new SqlCommand("SELECT * FROM Books WHERE bookId =@i", con))
                {
                    cmd.Parameters.AddWithValue("@i", this.BookToEditDelete);
                    con.Open();
                    var dr = cmd.ExecuteReader();
                    if (dr.Read())
                    {
                        textBox1.Text = dr.GetInt32(0).ToString();
                        textBox2.Text = dr.GetString(1);
                        textBox3.Text = dr.GetString(2);
                        checkBox1.Checked = dr.GetBoolean(3);
                        dateTimePicker1.Value = dr.GetDateTime(4);
                        oldFile = dr.GetString(5).ToString();
                        pictureBox1.Image = Image.FromFile(Path.Combine(@"..\..\Pictures", dr.GetString(5).ToString()));
                    }
                    con.Close();
                }
            }
        }
    }
}
