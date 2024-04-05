using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using System.Data.SqlClient;
using System.Runtime.Remoting.Metadata.W3cXsd2001;

namespace Inventory
{
    public partial class MainForm : Form
    {
        // egér vizsgálatához szükséges mert mind a 3 eseményében kezelünk valamit
        private bool mouseDown; //Azért kell hogy le tudjuk menteni és vizsgálni, hogy a pictureBoxon belül nyomtuk le az egér gombot
        private bool mouseMove; //Azért kell hogy le tudjuk menteni és vizsgálni, hogy a pictureBoxon belül mozgattuk az egeret

        private Point lastLocation; // Azért kell hogy le tudjuk menteni és vizsgálni, hogy hol nyomtuk le az egeret a pictureboxon belül, kép mozgatásához 
        private List<Point> circleCenters = new List<Point>(); // A korábban létrehozott körök száma és azoknak a koordinátája 
        private Point? movingCircleCenter = null; // A mozgatott kör középpontja (null, ha éppen nem mozgatunk kört)
                                                  //private Point circleCenter; // A kör középpontjának koordinátái, amit későbbiekben összevetünk a lactLocation-el(elmentett koordináttákal)
                                                  // Kör méretezéséhez használt segédek 
        private const int initialRadius = 10; // Kezdeti rádiusz érték
        private int radius = initialRadius; // Jelenlegi rádiusz, amit skálázunk

        //"animálási segédlet" + később sql hozzáadásánál segédlet a pontok és adatok egyesítéséhez 
        private int circleFoundI = -1; // Új változó a mozgatott kör indexének tárolására, lehetővé teszi nekünk a "minimális animálást", azért -1 a kezdőérték mert az indexelést jelen esetben 0val kezdem így előfurdúlhat, hogy elkapnám a 0-értéket
        private Point originalImageCoords;

        //--
        //SQL
        private string connectionString = @"Server=KONO-PC; Database=Inventory; Integrated Security=True;";

        //képbetöltés
        ImgLoader Loader = new ImgLoader(@"Server=KONO-PC; Database=Inventory; Integrated Security=True;");
        //---------------------------------------------------------------------------------------------------------------------------------------
        Point pointForSave = new Point();

