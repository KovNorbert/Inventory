using System;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Windows.Forms;


namespace Inventory
{
    public partial class SearchForm : Form
    {                
        private string connectionString = @"Server=KONO-PC; Database=Inventory; Integrated Security=True;";
        private int currentColumnIndex = -1; // Globális változó az oszlop indexének tárolására
        
        private int selectedId = -1;
        private int tableIndex = -1;

        private TextBox searchTextBox = new TextBox();
        public SearchForm()
        {
            InitializeComponent();
            InitializeComboBox();
        }
        private void InitializeComboBox()
        {
            comboBoxTables.Items.Add("Személyek");
            comboBoxTables.Items.Add("Eszközök");
            comboBoxTables.SelectedIndex = 0;
            searchTextBox.Visible = false;// ezek alapján bármikor bővíthető további táblákkal
        }
        private void TypingTextChange(object sender, EventArgs e)
        {
            string selectedTable = "";
            string searchValue = textBoxSearch.Text.Trim();
            if (comboBoxTables.SelectedItem == null) return;

            switch (comboBoxTables.SelectedIndex)
            {
                case 0:
                    selectedTable = "Person";
                    SearchDatabase(selectedTable, searchValue);
                    break;
                case 1:
                    selectedTable = "Tool";
                    SearchDatabase(selectedTable, searchValue);
                    break;
            }            
        }
        private void SearchDatabase(string tableName, string searchValue)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                try
                {
                    connection.Open();
                    SqlCommand command;
                    string sqlQuery="";

                    switch (comboBoxTables.SelectedIndex)
                    {
                        case 0:
                            sqlQuery = @"SELECT * FROM Person WHERE 
                            PersonName LIKE @SearchValue OR 
                            PersonPosition LIKE @SearchValue";
                            break;
                        case 1:
                            sqlQuery = @"SELECT * FROM Tool WHERE 
                            ToolName LIKE @SearchValue"; 
                            break;
                    }
                    command = new SqlCommand(sqlQuery, connection);
                    command.Parameters.AddWithValue("@SearchValue", $"%{searchValue}%");

                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        DataTable dataTable = new DataTable();
                        dataTable.Load(reader);
                        DisplayResults(dataTable, comboBoxTables.SelectedItem.ToString());
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Hiba történt: " + ex.Message);                    
                }
            }
        }
        private void DisplayResults(DataTable dataTable, string tableName)
        {
            dataGridViewResults.DataSource = dataTable;

            // Ellenőrizzük, hogy melyik tábla adatait töltöttük be
            if (tableName == "Person")
            {
                // Itt állítjuk be a Person tábla oszlopfejléceit
                dataGridViewResults.Columns["PersonID"].HeaderText = "Azonosító";
                dataGridViewResults.Columns["PersonName"].HeaderText = "Név";
                dataGridViewResults.Columns["PersonPosition"].HeaderText = "Pozíció";
            }
            else if (tableName == "Tool")
            {
                // Itt állítjuk be a Tool tábla oszlopfejléceit
                dataGridViewResults.Columns["ToolWorkID"].HeaderText = "Tárgyazonosító";
                dataGridViewResults.Columns["ToolSerialNumber"].HeaderText = "Sorozatszám";
                dataGridViewResults.Columns["ToolName"].HeaderText = "Eszköz Név";
                dataGridViewResults.Columns["ToolDescription"].HeaderText = "Leírás";
                dataGridViewResults.Columns["ToolCategory"].HeaderText = "Kategória";
                dataGridViewResults.Columns["ToolIssued"].HeaderText = "Kiadva";
            }
        }
        private void Form3_Load(object sender, EventArgs e)
        {
            string selectedTable = comboBoxTables.SelectedItem.ToString();
            string searchValue = textBoxSearch.Text.Trim();

            SearchDatabase(selectedTable, searchValue);
        }
        private void comboBoxTables_SelectedIndexChanged_1(object sender, EventArgs e)
        {
            string selectedTable = comboBoxTables.SelectedItem.ToString();
            string searchValue = textBoxSearch.Text.Trim();

            SearchDatabase(selectedTable, searchValue);
            textBoxSearch.Text = "";
        }

        private void dataGridViewResults_ColumnHeaderMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            if (e.Button != MouseButtons.Left)
            {

                if (searchTextBox.Visible == false)
                {
                    currentColumnIndex = e.ColumnIndex;
                    // A TextBox pozíciója és mérete alapján az oszlopfejléc
                    Rectangle rect = dataGridViewResults.GetCellDisplayRectangle(e.ColumnIndex, -1, true);
                    searchTextBox.Location = new Point(rect.X, rect.Y + dataGridViewResults.ColumnHeadersHeight);
                    searchTextBox.Size = new Size(rect.Width, rect.Height);

                    // Ha még nem adtuk hozzá a DataGridView-hoz a TextBox-ot
                    if (!dataGridViewResults.Controls.Contains(searchTextBox))
                    {
                        dataGridViewResults.Controls.Add(searchTextBox);
                        // Itt adjuk hozzá az eseménykezelőt, hogy elkerüljük a többszörös hozzáadást
                        searchTextBox.TextChanged += searchTextBox_TextChanged;
                    }

                    searchTextBox.Visible = true;
                    searchTextBox.Focus();
                }
                else if (searchTextBox.Visible == true)
                {
                    searchTextBox.Visible = false;
                }
            }
        }
        private void searchTextBox_TextChanged(object sender, EventArgs e)
        {

            if (currentColumnIndex == -1) return; // Ha nincs kiválasztott oszlop, nem csinálunk semmit

            DataTable dataTable = dataGridViewResults.DataSource as DataTable;
            if (dataTable != null)
            {
                string columnName = dataGridViewResults.Columns[currentColumnIndex].Name;
                string filterExpression = $"{columnName} LIKE '%{searchTextBox.Text}%'";

                // Szűrés alkalmazása
                dataTable.DefaultView.RowFilter = filterExpression;
            }
        
        }
        private void SearchTextBox_Leave(object sender, EventArgs e)
        {
            searchTextBox.Visible = false; // Eltüntetjük a textBoxot, amikor elveszíti a fókuszt
        }

        
        private void dataGridViewResults_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                int selectedId = Convert.ToInt32(dataGridViewResults.Rows[e.RowIndex].Cells["ID"].Value);
                int tableIndex = comboBoxTables.SelectedIndex;

                NewDataForm newDataForm = new NewDataForm();
                newDataForm.LoadData(selectedId, tableIndex);
                newDataForm.ShowDialog();
            }
        }

        private void dataGridViewResults_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0 && comboBoxTables.SelectedIndex == 0)
            {
                selectedId = Convert.ToInt32(dataGridViewResults.Rows[e.RowIndex].Cells["PersonID"].Value);
                tableIndex = comboBoxTables.SelectedIndex;
            }
            else if (e.RowIndex >= 0 && comboBoxTables.SelectedIndex == 1)
            {
                selectedId = Convert.ToInt32(dataGridViewResults.Rows[e.RowIndex].Cells["ToolWorkID"].Value);
                tableIndex = comboBoxTables.SelectedIndex;
            }
        }

        private void newDataButton_Click(object sender, EventArgs e)
        {
          
        }

        private void modifyButton_Click(object sender, EventArgs e)
        {
            
        }

        private void button1_Click(object sender, EventArgs e)
        {
            RemoveToolsForm assignToolsForm = new RemoveToolsForm();
            assignToolsForm.Show();
        }

        private void törlésToolStripMenuItem_Click(object sender, EventArgs e)
        {
            NewDataForm newData = new NewDataForm();

            if (selectedId != -1 && tableIndex != -1 && comboBoxTables.SelectedIndex == 0)
            {
                NewDataForm newDataForm = new NewDataForm();
                newDataForm.LoadDataForPerson(selectedId); // Átadjuk az adatokat
                newDataForm.addWhat = comboBoxTables.SelectedIndex;
                newDataForm.isMody = true;
                newDataForm.Show();
            }
            else if (selectedId != -1 && tableIndex != -1 && comboBoxTables.SelectedIndex == 1)
            {
                NewDataForm newDataForm = new NewDataForm();
                newDataForm.LoadDataForTool(selectedId); // Átadjuk az adatokat
                newDataForm.addWhat = comboBoxTables.SelectedIndex;
                newDataForm.isMody = true;
                newDataForm.Show();
            }
            else
            {
                MessageBox.Show("Kérjük, válasszon ki egy rekordot a módosításhoz!");
            }
        }

        private void újToolStripMenuItem_Click(object sender, EventArgs e)
        {
            NewDataForm newDataForm = new NewDataForm();
            newDataForm.addWhat = comboBoxTables.SelectedIndex;
            newDataForm.Show();
        }

        private void kilépésToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}