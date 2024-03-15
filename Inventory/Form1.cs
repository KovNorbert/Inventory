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
        private Point lastLocation; // Azért kell hogy le tudjuk menteni és vizsgálni, hogy hol nyomtuk le az egeret a pictureboxon belül, kép mozgatásához 
        private Point circleCenter; // A kör középpontjának koordinátái, amit későbbiekben összevetünk a lactLocation-el(elmentett koordináttákal)
        private List<Point> circleCenters = new List<Point>(); // A korábban létrehozott körök száma és azoknak a koordinátája 
        private Point? movingCircleCenter = null; // A mozgatott kör középpontja (null, ha éppen nem mozgatunk kört)
        
        // Kör méretezéséhez használt segédek 
        private const int initialRadius = 10; // Kezdeti rádiusz érték
        private int radius = initialRadius; // Jelenlegi rádiusz, amit skálázunk

        //"animálási segédlet"
        private int circleFoundI = -1; // Új változó a mozgatott kör indexének tárolására, lehetővé teszi nekünk a "minimális animálást", azért -1 a kezdőérték mert az indexelést jelen esetben 0val kezdem így előfurdúlhat, hogy elkapnám a 0-értéket

        //private ContextMenuStrip contextMenuStrip1; // A kis menü ablak(még kérdéses a beépítés)
        public MainForm()
        {
            InitializeComponent();
            LoadCoords(); // Koordináták betöltése

            // ContextMenuStrip létrehozása abban az esetben ha a törlés meg létrehozást külön jobb clickre akarnám tenni.
            //contextMenuStrip1 = new ContextMenuStrip();
            //contextMenuStrip1.Items.Add("Hozzáadás", null, AddCircle_Click);
            //mainPictureBox.ContextMenuStrip = contextMenuStrip1;
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
            mouseDown = true; //mivel lenyomtuk a gombot így ez igaz lesz 
            lastLocation = e.Location; //kattintási koordináta lementése későbbre
            Point imageCoords = ConvertScreenCoordsToImageCoords(e.Location);

            //végig megyünk a körök adatbázisán és vizsgáljuk, hogy belekattintottunk-e egybe vagy sem illetve elmentjük, hogy melyik körbe kattintottunk bele
            for (int i = 0; i < circleCenters.Count; i++)
            {
                if (IsPointInsideCircle(circleCenters[i], imageCoords))
                {
                    circleFoundI = i;
                    movingCircleCenter = circleCenters[i];
                    break;
                }
            }
        }

        private void mPB_MouseM(object sender, MouseEventArgs e) //képen belül mozgattuk-e az egeret
        {
            Point imageCoords = ConvertScreenCoordsToImageCoords(e.Location); // Átszámított kép koordináták

            if (circleFoundI != -1 && mouseDown) //körre kattintás vizsgálata ha igen áthelyezzük a kört 
            {               
                circleCenters[circleFoundI] = new Point(movingCircleCenter.Value.X + (imageCoords.X - movingCircleCenter.Value.X), movingCircleCenter.Value.Y + (imageCoords.Y - movingCircleCenter.Value.Y));
                mainPictureBox.Invalidate();
                mouseMove = true;//vizsgáljuk a mozgást
            }
            else if (mouseDown && circleFoundI <= 0)
            {
                mouseMove = true; //mivel mind a 2 esetben mozgatunk így kell ide is de itt jelen esetben nem a kört mozgatjuk hanem a képet a panelem belül
                int newX = mainPictureBox.Left + (e.X - lastLocation.X);
                int newY = mainPictureBox.Top + (e.Y - lastLocation.Y);
                
                //panelen belüli helyzet beállítása és korlátozása a pictureboxnak
                // ne lépjen ki a panel területéről:
                int maxRight = mainPanel.Width - mainPictureBox.Width;
                int maxBottom = mainPanel.Height - mainPictureBox.Height;

                // Ellenőrizzük, hogy a kívánt új X,Y pozíciók a panel határain belül vannak-e
                if (newX > 0) newX = 0; // Ne hagyja el a panel bal oldalát
                if (newY > 0) newY = 0; // Ne hagyja el a panel tetejét
                if (newX < maxRight) newX = maxRight; // Ne hagyja el a panel jobb oldalát
                if (newY < maxBottom) newY = maxBottom; // Ne hagyja el a panel alját

                // Az új pozíció korlátokon belül
                mainPictureBox.Left = newX;
                mainPictureBox.Top = newY;
            }
        }
        private void mPB_MouseU(object sender, MouseEventArgs e)
        {
            bool newCircle = false;// annak az ellenőrzése, hogy vettünk már fel új kört és ha igen akkor ne kérdezzen rá egyből az újonnan felvett kör törlésére

            Point imageCoords = ConvertScreenCoordsToImageCoords(e.Location);
            if (circleFoundI != -1)
            {
                SaveCoords(); // Módosítások mentése
                circleFoundI = -1;
                movingCircleCenter = null;
            }
            else if (movingCircleCenter.HasValue)
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
                    circleCenters.Add(imageCoords); // Kör középpontjának hozzáadása a listához
                    newCircle = true;                   
                }
            }
            if (!newCircle && !mouseMove)
            {
                foreach (var center in circleCenters)
                {
                    if (IsPointInsideCircle(center, imageCoords))
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
            //alapértelmezett beállítások visszaállítása 
            mouseDown = false;
            mouseMove = false;
            
            mainPictureBox.Invalidate(); // PictureBox újrarajzolása(dinamikus változásért)
            SaveCoords(); // Koordináták mentése
        }

        private void mP_MouseW(object sender, MouseEventArgs e) //zoom-in & -out-hoz és körök méretezéséhez szükséges 
        {
            const float zoomFactor = 1.1f; // Zoom mértékének beállítása

            if (mainPictureBox.SizeMode != PictureBoxSizeMode.Zoom) //kép alap beállítása auto-size aminek csak kényelmi okai vannak, de mivel azzal nem tudjuk megoldani a zoom parancsot ezért át kell állítani .Zoom-ra
            {
                mainPictureBox.SizeMode = PictureBoxSizeMode.Zoom;
                mainPictureBox.Width = mainPictureBox.Image.Width;
                mainPictureBox.Height = mainPictureBox.Image.Height;
            }


            // Számítsuk ki az új méreteket előre, hogy ellenőrizhessük őket
            int newWidth = (e.Delta > 0) ? (int)(mainPictureBox.Width * zoomFactor) : (int)(mainPictureBox.Width / zoomFactor);
            int newHeight = (e.Delta > 0) ? (int)(mainPictureBox.Height * zoomFactor) : (int)(mainPictureBox.Height / zoomFactor);

            // Ellenőrizzük, hogy a zoomolás után a méret nem lépi-e túl a megengedett maximumot
            if (newWidth > 1500 || newHeight > 1500) //általam megadott érték az alap képnél szépen néz ki több kép esetében tesztelés szükséges 
            {
                // Ha a kiszámított új méret túllépné a maximális zoom méretet, nem végezzük el a zoomolást
                return;
            }

            // Ellenőrizzük, hogy az új méretek nem kisebbek-e, mint a Panel méretei
            if ((e.Delta < 0 && mainPictureBox.Width > mainPanel.Width) || e.Delta > 0) //ez is kényelmi funkció hogy ne legyen túl kicsi
            {
                mainPictureBox.Width = newWidth;
                mainPictureBox.Height = newHeight;

            }

            mainPictureBox.Left = 0;
            mainPictureBox.Top = 0;
            mainPictureBox.Invalidate(); // Az újrarajzolás engedélyezése
        }

        private bool IsPointInsideCircle(Point circleCenter, Point point) => //körök koordinátáinak ellemzése, hogy megjelenített körön belül kattintottunk-e
            Math.Sqrt((circleCenter.X - point.X) * (circleCenter.X - point.X) + (circleCenter.Y - point.Y) * (circleCenter.Y - point.Y)) <= radius;

        private void mPB_paint(object sender, PaintEventArgs e)
        {
            Graphics g = e.Graphics;

            // Feltételezve, hogy a circleCenters lista az eredeti, skálázatlan koordinátákat tartalmazza
            float scaleX = (float)mainPictureBox.Width / mainPictureBox.Image.Width;
            float scaleY = (float)mainPictureBox.Height / mainPictureBox.Image.Height;

            foreach (Point center in circleCenters)
            {
                // Alkalmazzuk a skálázást az eredeti pontok koordinátáira minden egyes újrarajzoláskor
                int scaledX = (int)(center.X * scaleX);
                int scaledY = (int)(center.Y * scaleY);
                int scaledRadius = (int)(radius * Math.Min(scaleX, scaleY)); // A rádiusz skálázása

                // Rajzoljuk a köröket az új, skálázott koordinátákon
                g.FillEllipse(Brushes.Red, scaledX - scaledRadius, scaledY - scaledRadius, 2 * scaledRadius, 2 * scaledRadius);
            }
        }

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
        //private void AddCircle_Click(object sender, EventArgs e)
        //{
        //    // Kör hozzáadása
        //    circleCenters.Add(circleCenter);
        //    mainPictureBox.Invalidate();
        //    SaveCoords();
        //}
        private Point ConvertScreenCoordsToImageCoords(Point screenCoords)
        {
            float scaleX = (float)mainPictureBox.Image.Width / mainPictureBox.Width;
            float scaleY = (float)mainPictureBox.Image.Height / mainPictureBox.Height;

            int x = (int)(screenCoords.X * scaleX);
            int y = (int)(screenCoords.Y * scaleY);

            return new Point(x, y);
        }

        private Point ConvertImageCoordsToScreenCoords(Point imageCoords)
        {
            // Az eredeti kép koordinátáit a PictureBox százalékos pozíciójába konvertáljuk
            float xPercent = imageCoords.X / (float)mainPictureBox.Image.Width;
            float yPercent = imageCoords.Y / (float)mainPictureBox.Image.Height;

            // Képernyő koordinátáinak kiszámítása a PictureBox aktuális méretét figyelembe véve
            int x = (int)(xPercent * mainPictureBox.Width);
            int y = (int)(yPercent * mainPictureBox.Height);

            return new Point(x, y);
        }
    }
}