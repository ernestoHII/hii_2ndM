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
        private List<string> imagePaths;  // List of paths to your images
        private int currentImageIndex = 0;

        public Form1()
        {
            InitializeComponent();
            ReadImagePathsFromConfig();

            imageSliderTimer = new Timer();
            imageSliderTimer.Tick += ImageSliderTimer_Tick;
            SetTimerIntervalFromConfig();
            imageSliderTimer.Start();

            // Set the first image if available
            if (imagePaths.Count > 0)
            {
                pictureBox1.Image = Image.FromFile(imagePaths[currentImageIndex]);
            }

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

            try
            {
                if (pictureBox1 == null)
                {
                    throw new InvalidOperationException("pictureBox1 is not initialized.");
                }

                if (imagePaths == null || imagePaths.Count == 0)
                {
                    throw new InvalidOperationException("No image paths available.");
                }

                currentImageIndex = 0;
                var imagePath = imagePaths[currentImageIndex];

                if (!File.Exists(imagePath))
                {
                    throw new FileNotFoundException("The image file was not found.", imagePath);
                }

                pictureBox1.Image = Image.FromFile(imagePath);
            }
            catch (Exception ex)
            {
                // This will now tell you exactly what went wrong
                MessageBox.Show("Error loading image1: " + ex.Message);
                Environment.Exit(0);
            }

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

                    // Assuming imagePaths is a field of the class
                    imagePaths = new List<string>();  // Resetting or initializing the class field
                    bool isReadingPaths = false;

                    foreach (string line in lines)
                    {
                        if (line.StartsWith("ImagePaths:"))
                        {
                            isReadingPaths = true;
                            continue;
                        }

                        if (line.StartsWith("TimerIntervalSeconds:"))
                        {
                            isReadingPaths = false;
                            if (int.TryParse(line.Replace("TimerIntervalSeconds:", "").Trim(), out int parsedIntervalSeconds))
                            {
                                intervalSeconds = parsedIntervalSeconds;
                            }
                            continue;
                        }

                        if (isReadingPaths)
                        {
                            // Add image paths and print them for debugging
                            imagePaths.Add(line.Trim());
                            Console.WriteLine("Image path read from config: " + line.Trim()); // Print each path for debugging
                        }
                    }

                    // Rest of your code to use imagePaths and intervalSeconds
                }

                // Initialize the timer with the interval read from the config file
                imageSliderTimer = new Timer();
                imageSliderTimer.Interval = intervalSeconds * 1000; // Convert to milliseconds
                imageSliderTimer.Tick += ImageSliderTimer_Tick;
                imageSliderTimer.Start();

                // Now you can safely use imageSliderTimer and imagePaths
            }
            catch (Exception ex)
            {
                // Handle exceptions if any other error occurs
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

            if (imagePaths.Count > 0 && File.Exists(imagePaths[currentImageIndex]))
            {
                pictureBox1.Image = Image.FromFile(imagePaths[currentImageIndex]);
            }
            else
            {
                // Handle case where image is not found or list is empty
                // Consider setting a default image or providing a suitable message/notification
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
            if (configWatcher != null)
            {
                configWatcher.Dispose();
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

        private FileSystemWatcher configWatcher;

        private void SetupConfigFileWatcher()
        {
            string configFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "config.txt");

            configWatcher = new FileSystemWatcher
            {
                Path = Path.GetDirectoryName(configFilePath),
                Filter = Path.GetFileName(configFilePath),
                NotifyFilter = NotifyFilters.LastWrite
            };

            configWatcher.Changed += OnConfigFileChanged;
            configWatcher.EnableRaisingEvents = true;
        }

        private void OnConfigFileChanged(object source, FileSystemEventArgs e)
        {
            // Re-read the configuration
            ReadImagePathsFromConfig();

            // Consider using Invoke to update UI elements if needed
            this.Invoke((MethodInvoker)delegate
            {
                // Refresh UI based on new configuration, if needed
            });
        }


        private void TableLayoutPanel1_Paint(object sender, PaintEventArgs e)
        {
            tableLayoutPanel1.Controls.Clear();
            tableLayoutPanel1.ColumnCount = 3;

            int totalRows = dataTable.Rows.Count + 1;  // +1 for the header
            tableLayoutPanel1.RowCount = totalRows;

            Random random = new Random();
            decimal totalSub = 0; // Initialize the accumulator
            decimal netTotalValue = 0; // Initialize the accumulator

            // Function to create a new label with increased font size
            Label CreateLabelWithIncreasedFontSize(string text)
            {
                var label = new Label
                {
                    Text = text,    
                    TextAlign = ContentAlignment.MiddleCenter,
                    Dock = DockStyle.Fill
                };
                label.Font = new Font(label.Font.Name, label.Font.Size * 1.30f, label.Font.Style); // Increase font size by 10%
                return label;
            }

            // Adding Headers
            tableLayoutPanel1.Controls.Add(CreateLabelWithIncreasedFontSize("ItemDescription"));
            tableLayoutPanel1.Controls.Add(CreateLabelWithIncreasedFontSize("Price"));
            tableLayoutPanel1.Controls.Add(CreateLabelWithIncreasedFontSize("Quantity"));


            foreach (DataRow row in dataTable.Rows)
            {
                decimal price = Convert.ToDecimal(row["Price"]);
                string formattedPrice = price.ToString("F2");
                int quantity = random.Next(1, 5); // Note: Adjust the range if required
                totalSub += price * quantity; // Update the accumulator
                netTotalValue += price * quantity; // Update the accumulator

                // Adding row values with increased font size
                tableLayoutPanel1.Controls.Add(CreateLabelWithIncreasedFontSize(row["ItemDescription"].ToString()));
                tableLayoutPanel1.Controls.Add(CreateLabelWithIncreasedFontSize(formattedPrice));
                tableLayoutPanel1.Controls.Add(CreateLabelWithIncreasedFontSize(quantity.ToString()));
            }
            subTotal.Text = totalSub.ToString("N2");
            NetTotal.Text = netTotalValue.ToString("N2");
            decimal gstValue = netTotalValue * 0.06m; // Calculate 6% GST
            this.GST.Text = gstValue.ToString("N2");
            Balance.Text = netTotalValue.ToString("N2");            

            int rowHeight = 30;  // Assuming each row to be of 30 units height, you can adjust this based on your need

            // Set the height of the TableLayoutPanel based on the number of rows.
            if (totalRows > 20)
            {
                tableLayoutPanel1.Height = 18 * rowHeight;  // This would potentially activate the scrollbar in the Panel
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
        private void LogError(Exception ex)
        {
            string errorMessage = $"[{DateTime.Now}] - Error: {ex.Message}\nStackTrace: {ex.StackTrace}\n";
            File.AppendAllText(@"path\to\error_log.txt", errorMessage);
        }
    }
}
