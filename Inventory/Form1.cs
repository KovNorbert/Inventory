using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Net.Mime.MediaTypeNames;

namespace Inventory
{
    public partial class Form1 : Form
    {
        private bool mouseDown;
        private bool mouseMove;
        private Point lastLocation;
        private Point circleCenter; // A kör középpontjának koordinátái
        private List<Point> circleCenters = new List<Point>(); // A korábban létrehozott körök középpontjainak listája
        private ContextMenuStrip contextMenuStrip1; // A kis menü ablak
        public Form1()
        {
            InitializeComponent();
            pictureBox1.BackColor = Color.White; // Háttérszín beállítása
            LoadCoordinates(); // Koordináták betöltése

            // ContextMenuStrip létrehozása
            contextMenuStrip1 = new ContextMenuStrip();
            contextMenuStrip1.Items.Add("Hozzáadás", null, AddCircle_Click);
            pictureBox1.ContextMenuStrip = contextMenuStrip1;


        }
        private void Form1_Load(object sender, EventArgs e)
        {

            string imagesDirectory = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "img");

            // A kép neve
            string imageName = "test1.jpg"; // A kép nevét ide írd

            // Teljes elérési útvonal a képhez
            string imagePath = Path.Combine(imagesDirectory, imageName);

            // Ellenőrizzük, hogy a kép létezik-e
            if (File.Exists(imagePath))
            {
                // Betöltjük a képet a PictureBox-ba
                pictureBox1.Image = new System.Drawing.Bitmap(imagePath);
            }
            else
            {
                MessageBox.Show("A kép nem található az adott elérési úton!");
            }

        }

        private void pictureBox1_MouseDown(object sender, MouseEventArgs e)
        {
            mouseDown = true;
            mouseMove = false;
            lastLocation = e.Location;
        }

        private void pictureBox1_MouseMove(object sender, MouseEventArgs e)
        {

            if (mouseDown)
            {
                mouseMove = true;
                int newX = pictureBox1.Left + (e.X - lastLocation.X);
                int newY = pictureBox1.Top + (e.Y - lastLocation.Y);

                pictureBox1.Left = newX;
                pictureBox1.Top = newY;
            }
        }

        private void pictureBox1_MouseUp(object sender, MouseEventArgs e)
        {
            mouseDown = false;

            if (mouseMove == false)
            {
                DialogResult dialogResult = MessageBox.Show("Fel akarja-e venni a helyszínt", "Felvétel", MessageBoxButtons.YesNo);
                if (dialogResult == DialogResult.Yes)
                {
                    circleCenter = e.Location; // Kattintás helye a kör középpontja
                    circleCenters.Add(circleCenter); // Kör középpontjának hozzáadása a listához
                    pictureBox1.Invalidate(); // PictureBox újrarajzolása
                    SaveCoordinates(); // Koordináták mentése
                }
                else if (dialogResult == DialogResult.No)
                {

                }
            }
            mouseMove = false;

        }

        private void pictureBox1_Paint(object sender, PaintEventArgs e)
        {
            // Korábban létrehozott körök rajzolása
            int radius = 10; // Kör sugara
            foreach (Point center in circleCenters)
            {
                e.Graphics.FillEllipse(Brushes.Red, center.X - radius, center.Y - radius, 2 * radius, 2 * radius);
            }
        }

        private void SaveCoordinates()
        {
            // Koordináták mentése .csv fájlba
            string filePath = "coordinates.csv";
            List<string> lines = new List<string>();
            foreach (Point center in circleCenters)
            {
                lines.Add(center.X + "," + center.Y);
            }
            File.WriteAllLines(filePath, lines);
        }

        private void LoadCoordinates()
        {
            // Korábban mentett koordináták betöltése
            string filePath = "coordinates.csv";
            if (File.Exists(filePath))
            {
                List<string> lines = File.ReadAllLines(filePath).ToList();
                foreach (string line in lines)
                {
                    string[] parts = line.Split(',');
                    int x = int.Parse(parts[0]);
                    int y = int.Parse(parts[1]);
                    circleCenters.Add(new Point(x, y));
                }
            }
        }

        private void AddCircle_Click(object sender, EventArgs e)
        {
            // Kör hozzáadása
            circleCenters.Add(circleCenter);
            pictureBox1.Invalidate();
            SaveCoordinates();
        }
    }
}