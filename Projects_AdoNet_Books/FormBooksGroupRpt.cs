using Projects_AdoNet_Books.Reports;
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
    public partial class FormBooksGroupRpt : Form
    {
        public FormBooksGroupRpt()
        {
            InitializeComponent();
        }

        private void FormBooksGroupRpt_Load(object sender, EventArgs e)
        {
            DataSet ds = new DataSet();
            using (SqlConnection con = new SqlConnection(ConnectionHelper.ConString))
            {
                using (SqlDataAdapter da = new SqlDataAdapter("SELECT * FROM Books", con))
                {
                    da.Fill(ds, "Booksi");
                    ds.Tables["Booksi"].Columns.Add(new DataColumn("image", typeof(System.Byte[])));
                    for (var i = 0; i < ds.Tables["Booksi"].Rows.Count; i++)
                    {
                        ds.Tables["Booksi"].Rows[i]["image"] = File.ReadAllBytes(Path.Combine(Path.GetFullPath(@"..\..\Pictures"), ds.Tables["Booksi"].Rows[i]["picture"].ToString()));
                    }
                    da.SelectCommand.CommandText = "SELECT * FROM Reviews";
                    da.Fill(ds, "Reviews");
                    BooksGroupRpt rpt = new BooksGroupRpt();
                    rpt.SetDataSource(ds);
                    crystalReportViewer1.ReportSource = rpt;
                    rpt.Refresh();
                    crystalReportViewer1.Refresh();
                }
            }
        }
    }
}
