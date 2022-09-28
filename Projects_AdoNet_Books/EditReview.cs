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
    public partial class EditReview : Form
    {
        string action = "Edit";
        public EditReview()
        {
            InitializeComponent();
        }
        public Form1 OpenerForm { get; set; }
        private void EditReview_Load(object sender, EventArgs e)
        {
            LoadCombo();
        }
        private void LoadCombo()
        {
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

        private void comboBox2_SelectedIndexChanged(object sender, EventArgs e)
        {
            using (SqlConnection con = new SqlConnection(ConnectionHelper.ConString))
            {
                using (SqlCommand cmd = new SqlCommand($"SELECT * FROM Reviews  WHERE bookId=@t", con))
                {
                    cmd.Parameters.AddWithValue("@t", (int)comboBox2.SelectedValue);
                    con.Open();
                    SqlDataReader dr = cmd.ExecuteReader();
                    if (dr.Read())
                    {
                        textBox2.Text = dr.GetString(1);
                        textBox3.Text = dr.GetString(2);
                    }
                }
            }
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            using (SqlConnection con = new SqlConnection(ConnectionHelper.ConString))
            {
                using (SqlDataAdapter da = new SqlDataAdapter($"SELECT reviewId ,reviewername  FROM Reviews  WHERE bookId =${(int)comboBox1.SelectedValue}", con))
                {
                    DataTable dt = new DataTable();
                    da.Fill(dt);
                    this.comboBox2.DataSource = dt.DefaultView;
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

                    using (SqlCommand cmd = new SqlCommand(@"UPDATE Reviews  
                                            SET reviewername =@d, rating =@s, bookId =@t 
                                            WHERE reviewId =@i", con, tran))
                    {
                        cmd.Parameters.AddWithValue("@i", int.Parse(comboBox2.SelectedValue.ToString()));
                        cmd.Parameters.AddWithValue("@d", textBox2.Text);
                        cmd.Parameters.AddWithValue("@s", textBox3.Text);
                        cmd.Parameters.AddWithValue("@t", (int)comboBox1.SelectedValue);


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
            this.action = "Delete";
            using (SqlConnection con = new SqlConnection(ConnectionHelper.ConString))
            {
                con.Open();
                using (SqlTransaction tran = con.BeginTransaction())
                {

                    using (SqlCommand cmd = new SqlCommand(@"DELETE  Reviews  
                                            WHERE reviewId=@i", con, tran))
                    {
                        cmd.Parameters.AddWithValue("@i", int.Parse(comboBox2.Text));



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
