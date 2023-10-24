using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Windows.Forms;

namespace _2ndMonitor
{
    public partial class Form1 : Form
    {
        private DataTable dataTable;
        private Timer imageSliderTimer;
        private List<string> imagePaths;  // List of paths to your images
        private int currentImageIndex = 0;

        public Form1()
        {
            InitializeComponent();
            // Get the screens available
            Screen[] screens = Screen.AllScreens;

            // Check if there is a second monitor
            if (screens.Length > 1)
            {
                // Print a message to the console indicating a second monitor is detected
                Console.WriteLine("A second monitor is detected.");

                // Set the window's location to the second monitor (top-left corner)
                Rectangle secondScreenBounds = screens[1].Bounds;
                this.StartPosition = FormStartPosition.Manual;
                this.Location = new Point(secondScreenBounds.Left, secondScreenBounds.Top);

                // Set the window's size to 1366x768
                this.Size = new System.Drawing.Size(1366, 768);
            }
            else
            {
                // If there is only one monitor, set the desired form size
                this.Size = new System.Drawing.Size(1366, 768);
                Console.WriteLine("BUG");
            }

            // Initialize the list of image paths
            imagePaths = new List<string>
            {
                "C:\\Users\\hii\\Documents\\Ads.png",
                "C:\\Users\\hii\\Documents\\Ads2.png",
                "C:\\Users\\hii\\Documents\\Ads3.png",
                "C:\\Users\\hii\\Documents\\Ads4.png"
            };

            // Set the first image
            pictureBox1.Image = Image.FromFile(imagePaths[currentImageIndex]);

            // Initialize the timer for image slideshow
            imageSliderTimer = new Timer();
            imageSliderTimer.Interval = 5000;  // 60 seconds
            imageSliderTimer.Tick += ImageSliderTimer_Tick;
            imageSliderTimer.Start();

            FetchData();
            SetupSqlDependency();

        }

        private void SetupSqlDependency()
        {
            // Start SqlDependency with the application's connection string.
            SqlDependency.Start("Server=DESKTOP-JDQGAO5;Database=easypos;User Id=sa;Password=easyfis;");

            FetchData();
        }

        private void ImageSliderTimer_Tick(object sender, EventArgs e)
        {
            // Move to the next image in the list
            currentImageIndex++;
            if (currentImageIndex >= imagePaths.Count)
            {
                currentImageIndex = 0;  // Reset to the first image if we've shown all images
            }
            pictureBox1.Image = Image.FromFile(imagePaths[currentImageIndex]);
        }
        private void FetchData()
        {
            string connectionString = "Server=DESKTOP-JDQGAO5;Database=easypos;User Id=sa;Password=easyfis;";

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                using (SqlCommand command = new SqlCommand("SELECT TOP 20 ItemCode, ItemDescription, Price FROM MstItem", connection))
                {
                    // Setup the SQL dependency
                    var dependency = new SqlDependency(command);
                    dependency.OnChange += new OnChangeEventHandler(OnDataChanged);

                    // Clear the notification for further use
                    SqlDependency.ExStart(connectionString);

                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        dataTable = new DataTable();
                        dataTable.Load(reader);
                    }
                }
            }
            TableLayoutPanel1_Paint(null, null);
        }

        private void OnDataChanged(object sender, SqlNotificationEventArgs e)
        {
            if (e.Type == SqlNotificationType.Change)
            {
                // Reload the data
                FetchData();
            }
        }



        private void TableLayoutPanel1_Paint(object sender, PaintEventArgs e)
        {
            tableLayoutPanel1.Controls.Clear();
            tableLayoutPanel1.ColumnCount = 4;

            int totalRows = dataTable.Rows.Count + 1;  // +1 for the header
            tableLayoutPanel1.RowCount = totalRows;

            Random random = new Random();
            decimal totalSub = 0; // Initialize the accumulator
            decimal netTotalValue = 0; // Initialize the accumulator

            // Adding Headers
            tableLayoutPanel1.Controls.Add(new Label() { Text = "ItemCode", TextAlign = ContentAlignment.MiddleCenter, Dock = DockStyle.Fill });
            tableLayoutPanel1.Controls.Add(new Label() { Text = "ItemDescription", TextAlign = ContentAlignment.MiddleCenter, Dock = DockStyle.Fill });
            tableLayoutPanel1.Controls.Add(new Label() { Text = "Price", TextAlign = ContentAlignment.MiddleCenter, Dock = DockStyle.Fill });
            tableLayoutPanel1.Controls.Add(new Label() { Text = "Quantity", TextAlign = ContentAlignment.MiddleCenter, Dock = DockStyle.Fill });

            foreach (DataRow row in dataTable.Rows)
            {
                decimal price = Convert.ToDecimal(row["Price"]);
                string formattedPrice = price.ToString("F2");
                int quantity = random.Next(1, 5); // Note: Adjust the range if required
                totalSub += price * quantity; // Update the accumulator
                netTotalValue += price * quantity; // Update the accumulator

                tableLayoutPanel1.Controls.Add(new Label() { Text = row["ItemCode"].ToString(), TextAlign = ContentAlignment.MiddleCenter, Dock = DockStyle.Fill });
                tableLayoutPanel1.Controls.Add(new Label() { Text = row["ItemDescription"].ToString(), TextAlign = ContentAlignment.MiddleCenter, Dock = DockStyle.Fill });
                tableLayoutPanel1.Controls.Add(new Label() { Text = formattedPrice, TextAlign = ContentAlignment.MiddleCenter, Dock = DockStyle.Fill });
                tableLayoutPanel1.Controls.Add(new Label() { Text = quantity.ToString(), TextAlign = ContentAlignment.MiddleCenter, Dock = DockStyle.Fill });
            }
            subTotal.Text = totalSub.ToString("N2");
            NetTotal.Text = netTotalValue.ToString("N2");

            decimal gstValue = netTotalValue * 0.06m; // Calculate 6% GST
            this.GST.Text = gstValue.ToString("N2");
            Balance.Text = netTotalValue.ToString("N2");

            


            int rowHeight = 30;  // Assuming each row to be of 30 units height, you can adjust this based on your need

            // Set the height of the TableLayoutPanel based on the number of rows.
            if (totalRows > 10)
            {
                tableLayoutPanel1.Height = 10 * rowHeight;  // This would potentially activate the scrollbar in the Panel
            }
            else
            {
                tableLayoutPanel1.Height = totalRows * rowHeight;
            }
        }
        protected override void OnClosed(EventArgs e)
        {
            base.OnClosed(e);

            // Stop the SqlDependency when the form closes
            SqlDependency.Stop("Server=DESKTOP-JDQGAO5;Database=easypos;User Id=sa;Password=easyfis;");
        }

    }
}
