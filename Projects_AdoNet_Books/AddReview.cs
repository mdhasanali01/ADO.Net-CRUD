using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Projects_AdoNet_Books
{
    public partial class AddReview : Form
    {
        public AddReview()
        {
            InitializeComponent();
        }
        public Form1 OpenerForm { get; set; }
        private void AddReview_Load(object sender, EventArgs e)
        {
            this.textBox1.Text = GetNewReviewId().ToString();
            using (SqlConnection con = new SqlConnection(ConnectionHelper.ConString))
            {
                using (SqlDataAdapter da = new SqlDataAdapter("SELECT bookId,title FROM Books", con))
                {
                    DataTable dt = new DataTable();
                    da.Fill(dt);
                    this.comboBox1.DataSource = dt.DefaultView;
                }
            }
        }
        private int GetNewReviewId()
        {
            using (SqlConnection con = new SqlConnection(ConnectionHelper.ConString))
            {
                using (SqlCommand cmd = new SqlCommand("SELECT ISNULL(MAX(reviewId), 0) FROM Reviews", con))
                {
                    con.Open();
                    int id = (int)cmd.ExecuteScalar();
                    con.Close();
                    return id + 1;
                }
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            using (SqlConnection con = new SqlConnection(ConnectionHelper.ConString))
            {
                con.Open();
                using (SqlTransaction tran = con.BeginTransaction())
                {

                    using (SqlCommand cmd = new SqlCommand(@"INSERT INTO Reviews 
                                            (reviewId, reviewername, rating, bookId) VALUES
                                            (@i, @n, @r, @bi)", con, tran))
                    {
                        cmd.Parameters.AddWithValue("@i", int.Parse(textBox1.Text));
                        cmd.Parameters.AddWithValue("@n", textBox2.Text);
                        cmd.Parameters.AddWithValue("@r", textBox3.Text);
                        cmd.Parameters.AddWithValue("@bi", (int)comboBox1.SelectedValue);


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
    }
}