        private Point lastImageCoords;
        public MainForm()
        {
            InitializeComponent();
        }
        private void MainForm_Load(object sender, EventArgs e) // későbbiekben egy halom mssql kódsor lesz itt 
        {
            string connectionString = @"Data Source=KONO-PC;Initial Catalog=Inventory;Integrated Security=True;";
            string query = "SELECT MapName FROM Map";

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            mapComboBox.Items.Add(reader["MapName"].ToString());
                        }
                    }
                }
            }
            if (mapComboBox.Items.Count > 0)
                mapComboBox.SelectedIndex = 0;
        }
        private void mPB_MouseD(object sender, MouseEventArgs e)
        {
            circleFoundI = -1; mouseDown = false;

            mouseDown = true;
            // Kattintási koordináta lementése
            lastLocation = e.Location;
            Point imageCoords = ConvertScreenCoordsToImageCoords(e.Location);

            lastImageCoords = imageCoords; // Eredeti kattintási helyzet mentése
            

            for (int i = 0; i < circleCenters.Count; i++)
            {
                if (e.Button == MouseButtons.Right && IsPointInsideCircle(circleCenters[i], imageCoords))
                {
                    circleFoundI = i;
                    mouseDown = false;
                }
                else
                {
                    if (IsPointInsideCircle(circleCenters[i], imageCoords))
                    {
                        circleFoundI = i;
                        movingCircleCenter = circleCenters[i];                        
                        originalImageCoords = circleCenters.ElementAtOrDefault(circleFoundI); // Eredeti pont helyzetének mentése
                        break;
                    }
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
            coordLabel.Text = $"X: {e.X}, Y: {e.Y}";
        }
        private void mPB_MouseU(object sender, MouseEventArgs e)
        {
            Point imageCoords = ConvertScreenCoordsToImageCoords(e.Location);

            if (e.Button == MouseButtons.Right)
            {
                for (int i = 0; i < circleCenters.Count; i++)
                {
                    if (IsPointInsideCircle(circleCenters[i], imageCoords))
                    {
                        circleFoundI = i;
                        ModyNDelContextMenu();
                        break; // Találtunk egy kört, ne keressünk tovább
                    }
                    
                }
            }
            if (movingCircleCenter.HasValue && mouseDown)
            {
                // Az új pozíció kiszámítása
                pointForSave = ConvertScreenCoordsToImageCoords(e.Location);

                // Az adatbázisban lévő rekord frissítése előtti megerősítés
                DialogResult dialogResult = MessageBox.Show("Akarja menteni az eszköz új helyét?", "Áthelyezés", MessageBoxButtons.YesNo);
                if (dialogResult == DialogResult.Yes)
                {
                    // Ha "Igen", frissítjük az adatbázist és a UI-t
                    UpdateCircleLocationInDatabase(movingCircleCenter.Value, pointForSave);
                    MessageBox.Show("Új koordináták elmentve.");
                }
                else
                {
                    // Ha "Nem", visszaállítjuk az eredeti pozíciót
                    circleCenters[circleFoundI] = originalImageCoords;
                }

                // Lista és UI frissítése
                movingCircleCenter = null; // Befejezzük a kör mozgatását
                mainPictureBox.Invalidate(); // Frissítjük a PictureBox-ot
            }

            if (circleFoundI != -1 && mouseDown)
            {
                // SaveCoords(); // Módosítások mentése
                circleFoundI = -1;
                movingCircleCenter = null;
            }
            else if (movingCircleCenter.HasValue && mouseDown)
            {
                // A mozgatott kör végleges pozícióba helyezése
                circleCenters.Add(movingCircleCenter.Value);
                movingCircleCenter = null; // Befejezzük a kör mozgatását
                mainPictureBox.Invalidate(); // Frissítjük a PictureBox-ot
            }
            //else if (!mouseMove && mouseDown) // Új kör felvétele, ha esetleg mozgattuk volna a képet akkor nem ajánlja fel az új kör létrehozását
            //{

            //    //DialogBox-al kérdezem a létrehozást és a törlést, hogy ne hozzunk létre vagy töröljünk már létező köröket véletlenszerű kattintásoknál 
            //    DialogResult dialogResult = MessageBox.Show("Fel akarja-e venni a helyszínt", "Felvétel", MessageBoxButtons.YesNo);
            //    if (dialogResult == DialogResult.Yes)
            //    {
            //        circleCenters.Add(imageCoords); // Kör középpontjának hozzáadása a listához
            //        newCircle = true;
            //    }
            //}
            //if (!newCircle && !mouseMove && mouseDown)
            //{
            //    foreach (var center in circleCenters)
            //    {
            //        if (IsPointInsideCircle(center, imageCoords))
            //        {
            //            // Ha igen, felajánljuk a törlés lehetőségét.
            //            DialogResult dialogResult = MessageBox.Show("Törölni akarja a helyszínt?", "Felvétel", MessageBoxButtons.YesNo);
            //            if (dialogResult == DialogResult.Yes)
            //            {
            //                circleCenters.Remove(center); // Törlés, ha a felhasználó igennel válaszol.
            //                mainPictureBox.Invalidate();
            //                //SaveCoords();
            //                break; // Megtaláltuk a kört, nincs szükség további ellenőrzésre.
            //            }
            //        }
            //    }
            //}
            //alapértelmezett beállítások visszaállítása 
            mouseDown = false;
            mouseMove = false;
            mainPictureBox.Invalidate(); // PictureBox újrarajzolása(dinamikus változásért)
            //SaveCoords(); // Koordináták mentése

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
            // Kiszámítjuk az új méreteket előre, hogy ellenőrizhessük őket
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
        private void LoadCoords(int mapId)
        {
            circleCenters.Clear(); // Először tisztítjuk a meglévő koordinátákat

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                var command = new SqlCommand("SELECT X, Y FROM ToolLocation WHERE MapID = @MapID", connection);
                command.Parameters.AddWithValue("@MapID", mapId);

                using (SqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        int x = reader.GetInt32(0);
                        int y = reader.GetInt32(1);
                        circleCenters.Add(new Point(x, y));
                    }
                }
            }
            mainPictureBox.Invalidate(); // Frissítsük a képet a betöltött koordinátákkal
        }
        private Point ConvertScreenCoordsToImageCoords(Point screenCoords)
        {
            float scaleX = (float)mainPictureBox.Image.Width / mainPictureBox.Width;
            float scaleY = (float)mainPictureBox.Image.Height / mainPictureBox.Height;

            int x = (int)(screenCoords.X * scaleX);
            int y = (int)(screenCoords.Y * scaleY);

            return new Point(x, y);
        }
        //private Point ConvertImageCoordsToScreenCoords(Point imageCoords)
        //{
        //    // Az eredeti kép koordinátáit a PictureBox százalékos pozíciójába konvertáljuk
        //    float xPercent = imageCoords.X / (float)mainPictureBox.Image.Width;
        //    float yPercent = imageCoords.Y / (float)mainPictureBox.Image.Height;

        //    // Képernyő koordinátáinak kiszámítása a PictureBox aktuális méretét figyelembe véve
        //    int x = (int)(xPercent * mainPictureBox.Width);
        //    int y = (int)(yPercent * mainPictureBox.Height);

        //    return new Point(x, y);
        //}
        private void button1_Click(object sender, EventArgs e)
        {
            MapForm mapForm = new MapForm();
            mapForm.Show();
        }
        private void mCB_selectedIChanged(object sender, EventArgs e)
        {
            LoadSelectedImage();

        }
        private void LoadSelectedImage()
        {
            if (mapComboBox.SelectedIndex == -1) return;

            string selectedMapName = mapComboBox.SelectedItem.ToString();
            int selectedMapId = GetMapIdByName(selectedMapName);

            // Töltsd be a képet
            System.Drawing.Image image = Loader.LoadImage(selectedMapName);
            if (image != null)
            {
                mainPictureBox.Image = image;
                LoadCoords(selectedMapId); // Itt hívjuk meg a LoadCoords-t a kiválasztott MapID-vel
            }
        }
        private int GetMapIdByName(string mapName)
        {
            int mapId = -1;
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                var command = new SqlCommand("SELECT MapID FROM Map WHERE MapName = @MapName", connection);
                command.Parameters.AddWithValue("@MapName", mapName);

                using (SqlDataReader reader = command.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        mapId = reader.GetInt32(0);
                    }
                }
            }
            return mapId;
        }
        private void UpdateCircleLocationInDatabase(Point originalLocation, Point newLocation)
        {
            int selectedMapId = GetMapIdByName(mapComboBox.SelectedItem.ToString());

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                var command = new SqlCommand("UPDATE ToolLocation SET X = @NewX, Y = @NewY WHERE MapID = @MapID AND X = @OriginalX AND Y = @OriginalY", connection);
                command.Parameters.AddWithValue("@NewX", newLocation.X);
                command.Parameters.AddWithValue("@NewY", newLocation.Y);
                command.Parameters.AddWithValue("@MapID", selectedMapId);
                command.Parameters.AddWithValue("@OriginalX", originalLocation.X);
                command.Parameters.AddWithValue("@OriginalY", originalLocation.Y);

                command.ExecuteNonQuery();
            }
        }
        //private void SaveCoords()
        //{
        //    if (mapComboBox.SelectedIndex == -1 || movingCircleCenter == null) return;

        //    string selectedMapName = mapComboBox.SelectedItem.ToString();
        //    int selectedMapId = GetMapIdByName(selectedMapName);

        //    using (SqlConnection connection = new SqlConnection(connectionString))
        //    {
        //        connection.Open();

        //        // Mivel csak egy kör pozícióját frissítjük, itt nem törlünk
        //        var updateCommand = new SqlCommand("UPDATE ToolLocation SET X = @X, Y = @Y WHERE ToolWorkID = @ToolWorkID AND MapID = @MapID", connection);
        //        updateCommand.Parameters.AddWithValue("@X", movingCircleCenter.Value.X);
        //        updateCommand.Parameters.AddWithValue("@Y", movingCircleCenter.Value.Y);
        //        updateCommand.Parameters.AddWithValue("@ToolWorkID", toolWorkId);
        //        updateCommand.Parameters.AddWithValue("@MapID", selectedMapId);
        //    }
        //}

        private void dBButton_Click(object sender, EventArgs e)
        {

        }

        private void aboutStripMenuItem_Click(object sender, EventArgs e)
        {
            AboutForm aboutForm = new AboutForm();
            aboutForm.ShowDialog();
        }

        private void saveStripMenuItem_Click(object sender, EventArgs e)
        {
            // SaveCoords(); // Koordináták mentése
        }

        private void ModyNDelContextMenu()
        {
            ContextMenuStrip contextMenu = new ContextMenuStrip();
            //ToolStripMenuItem addMenuItem = new ToolStripMenuItem("Hozzáadás");
            ToolStripMenuItem deleteMenuItem = new ToolStripMenuItem("Törlés");
            ToolStripMenuItem infoMenuItem = new ToolStripMenuItem("Információ");

            //contextMenu.Items.Add(addMenuItem);
            contextMenu.Items.Add(deleteMenuItem);
            contextMenu.Items.Add(infoMenuItem);

            // Eseménykezelők hozzáadása
            infoMenuItem.Click += InfoMenuItem_Click;
            deleteMenuItem.Click += DeleteMenuItem_Click;
            //addMenuItem.Click += AddMenuItem_Click;

            mainPictureBox.ContextMenuStrip = contextMenu;
        }

        private void AddMenuItem_Click(object sender, EventArgs e)
        {
            ToolSelectionForm selectionForm = new ToolSelectionForm();
            var dialogResult = selectionForm.ShowDialog();

            if (dialogResult == DialogResult.OK)
            {
                int selectedToolWorkId = selectionForm.SelectedToolWorkId;               

                // Az új ToolLocation mentése az adatbázisba a lastImageCoords és a kiválasztott MapID alapján.
                SaveToolLocationToDatabase(selectedToolWorkId, lastImageCoords);
            }
        }

        private void InfoMenuItem_Click(object sender, EventArgs e)
        {
            if (circleFoundI < 0 || circleFoundI >= circleCenters.Count) return;

            // A kiválasztott kör pozíciója
            Point selectedCircle = circleCenters[circleFoundI];
            // A jelenleg kiválasztott térkép azonosítójának megszerzése
            int mapId = GetMapIdByName(mapComboBox.SelectedItem.ToString());

            string query1 = @"
                SELECT ToolWorkID 
                FROM ToolLocation 
                WHERE X = @X AND Y = @Y AND MapID = @MapID";

            int toolWorkId = -1; // Alapértelmezett érték, ha nem található az eszköz

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                using (SqlCommand cmd = new SqlCommand(query1, conn))
                {
                    // Paraméterek hozzáadása a lekérdezéshez
                    cmd.Parameters.AddWithValue("@X", selectedCircle.X);
                    cmd.Parameters.AddWithValue("@Y", selectedCircle.Y);
                    cmd.Parameters.AddWithValue("@MapID", mapId);

                    object result = cmd.ExecuteScalar(); // Csak egy értéket várunk
                    if (result != null)
                    {
                        toolWorkId = Convert.ToInt32(result);
                    }
                }
            }
            string query = @"
                SELECT t.ToolWorkID, t.ToolSerialNumber, t.ToolName, t.ToolDescription, t.ToolCategory, 
                    p.PersonID, p.PersonName, p.PersonPosition, m.MapName
                FROM Tool t
                LEFT JOIN Issued i ON t.ToolWorkID = i.ToolWorkID
                LEFT JOIN Person p ON i.PersonID = p.PersonID
                LEFT JOIN ToolLocation tl ON t.ToolWorkID = tl.ToolWorkID
                LEFT JOIN Map m ON tl.MapID = m.MapID
                WHERE t.ToolWorkID = @ToolWorkID AND tl.MapID = @MapID"; 

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@ToolWorkID", toolWorkId); // A ToolWorkID paraméter hozzáadása
                    cmd.Parameters.AddWithValue("@MapID", mapId); // A MapID paraméter hozzáadása

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            // InfoForm megnyitása a lekérdezett adatokkal
                            InfoForm infoForm = new InfoForm();
                            infoForm.SetData(
                                reader["ToolWorkID"].ToString(),
                                reader["ToolSerialNumber"].ToString(),
                                reader["ToolName"].ToString(),
                                reader["ToolDescription"].ToString(),
                                reader["ToolCategory"].ToString(),
                                reader["PersonID"].ToString(),
                                reader["PersonName"].ToString(),
                                reader["PersonPosition"].ToString(),
                                reader["MapName"].ToString()
                            );
                            infoForm.ShowDialog();
                        }
                        else
                        {
                            MessageBox.Show("Nem található információ az eszközről.", "Információ", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        }
                    }
                }
            }
        }
        private void DeleteMenuItem_Click(object sender, EventArgs e)
        {
            DialogResult dialogResult = MessageBox.Show("Biztos törölni akarja a képen lévő jelölést", "Törlés", MessageBoxButtons.YesNo);
            if (dialogResult == DialogResult.Yes)
            {

            
            if (circleFoundI < 0 || circleFoundI >= circleCenters.Count) return;

            // A kiválasztott kör pozíciójának és a térkép azonosítójának megszerzése
            Point selectedCircle = circleCenters[circleFoundI];
            int mapId = GetMapIdByName(mapComboBox.SelectedItem.ToString());

            // A ToolWorkID lekérdezése
            int toolWorkId = -1;
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                string query = @"
                    SELECT ToolWorkID 
                    FROM ToolLocation 
                    WHERE X = @X AND Y = @Y AND MapID = @MapID";
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@X", selectedCircle.X);
                    cmd.Parameters.AddWithValue("@Y", selectedCircle.Y);
                    cmd.Parameters.AddWithValue("@MapID", mapId);

                    object result = cmd.ExecuteScalar();
                    if (result != null)
                    {
                        toolWorkId = Convert.ToInt32(result);
                    }
                }
            }

            // Ha van ToolWorkID, akkor töröljük a kört az adatbázisból
            if (toolWorkId != -1)
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    string deleteQuery = "DELETE FROM ToolLocation WHERE ToolWorkID = @ToolWorkID";
                    using (SqlCommand cmd = new SqlCommand(deleteQuery, conn))
                    {
                        cmd.Parameters.AddWithValue("@ToolWorkID", toolWorkId);
                        cmd.ExecuteNonQuery();
                    }
                }
                // Töröljük a kört a felhasználói felületről
                circleCenters.RemoveAt(circleFoundI);
                mainPictureBox.Invalidate();

                MessageBox.Show("A kör sikeresen törölve lett.", "Törlés megerősítése", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else
            {
                MessageBox.Show("A kör nem található az adatbázisban.", "Törlési hiba", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            circleFoundI = -1; // Reseteljük az indexet
            }
        }

        private void SaveToolLocationToDatabase(int toolWorkId, Point location)
        {
            int mapId = GetMapIdByName(mapComboBox.SelectedItem.ToString());
            string query = @"
                INSERT INTO ToolLocation (ToolWorkID, MapID, X, Y)
                VALUES (@ToolWorkID, @MapID, @X, @Y)";

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@ToolWorkID", toolWorkId);
                    cmd.Parameters.AddWithValue("@MapID", mapId); // Az aktuális MapID használata
                    cmd.Parameters.AddWithValue("@X", location.X);
                    cmd.Parameters.AddWithValue("@Y", location.Y);

                    cmd.ExecuteNonQuery();
                }
            }

            // Frissítjük a PictureBox-ot, hogy megjelenítse az új pontot.
            mainPictureBox.Invalidate();
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void térképToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MapForm mapForm = new MapForm();
            mapForm.Show();
        }

        private void adatokToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SearchForm SearchForm = new SearchForm();
            SearchForm.Show();
        }

        private void pontToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ToolSelectionForm selectionForm = new ToolSelectionForm();
            var dialogResult = selectionForm.ShowDialog();

            if (dialogResult == DialogResult.OK)
            {
                int selectedToolWorkId = selectionForm.SelectedToolWorkId;
                // A fix koordináták beállítása (15, 15)
                Point fixedLocation = new Point(15, 15);

                // Az aktuális MapID lekérése a ComboBox-ból
                int currentMapId = GetMapIdByName(mapComboBox.SelectedItem.ToString());
                if (currentMapId != -1)
                {
                    SaveToolLocationToDatabase(selectedToolWorkId, fixedLocation);
                    // Új pont hozzáadása a circleCenters listához, hogy megjelenjen a PictureBox-ban
                    circleCenters.Add(fixedLocation);
                    // Frissítjük a PictureBox-ot, hogy megjelenítse az új pontot
                    mainPictureBox.Invalidate();
                }
                else
                {
                    MessageBox.Show("Érvénytelen térkép azonosító. Kérjük, válasszon egy érvényes térképet.");
                }
            }
        }
        private void kiadásToolStripMenuItem_Click(object sender, EventArgs e)
        {
            AssignToolForm assignTool = new AssignToolForm();
            assignTool.Show();
        }

        private void visszavételToolStripMenuItem_Click(object sender, EventArgs e)
        {
            RemoveToolsForm removeToolsForm = new RemoveToolsForm();
            removeToolsForm.Show();
        }
    }
}