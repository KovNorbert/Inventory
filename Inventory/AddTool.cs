using System;
using System.Data.SqlClient;

namespace Inventory
{
    internal class AddTool
    {
        //Adatbázis hozzáadáshoz kell egyszerű adatok beolvasása és elmentése az adatbázisba
        public bool addTool(Tool tool, string connectionString)
        {
            int affectedRows = 0;
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                string query = @"INSERT INTO Tool (ToolSerialNumber, ToolName, ToolDescription, ToolCategory, ToolIssued) 
                         VALUES (@SerialNumber, @Name, @Description, @Category, @Issued)";
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@SerialNumber", tool.ToolSerialNumber ?? (object)DBNull.Value);
                    command.Parameters.AddWithValue("@Name", tool.ToolName);
                    command.Parameters.AddWithValue("@Description", tool.ToolDescription ?? (object)DBNull.Value);
                    command.Parameters.AddWithValue("@Category", tool.ToolCategory ?? (object)DBNull.Value);
                    command.Parameters.AddWithValue("@Issued", tool.ToolIssued);

                    connection.Open();
                    affectedRows = command.ExecuteNonQuery();
                    connection.Close();
                }
            }
            return affectedRows > 0;
        }
    }
}
