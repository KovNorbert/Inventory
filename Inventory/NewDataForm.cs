using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics.Eventing.Reader;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Inventory
{
    public partial class NewDataForm : Form
    {
        SearchForm searchForm = new SearchForm();
        AddPerson addPerson = new AddPerson();
        AddTool addTool = new AddTool();
        public int addWhat = -1;
        private string connectionString = @"Server=KONO-PC; Database=Inventory; Integrated Security=True;";
        private int selectedId = -1;
        public bool isMody = false;
        public NewDataForm()
        {
            InitializeComponent();
            UpdateButtonState();
        }
        private void addButton_Click(object sender, EventArgs e)
        {
            switch (addWhat)
            {
                case 0:
                    if (addButton.Text != "Törlés")
                    {
                        string personName = firstTB.Text;
                        string personPosi = secondTB.Text;
                        Person newPerson = new Person(personName, personPosi);

                        bool isPesonAdded = addPerson.addPerson(newPerson, connectionString);
                        if (isPesonAdded)
                            MessageBox.Show("Személy felvétele megtörtént");
                        else
                            MessageBox.Show("Személy felvétele sikertelen");
                    }
                    else                    
                        DeletePersonData(selectedId);
                    
                    break;
                case 1:
                    if (addButton.Text != "Törlés")
                    {
                        string toolName = secondTB.Text;
                        string toolSerial = firstTB.Text;
                        string toolCategory = fourthTB.Text;
                        string toolDisc = thirdTB.Text;
                        Tool newTool = new Tool(toolName, toolSerial, toolCategory, toolDisc);

                        bool isToolAdded = addTool.addTool(newTool, connectionString);
                        if (isToolAdded)
                            MessageBox.Show("Eszköz felvétele megtörtént");
                        else
                            MessageBox.Show("Eszköz felvétele sikertelen");
                    }
                    else
                        DeleteToolData(selectedId);
                    break;
            }            
        }
        private void UpdateButtonState()
        {
            if (addWhat ==0)            
                addButton.Enabled = firstTB.Text.Length >= 2 && secondTB.Text.Length >= 2;
            else if (addWhat ==1)
                addButton.Enabled = firstTB.Text.Length >= 2 && thirdTB.Text.Length >= 2 && fourthTB.Text.Length >= 2;
        }
        private void newData_Load(object sender, EventArgs e)
        {
            if (addWhat == 0 && isMody == false)
            {
                Size = new Size(250, 200);

            }
            else if (addWhat == 1 && !isMody)
            {
                addL0.Text = "Eszköz felvétele";
                addL2.Text = "Szériaszám";
                addL3.Visible = true;
                addL4.Visible = true;
                Size = new Size(250, 275);
                thirdTB.Visible = true;
                fourthTB.Visible = true;
                addButton.Location = new Point(135, 200);
                saveModificationButton.Location = new Point(15, 200);
            }
            else if (addWhat == 0 && isMody)
            {
                Size = new Size(250, 200);
                saveModificationButton.Visible = true;
                addButton.Text = "Törlés";

            }
            else if (addWhat == 1 && isMody)
            {
                addL0.Text = "Eszköz felvétele";
                addL2.Text = "Szériaszám";
                addL3.Visible = true;
                addL4.Visible = true;
                Size = new Size(250, 275);
                thirdTB.Visible = true;
                fourthTB.Visible = true;
                addButton.Location = new Point(135, 200);
                saveModificationButton.Location = new Point(15, 200);
                saveModificationButton.Visible = true;
                addButton.Text = "Törlés";
            }

        }
        private void personNameTB_TextChanged(object sender, EventArgs e)
        {
            if (isMody)
                UpdateButtonState();
            else
                addButton.Enabled = true;
        }
        private void personPosiTB_TextChanged(object sender, EventArgs e)
        {
            if (isMody)
                UpdateButtonState();
            else
                addButton.Enabled = true;
        }
        private void thirdTB_TextChanged(object sender, EventArgs e)
        {
            if (isMody)
                UpdateButtonState();
            else
                addButton.Enabled = true;
        }
        private void fourthTB_TextChanged(object sender, EventArgs e)
        {
            if (isMody)
                UpdateButtonState();
            else
                addButton.Enabled = true;
        }
        public void LoadData(int id, int tableIndex)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                SqlCommand command;

                if (tableIndex == 0) // Személyek
                {
                    command = new SqlCommand("SELECT * FROM Person WHERE PersonID = @ID", connection);
                    command.Parameters.AddWithValue("@ID", id);
                }
                else // Eszközök
                {
                    command = new SqlCommand("SELECT * FROM Tool WHERE ToolWorkID = @ID", connection);
                    command.Parameters.AddWithValue("@ID", id);
                }

                using (SqlDataReader reader = command.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        firstTB.Text = reader.GetString(1);
                    }
                }
            }
        }
        public void LoadDataForPerson(int id)
        {
            selectedId = id; // Beállítjuk a kiválasztott személy azonosítóját

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                SqlCommand command = new SqlCommand("SELECT PersonName, PersonPosition FROM Person WHERE PersonID = @ID", connection);
                command.Parameters.AddWithValue("@ID", id);

                using (SqlDataReader reader = command.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        firstTB.Text = reader.GetString(0); // PersonName beállítása
                        secondTB.Text = reader.GetString(1); // PersonPosition beállítása
                    }
                }
            }
        }
        public void LoadDataForTool(int toolId)
        {
            selectedId = toolId; // Beállítjuk a kiválasztott eszköz azonosítóját

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                SqlCommand command = new SqlCommand("SELECT ToolName, ToolSerialNumber, ToolDescription, ToolCategory FROM Tool WHERE ToolWorkID = @ID", connection);
                command.Parameters.AddWithValue("@ID", toolId);

                using (SqlDataReader reader = command.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        firstTB.Text = reader.GetString(0);
                        secondTB.Text = reader.GetString(1);
                        thirdTB.Text = reader.GetString(3);
                        fourthTB.Text = reader.GetString(2);
                    }
                }
            }
        }
        public void UpdatePersonData(int personId)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                string query = "UPDATE Person SET PersonName = @Name, PersonPosition = @Position WHERE PersonID = @ID";
                SqlCommand command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@ID", personId);
                command.Parameters.AddWithValue("@Name", firstTB.Text);
                command.Parameters.AddWithValue("@Position", secondTB.Text);

                int affectedRows = command.ExecuteNonQuery();
                if (affectedRows > 0)
                {
                    MessageBox.Show("A személy adatainak frissítése sikeres.");
                }
                else
                {
                    MessageBox.Show("Nem sikerült frissíteni a személy adatait.");
                }
            }
        }
        public void UpdateToolData(int toolId)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                string query = "UPDATE Tool SET ToolName = @ToolName, ToolSerialNumber = @ToolSerialNumber, ToolDescription = @ToolDescription, ToolCategory = @ToolCategory WHERE ToolWorkID = @ToolWorkID";
                SqlCommand command = new SqlCommand(query, connection);

                // A paraméterek helyes beállítása
                command.Parameters.AddWithValue("@ToolWorkID", toolId); // A ToolWorkID paraméter helyes beállítása
                command.Parameters.AddWithValue("@ToolName", firstTB.Text); // A ToolName paraméter beállítása
                command.Parameters.AddWithValue("@ToolSerialNumber", secondTB.Text); // A ToolSerialNumber paraméter beállítása
                command.Parameters.AddWithValue("@ToolDescription", thirdTB.Text); // A ToolDescription paraméter beállítása
                command.Parameters.AddWithValue("@ToolCategory", fourthTB.Text); // A ToolCategory paraméter beállítása

                int affectedRows = command.ExecuteNonQuery();
                if (affectedRows > 0)
                {
                    MessageBox.Show("Az eszköz adatainak frissítése sikeres.");
                }
                else
                {
                    MessageBox.Show("Nem sikerült frissíteni az eszköz adatait.");
                }
            }
        }

        public void DeletePersonData(int personId)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                string query = "DELETE FROM Person WHERE PersonID = @ID";
                SqlCommand command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@ID", personId);

                connection.Open();
                int result = command.ExecuteNonQuery();
                if (result > 0)
                    MessageBox.Show("Személy sikeresen törölve.");
                else
                    MessageBox.Show("A törlés sikertelen.");
            }
        }

        public void DeleteToolData(int toolId)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                string query = "DELETE FROM Tool WHERE ToolWorkID = @ID";
                SqlCommand command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@ID", toolId);

                connection.Open();
                int result = command.ExecuteNonQuery();
                if (result > 0)
                    MessageBox.Show("Eszköz sikeresen törölve.");
                else
                    MessageBox.Show("A törlés sikertelen.");
            }
        }

        private void saveModificationButton_Click(object sender, EventArgs e)
        {
            if (addWhat == 0) // Személy
            {
                UpdatePersonData(selectedId);
            }
            else if (addWhat == 1) // Eszköz
            {
                UpdateToolData(selectedId);
            }
        }
    }
}
