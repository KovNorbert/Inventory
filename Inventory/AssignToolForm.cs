using System;
using System.Data;
using System.Data.SqlClient;
using System.Windows.Forms;

namespace Inventory
{
    public partial class AssignToolForm : Form
    {
        private string connectionString = @"Server=KONO-PC; Database=Inventory; Integrated Security=True;";

        public AssignToolForm()
        {
            InitializeComponent();
            LoadPeople();
            LoadAvailableTools();
            comboBoxPeople.SelectedIndex = 0;
        }
        //Kód segédlet a ComboBoxPeople betöltéséhez
        private void LoadPeople()
        {
            using (var connection = new SqlConnection(connectionString))
            {
                try
                {
                    connection.Open();
                    var command = new SqlCommand("SELECT PersonID, PersonName FROM Person", connection);
                    var reader = command.ExecuteReader();
                    while (reader.Read())
                    {
                        comboBoxPeople.Items.Add(new { Text = reader["PersonName"].ToString(), Value = reader["PersonID"] });
                    }
                    comboBoxPeople.DisplayMember = "Text";
                    comboBoxPeople.ValueMember = "Value";
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Hiba történt: " + ex.Message);
                }
            }
        }

        //Kód segédlet a datagridView betöltéséhez
        private void LoadAvailableTools()
        {
            string query = @"
                SELECT ToolWorkID, ToolName, ToolSerialNumber, ToolDescription, ToolCategory
                FROM Tool
                WHERE ToolIssued = 0";

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                SqlCommand cmd = new SqlCommand(query, conn);

                try
                {
                    conn.Open();
                    SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                    DataTable dataTable = new DataTable();
                    adapter.Fill(dataTable);

                    dataGridViewTools.DataSource = dataTable;
                    // Opcionálisan beállíthatod a megjelenítendő oszlopok fejléceit is itt
                    dataGridViewTools.Columns["ToolWorkID"].HeaderText = "Eszközazonosító";
                    dataGridViewTools.Columns["ToolName"].HeaderText = "Név";
                    dataGridViewTools.Columns["ToolSerialNumber"].HeaderText = "Sorozatszám";
                    dataGridViewTools.Columns["ToolDescription"].HeaderText = "Leírás";
                    dataGridViewTools.Columns["ToolCategory"].HeaderText = "Kategória";
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Hiba történt az adatok betöltése közben: " + ex.Message);
                }
            }
        }
        //kód az eszköz hozzárendeléshez a kiválasztott személyhez 
        private void AssignToolToPerson(string toolId, string personId)
        {
            using (var connection = new SqlConnection(connectionString))
            {
                try
                {
                    connection.Open();
                    var command = new SqlCommand("INSERT INTO Issued (PersonID, ToolWorkID) VALUES (@PersonID, @ToolWorkID); UPDATE Tool SET ToolIssued = 1 WHERE ToolWorkID = @ToolWorkID", connection);
                    command.Parameters.AddWithValue("@PersonID", personId);
                    command.Parameters.AddWithValue("@ToolWorkID", toolId);
                    command.ExecuteNonQuery();

                    MessageBox.Show("Az eszköz hozzárendelése sikeres!");
                    LoadAvailableTools(); // Frissítjük az elérhető eszközök listáját
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Hiba történt: " + ex.Message);
                }
            }
        }
        private void addButton_click(object sender, EventArgs e)
        {
            // Eszköz hozzáadása a kiválasztott személyhez gomb segítségével 
            if (dataGridViewTools.SelectedRows.Count == 0)
            {
                MessageBox.Show("Válasszon ki egy eszközt a listából.");
                return;
            }

            if (comboBoxPeople.SelectedItem == null)
            {
                MessageBox.Show("Válasszon ki egy személyt.");
                return;
            }

            var selectedToolId = dataGridViewTools.SelectedRows[0].Cells["ToolWorkID"].Value.ToString();
            var selectedPerson = (comboBoxPeople.SelectedItem as dynamic).Value.ToString();

            AssignToolToPerson(selectedToolId, selectedPerson);

            // Frissítések után újratöltjük az elérhető eszközök listáját,
            // hogy az már hozzárendelt eszközök ne jelenjenek meg
            LoadAvailableTools();
        }

        private void kilépésToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void visszavételToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Close();
            RemoveToolsForm removeToolsForm = new RemoveToolsForm();
            removeToolsForm.ShowDialog();
        }
    }
}