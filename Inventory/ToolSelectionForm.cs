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

namespace Inventory
{
    public partial class ToolSelectionForm : Form
    {
        public int SelectedToolWorkId { get; private set; } = -1;
        string connectionString = $"Server={Properties.Settings.Default.ServerName}; Database=Inventory; Integrated Security=True;";
        public ToolSelectionForm()
        {
            InitializeComponent();
            LoadToolsWithoutLocation();
        }
        private void LoadToolsWithoutLocation()
        {
            // Adatbázisból lekérdezzük azokat a Tool-okat, amelyekhez még nincs ToolLocation hozzárendelve
            string query = @"SELECT ToolWorkID FROM Tool WHERE ToolWorkID NOT IN (SELECT ToolWorkID FROM ToolLocation)";

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            listBox1.Items.Add(reader["ToolWorkID"].ToString());
                        }
                    }
                }
            }
        }

        private void saveB_Click(object sender, EventArgs e)
        {
            if (listBox1.SelectedItem != null)
            {
                SelectedToolWorkId = Convert.ToInt32(listBox1.SelectedItem.ToString());
                this.DialogResult = DialogResult.OK;
                this.Close();
            }
            else
            {
                MessageBox.Show("Kérlek, válassz egy eszközt a listából.");
            }
        }
    }
}