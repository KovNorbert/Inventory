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
    public partial class MainForm : Form
    {
        private bool mouseDown; //Azért kell hogy le tudjuk menteni és vizsgálni, hogy a pictureBoxon belül nyomtuk le az egér gombot
        private bool mouseMove; //Azért kell hogy le tudjuk menteni és vizsgálni, hogy a pictureBoxon belül mozgattuk az egeret
        private Point lastLocation; // Azért kell hogy le tudjuk menteni és vizsgálni, hogy hol nyomtuk le az egeret a pictureboxon belül
        private Point circleCenter; // A kör középpontjának koordinátái, amit későbbiekben összevetünk a lactLocation-el(elmentett koordináttákal)
        private List<Point> circleCenters = new List<Point>(); // A korábban létrehozott körök középpontjainak listája
        private ContextMenuStrip contextMenuStrip1; // A kis menü ablak(még kérdéses a beépítés)
        private int radius = 10; // Kör sugara (változtatható láthatóság miatt, valószínüleg a végén dinamikus lesz a zoom in/out miatt)
        private Point? movingCircleCenter = null; // A mozgatott kör középpontja (null, ha éppen nem mozgatunk kört)
        public MainForm()
        {
            InitializeComponent();          
            LoadCoords(); // Koordináták betöltése

            // ContextMenuStrip létrehozása abban az esetben ha a törlés meg létrehozást külön jobb clickre akarnám tenni.
            contextMenuStrip1 = new ContextMenuStrip();
            contextMenuStrip1.Items.Add("Hozzáadás", null, AddCircle_Click);
            mainPictureBox.ContextMenuStrip = contextMenuStrip1;
        }
        private void MainForm_Load(object sender, EventArgs e) // későbbiekben egy halom mssql kódsor lesz itt 
        {
            string imagesDirectory = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "img"); // hol helyezkedik el a kép most jelenleg azért ilyen egyszerű mert alapértelmezett helyen van a mappa
            
            // A kép neve
            string imageName = "test1.jpg"; // A kép nevét itt kérem be, ezt majd későbbiekben kiteszem egy listBox-ba hogy minden betöltött kép elérhető legyen
            
            // Teljes elérési útvonal a képhez (elhelyezkedés és képnév)
            string imagePath = Path.Combine(imagesDirectory, imageName);

            // Kép ellenőrzése, hogy létezik-e 
            if (File.Exists(imagePath))
            {
                // Kép betöltése pictureBox-ba
                mainPictureBox.Image = new System.Drawing.Bitmap(imagePath);
            }
            else // ha nem létezne a kép akkor hibeüzenet
            {
                MessageBox.Show("A kép nem található az adott elérési úton!");
            }
        }
        private void mPB_MouseD(object sender, MouseEventArgs e) // pictureBox-on belüli egér lenyomás vizsgálata
        {
            mouseDown = true; //egér lenyomódott a képen belül
            lastLocation = e.Location;
           
            // Ellenőrizzük, hogy egy már létező kör középpontjában kattintottunk-e
            foreach (var center in circleCenters) //ellenőrizzük a már létező lista alapján a középpontokat a 
            {
                if (IsPointInsideCircle(center, e.Location))  //center a megadott sugár és e.Location(hová kattintottunk) alapján határozza meg a koordinátár
                {
                    movingCircleCenter = center; // Ha igen, akkor beállítjuk a mozgatandó kör középpontját
                    circleCenters.Remove(center); // Ideiglenesen eltávolítjuk a listából, hogy újra pozicionálhassuk
                    break;
                }
            }
        }

        private void mPB_MouseM(object sender, MouseEventArgs e) //képen belül mozgattuk-e az egeret
        {
          if (mouseDown && movingCircleCenter.HasValue) //itt jön elő az, hogy miért kellett a mouseDown felvétel ugyan is ha ez nem lenne itt akkor nem egér lenyomásnál mozgatnánk a dolgokat hanem azonnal amint az egér beleér
            {
                // Mozgatjuk a kört az egér új helyzetébe
                movingCircleCenter = new Point(e.X, e.Y); //az új koordinátáit itt adjuk meg a körnek
                mainPictureBox.Invalidate(); // Animációért lenne felelős de a form.app nem képes teljesen lekezelni ezt így akkor is működik az kör áthelyezése ha nem lenne itt ez, a különbség annyi hogy ha ki vesszük akkor a kör marad egész addig amíg át nem helyezzük az új helyére
                mouseMove = true; //itt jön elő az, hogy miért kellett a mouseMove ugyan is, ha nem lenne nem tudnánk vizsgálni, hogy mozgattunk-e valamit a képen belül

            }else if (mouseDown)
            {
                mouseMove = true; //mivel mind a 2 esetben mozgatunk így kell ide is de itt jelen esetben nem a kört mozgatjuk hanem a képet a panelem belül
                int newX = mainPictureBox.Left + (e.X - lastLocation.X);
                int newY = mainPictureBox.Top + (e.Y - lastLocation.Y);
                //itt állítjuk be hogy a panelhez csatol picture box hol legyenek a panelen belül
                mainPictureBox.Left = newX;
                mainPictureBox.Top = newY;
            }
        }
    private void mPB_MouseU(object sender, MouseEventArgs e)
        {
            bool newCircle=false;// annak az ellenőrzése, hogy vettünk már fel új kört és ha igen akkor ne kérdezzen rá egyből az újonnan felvett kör törlésére


            if (movingCircleCenter.HasValue)
            {
                // A mozgatott kör végleges pozícióba helyezése
                circleCenters.Add(movingCircleCenter.Value);
                movingCircleCenter = null; // Befejezzük a kör mozgatását
                mainPictureBox.Invalidate(); // Frissítjük a PictureBox-ot
            }
            else if (!mouseMove) // Új kör felvétele, ha esetleg mozgattuk volna a képet akkor nem ajánlja fel az új kör létrehozását
            {   
                //DialogBox-al kérdezem a létrehozást és a törlést, hogy ne hozzunk létre vagy töröljünk már létező köröket véletlenszerű kattintásoknál 
                DialogResult dialogResult = MessageBox.Show("Fel akarja-e venni a helyszínt", "Felvétel", MessageBoxButtons.YesNo);
                if (dialogResult == DialogResult.Yes)
                {
                    circleCenter = e.Location; // Kattintás helye a kör középpontja
                    circleCenters.Add(circleCenter); // Kör középpontjának hozzáadása a listához
                    newCircle = true;
                }
            }
            if (!newCircle && !mouseMove)
            {
                foreach (var center in circleCenters)
                {
                    if (IsPointInsideCircle(center, e.Location))
                    {
                        // Ha igen, felajánljuk a törlés lehetőségét.
                        DialogResult dialogResult = MessageBox.Show("Törölni akarja a helyszínt?", "Felvétel", MessageBoxButtons.YesNo);
                        if (dialogResult == DialogResult.Yes)
                        {
                            circleCenters.Remove(center); // Törlés, ha a felhasználó igennel válaszol.
                            mainPictureBox.Invalidate();
                            SaveCoords();                              
                            break; // Megtaláltuk a kört, nincs szükség további ellenőrzésre.
                        }
                    }
                }
            }
            //ezek azért kellenek, hogy vissza álíltsuk az alapértelmezett beállításokat 
            mouseDown = false;
            mouseMove = false;

            mainPictureBox.Invalidate(); // PictureBox újrarajzolása(dinamikus változásért)
            SaveCoords(); // Koordináták mentése
        }

        private bool IsPointInsideCircle(Point circleCenter, Point point) => //körök koordinátáinak ellemzése, hogy megjelenített körön belül kattintottunk-e
            Math.Sqrt((circleCenter.X - point.X) * (circleCenter.X - point.X) + (circleCenter.Y - point.Y) * (circleCenter.Y - point.Y)) <= radius;

        private void mPB_paint(object sender, PaintEventArgs e) => //Felrajzolja a köröket a pictureBoxra
            circleCenters.ForEach(center => e.Graphics.FillEllipse(Brushes.Red, center.X - radius, center.Y - radius, 2 * radius, 2 * radius));

        private void SaveCoords() =>  //körök koordinátáinak lementése későbbiekben mssql-ben
            File.WriteAllLines("coordinates.csv", circleCenters.Select(center => $"{center.X},{center.Y}")); 

        private void LoadCoords()
        {
            var filePath = "coordinates.csv"; // későbbiekben mssql-ből
            if (File.Exists(filePath))
            {
                var lines = File.ReadAllLines(filePath);
                circleCenters = lines.Select(line => line.Split(',')).Where(parts => parts.Length == 2 && int.TryParse(parts[0], out int x) && int.TryParse(parts[1], out int y)).Select(parts => new Point(int.Parse(parts[0]), int.Parse(parts[1]))).ToList();
            }
        }
        private void AddCircle_Click(object sender, EventArgs e)
        {
            // Kör hozzáadása
            circleCenters.Add(circleCenter);
            mainPictureBox.Invalidate();
            SaveCoords();
        }
    }
}