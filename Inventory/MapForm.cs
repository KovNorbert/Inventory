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

namespace Inventory
{
    public partial class MapForm : Form
    {
        public MapForm()
        {
            InitializeComponent();
        }

        private void MapUpload_Click(object sender, EventArgs e)
        {
            string mapName = MapUploadTB.Text.Trim(); // A kép neve a TextBox-ból

            // Ellenőrizzük, hogy a név megfelelő-e
            if (string.IsNullOrEmpty(mapName) || mapName.Length < 3)
            {
                MessageBox.Show("A térkép neve nem lehet üres, és legalább három karakter hosszúnak kell lennie.");
                return; // Kilépünk a metódusból, ha a név nem megfelelő
            }

            // Fájlválasztó dialógus megnyitása
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Image Files|*.jpg;*.jpeg;*.png;";
            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                string imagePath = openFileDialog.FileName;

                // Kép bináris formátumba konvertálása
                byte[] mapData;
                using (FileStream fs = new FileStream(imagePath, FileMode.Open, FileAccess.Read))
                {
                    using (BinaryReader br = new BinaryReader(fs))
                    {
                        mapData = br.ReadBytes((int)fs.Length);
                    }
                }

                // SQL kapcsolat string. Itt a Windows hitelesítést használjuk
                string connectionString = $"Server={Properties.Settings.Default.ServerName}; Database=Inventory; Integrated Security=True;";

                // Kép feltöltése az adatbázisba
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    string sql = "INSERT INTO Map (MapName, MapPicture) VALUES (@MapName, @MapPicture)";
                    using (SqlCommand cmd = new SqlCommand(sql, conn))
                    {
                        cmd.Parameters.AddWithValue("@MapName", mapName);
                        cmd.Parameters.AddWithValue("@MapPicture", mapData);

                        conn.Open();
                        cmd.ExecuteNonQuery();
                        MessageBox.Show("A kép sikeresen feltöltve az adatbázisba.");
                    }
                }
            }
            this.Close();
        }
    }
}
