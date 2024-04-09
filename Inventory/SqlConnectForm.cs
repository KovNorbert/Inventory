using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data.SqlClient;

namespace Inventory
{
    public partial class SqlConnectForm : Form
    {
        bool firstFocus = false;
        bool connected = false;
        int i = 0;
        
        public SqlConnectForm()
        {
            InitializeComponent();
        }

        private void connectButton_Click(object sender, EventArgs e)
        {
            // A szerver nevének beolvasása a TextBox-ból
            string serverName = connectTB.Text;

            // A kapcsolat ellenőrzésére használt connection string összeállítása
            string connectionString = $"Server={serverName};Database=master;Integrated Security=True";

            try
            {
                // Adatbázi szerverhez való csatlakozási próba
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open(); // Csatlakozási kísérlet
                    // Ha a csatlakozás sikeres, frissítjük a label szövegét
                    labelConnectionResult.Text = "A csatlakozás sikeres!";
                    labelConnectionResult.ForeColor = System.Drawing.Color.Green;

                    // Itt mentjük a szerver nevét a Settings-be ez bárhonnan kiolvasható
                    Properties.Settings.Default.ServerName = serverName;
                    Properties.Settings.Default.Save();
                    timer1.Enabled = true;
                    connected = true;
                    
                }
            }
            catch (SqlException)
            {
                // Ha nem sikerül a csatlakozás, informáljuk a felhasználót
                labelConnectionResult.Text = "A csatlakozás sikertelen. Ellenőrizze a szerver nevét.";
                labelConnectionResult.ForeColor = System.Drawing.Color.Red;
            }
        }
        // Timer felvétele, hogy ne egyből nyissuk meg a többi hozzáférést hanem tudjuk tájékoztatni a felhasználót a csatlakozási sikerről
        private void connectTB_MouseClick(object sender, MouseEventArgs e)
        {

            if (!firstFocus)
            {
                connectTB.Text = "";
                firstFocus = true;
            }
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            if (connected && i==0) 
            {
                i++;
                MainForm mainForm = new MainForm();
                this.Hide();
                mainForm.ShowDialog();
            }
        }
    }
}
