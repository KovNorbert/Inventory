using System.Data.SqlClient;

namespace Inventory
{
    internal class AddPerson
    {
        //Adatbázis hozzáadásához kell Nevet és Poziciót adjuk meg csak mert a ID automatikusan kapják
        public bool addPerson(Person person, string connectionString)
        {
            int affectedRows = 0;
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                string query = "INSERT INTO Person (PersonName, PersonPosition) VALUES (@Name, @Position)";
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@Name", person.PersonName);
                    command.Parameters.AddWithValue("@Position", person.PersonPosition);

                    connection.Open();
                    affectedRows = command.ExecuteNonQuery();
                    connection.Close();
                }
            }
            return affectedRows > 0;
        }
    }
}
