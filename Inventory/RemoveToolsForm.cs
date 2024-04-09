using System;
using System.Data;
using System.Data.SqlClient;
using System.Windows.Forms;

namespace Inventory
{
    public partial class RemoveToolsForm : Form
    {
        //string connectionString = $"Server={Properties.Settings.Default.ServerName}; Database=Inventory; Integrated Security=True;";
        string connectionString = $"Server={Properties.Settings.Default.ServerName}; Database=Inventory; Integrated Security=True;";

        public RemoveToolsForm()
        {
            InitializeComponent();
            LoadPeople();
            InitializeDataGridView();
        }

        private void LoadPeople()
        {
            using (var connection = new SqlConnection(connectionString))
            {
                try
                {
                    connection.Open();
                    var command = new SqlCommand("SELECT PersonID, PersonName FROM Person ORDER BY PersonName", connection);
                    var adapter = new SqlDataAdapter(command);
                    var dataTable = new DataTable();
                    adapter.Fill(dataTable);

                    comboBoxPeople.DisplayMember = "PersonName";
                    comboBoxPeople.ValueMember = "PersonID";
                    comboBoxPeople.DataSource = dataTable;
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Hiba történt: " + ex.Message);
                }
            }
        }
        private void LoadPersonTools(int personId)
        {
            string query = @"
                SELECT Tool.ToolWorkID, ToolName
                FROM Tool
                INNER JOIN Issued ON Tool.ToolWorkID = Issued.ToolWorkID
                WHERE Issued.PersonID = @PersonID";

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@PersonID", personId);

                try
                {
                    conn.Open();
                    SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                    DataTable dataTable = new DataTable();
                    adapter.Fill(dataTable);

                    dataGridViewTools.DataSource = dataTable;

                    dataGridViewTools.Columns["ToolWorkID"].HeaderText = "Eszköz azonosító";
                    dataGridViewTools.Columns["ToolName"].HeaderText = "Eszköz neve";
                    
                    var deleteColumn = dataGridViewTools.Columns["actionColumn"];
                    if (deleteColumn != null)
                    {
                        deleteColumn.DisplayIndex = dataGridViewTools.Columns.Count - 1;
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Hiba történt az adatok betöltése közben: " + ex.Message);
                }
            }
        }
        private void InitializeDataGridView()
        {//felvétele a gyors törlés beállítása miatt
            if (!dataGridViewTools.Columns.Contains("actionColumn"))
            {
                DataGridViewButtonColumn actionButtonColumn = new DataGridViewButtonColumn();
                actionButtonColumn.Name = "actionColumn";
                actionButtonColumn.HeaderText = "";
                actionButtonColumn.Text = "Törlés";
                actionButtonColumn.UseColumnTextForButtonValue = true;
                dataGridViewTools.Columns.Add(actionButtonColumn);
            }

            dataGridViewTools.CellClick += DataGridViewTools_CellClick;
        }

        private void DataGridViewTools_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.ColumnIndex == dataGridViewTools.Columns["actionColumn"].Index && e.RowIndex >= 0)
            {
                var toolWorkId = Convert.ToInt32(dataGridViewTools.Rows[e.RowIndex].Cells["ToolWorkID"].Value);
                RemoveToolAssignment(toolWorkId);
            }
        }

        private void RemoveToolAssignment(int toolWorkId)
        {
            using (var connection = new SqlConnection(connectionString))
            {
                connection.Open();
                var query = "DELETE FROM Issued WHERE ToolWorkID = @ToolWorkID; UPDATE Tool SET ToolIssued = 0 WHERE ToolWorkID = @ToolWorkID";
                var command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@ToolWorkID", toolWorkId);
                command.ExecuteNonQuery();

                MessageBox.Show("Az eszköz hozzárendelése sikeresen eltávolítva.");
                LoadPersonTools((int)comboBoxPeople.SelectedValue);
            }
        }

        private void comboBoxPeople_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (comboBoxPeople.SelectedValue != null)
            {
                var personId = (int)comboBoxPeople.SelectedValue;
                LoadPersonTools(personId);
            }
        }

        private void RemoveToolsForm_Load(object sender, EventArgs e)
        {
            // Ez biztosítja, hogy az első elem automatikusan ki legyen választva és betöltődjön a hozzá kapcsolódó eszköz lista
            if (comboBoxPeople.Items.Count > 0)
            {
                comboBoxPeople.SelectedIndex = 0;
            }
        }

        private void kilépésToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void kiadásToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Close();
            AssignToolForm assignToolForm = new AssignToolForm();
            assignToolForm.ShowDialog();
        }
    }
}