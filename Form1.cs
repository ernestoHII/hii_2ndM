using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Windows.Forms;
using System.IO;

namespace _2ndMonitor
{
    public partial class Form1 : Form
    {
        private DataTable dataTable;
        private Timer imageSliderTimer;
/*        private List<string> imagePaths;  // List of paths to your images
*/        private int currentImageIndex = 0;
        private List<string> imagePaths = new List<string>();

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

            InitializeComponent();

            try
            {
                // Initialize the list of image paths and timer
                ReadImagePathsFromConfig();
                currentImageIndex = 0;
                pictureBox1.Image = Image.FromFile(imagePaths[currentImageIndex]);
            }
            catch (Exception ex)
            {
                // Handle the exception gracefully, e.g., by displaying a default image
                // or showing an error message to the user
                pictureBox1.Image = null; // Clear the image
                MessageBox.Show("Error loading image: " + ex.Message);
                Environment.Exit(0); // Forcefully exit the application
            }


            imageSliderTimer = new Timer();
            imageSliderTimer.Tick += ImageSliderTimer_Tick;
            SetTimerIntervalFromConfig();
            imageSliderTimer.Start();
            SetupSqlDependency();
            FetchData();
        }
        private void ReadImagePathsFromConfig()
        {
            try
            {
                int intervalSeconds = 5; // Default interval in seconds

                // Read the config file if it exists
                string configFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "config.txt");
                if (File.Exists(configFilePath))
                {
                    string[] lines = File.ReadAllLines(configFilePath);

                    foreach (string line in lines)
                    {
                        if (line.StartsWith("ImagePaths:"))
                        {
                            // Start reading image paths
                            continue;
                        }

                        if (line.StartsWith("TimerIntervalSeconds:"))
                        {
                            // Adjust the timer interval
                            if (int.TryParse(line.Replace("TimerIntervalSeconds:", "").Trim(), out int parsedIntervalSeconds))
                            {
                                intervalSeconds = parsedIntervalSeconds;
                            }
                        }
                        else
                        {
                            // Add image paths
                            imagePaths.Add(line.Trim());
                        }
                    }
                }

                // Initialize the timer with the interval read from the config file
                imageSliderTimer = new Timer();
                imageSliderTimer.Interval = intervalSeconds * 1000; // Convert to milliseconds
                imageSliderTimer.Tick += ImageSliderTimer_Tick;
                imageSliderTimer.Start();

                // Now you can safely use imageSliderTimer
            }
            catch (Exception ex)
            {
                // Handle exceptions if any other error occurs (optional)
                MessageBox.Show("Error reading config.txt: " + ex.Message);
                imagePaths = new List<string>(); // Default image paths
            }
        }


        private void SetTimerIntervalFromConfig()
        {
            try
            {
                string configFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "config.txt");

                // Create a default config file with default values if it doesn't exist
                if (!File.Exists(configFilePath))
                {
                                        string defaultConfigContents = $@"# Configuration for image slideshow
                    ImagePaths:
                    DefaultImage.png  # Specify the default image location in the same folder
                    TimerIntervalSeconds: 5";
                    File.WriteAllText(configFilePath, defaultConfigContents);
                }

                // Read the timer interval from the config file
                string[] lines = File.ReadAllLines(configFilePath);
                foreach (string line in lines)
                {
                    if (line.StartsWith("TimerIntervalSeconds:"))
                    {
                        // Adjust the timer interval
                        int intervalSeconds;
                        if (int.TryParse(line.Replace("TimerIntervalSeconds:", "").Trim(), out intervalSeconds))
                        {
                            imageSliderTimer.Interval = intervalSeconds * 1000; // Convert to milliseconds
                        }
                        break; // No need to read further
                    }
                }
            }
            catch (Exception)
            {
                // Handle exceptions if any other error occurs (optional)
            }
        }




        private void SetupSqlDependency()
        {
            // Start the SqlDependency listener.
            string connectionString = "Server=localhost;Database=easypos;User Id=notifman;Password=root1234;";

            SqlDependency.Start(connectionString);
        }
        private void ImageSliderTimer_Tick(object sender, EventArgs e)
        {
            // Display the next image in the list
            currentImageIndex++;
            if (currentImageIndex >= imagePaths.Count)
            {
                currentImageIndex = 0;
            }
            try
            {
                // Initialize the list of image paths and timer
                ReadImagePathsFromConfig();
                currentImageIndex = 0;
                pictureBox1.Image = Image.FromFile(imagePaths[currentImageIndex]);
            }
            catch (Exception ex)
            {
                // Handle the exception gracefully, e.g., by displaying a default image
                // or showing an error message to the user
                pictureBox1.Image = null; // Clear the image
                MessageBox.Show("Error loading image: " + ex.Message);
                Environment.Exit(0); // Forcefully exit the application
            }

        }
        private void FetchData()
        {           
            string connectionString = "Server=localhost;Database=easypos;User Id=notifman;Password=root1234;";

            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    using (SqlCommand command = new SqlCommand("SELECT ItemCode, ItemDescription, Price FROM dbo.MstItem", connection))
                    {
                        // Setup the SQL dependency
                        var dependency = new SqlDependency(command);
                        dependency.OnChange += new OnChangeEventHandler(OnDataChanged);

                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            dataTable = new DataTable();
                            dataTable.Load(reader);
                        }
                    }
                }
                TableLayoutPanel1_Paint(null, null);
            }
            catch (SqlException ex)
            {
                MessageBox.Show($"Error in database operation: {ex.Message}");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"General error: {ex.Message}");
            }
        
        }
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }

            string connectionString = "Server=DESKTOP-JDQGAO5;Database=easypos;User Id=sa;Password=easyfis;";
            SqlDependency.Stop(connectionString);

            base.Dispose(disposing);
        }
        private void OnDataChanged(object sender, SqlNotificationEventArgs e)
        {
            // Handle the data change event here.
            SqlDependency dependency = sender as SqlDependency;
            if (dependency != null)
            {
                dependency.OnChange -= OnDataChanged;
            }

            // Check if the change type is an insert, update, or delete.
            if (e.Info == SqlNotificationInfo.Insert || e.Info == SqlNotificationInfo.Update || e.Info == SqlNotificationInfo.Delete)
            {
                // Check if the form's handle has been created.
                if (this.IsHandleCreated)
                {
                    this.Invoke(new MethodInvoker(() =>
                    {
                        FetchData(); // Assuming FetchData() only modifies controls
                    }));
                }
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
