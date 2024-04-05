using System;
using System.Data.SqlClient;
using System.IO;
using System.Drawing;

namespace Inventory
{
    // kép betöltéséhez kell, itt "kódoljuk ki" a sql-be feltöltött képeket byte formátumból 
    internal class ImgLoader
    {
        private string connectionString;
        public ImgLoader(string connectionString)
        {
            this.connectionString = connectionString;
        }
        public Image LoadImage(string mapName)
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                using (SqlCommand cmd = new SqlCommand("SELECT MapPicture FROM Map WHERE MapName = @MapName", conn))
                {
                    cmd.Parameters.AddWithValue("@MapName", mapName);
                    byte[] imageData = (byte[])cmd.ExecuteScalar();

                    if (imageData != null)
                    {
                        using (MemoryStream ms = new MemoryStream(imageData))
                        {
                            return Image.FromStream(ms);
                        }
                    }
                    else
                    {
                        return null;
                    }
                }
            }
        }
    } 
}