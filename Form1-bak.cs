using System;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Windows.Forms;
using System.IO;
using Newtonsoft.Json.Linq;
using System.Diagnostics;

namespace _2ndMonitor
{
    public partial class Form1 : Form
    {
        private DataTable dataTable;
        private Timer imageSliderTimer;
        private List<string> imagePaths;  // List of paths to your images
        private int currentImageIndex = 0;
        private string connectionString = "Server=DESKTOP-JDQGAO5;Database=easypos;User Id=notifman;Password=root1234;";

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
        private void SetupSqlDependency()
        {
            // Start the SqlDependency listener.
            string connectionString2 = "Server=DESKTOP-JDQGAO5;Database=easypos;User Id=notifman;Password=root1234;";

            SqlDependency.Start(connectionString2);
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

            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    using (SqlCommand command = new SqlCommand("SELECT TOP 1 FormInformation, ActionInformation FROM dbo.SysAuditTrail ORDER BY Id DESC", connection))
                    {
                        // Replace YourPrimaryKeyColumn with the actual primary key column of your table.
                        // This assumes that the primary key column can be used to determine the order of rows.

                        // Setup the SQL dependency
                        var dependency = new SqlDependency(command);
                        dependency.OnChange += new OnChangeEventHandler(OnDataChanged);

                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            dataTable = new DataTable();
                            dataTable.Load(reader);

                            // Check the value of ActionInformation in the last row
                            if (dataTable.Rows.Count > 0)
                            {
                                string formInformation = dataTable.Rows[0]["FormInformation"].ToString();
                                string actionInformation = dataTable.Rows[0]["ActionInformation"].ToString();
                                Console.WriteLine("Action Information: " + actionInformation); // Print to the console
                                Console.WriteLine("Form Information: " + formInformation); // Print to the console

                                if (actionInformation == "AddSales" || actionInformation == "AddSalesLine" || actionInformation == "UpdateSalesLine")/* || actionInformation == "DeleteSalesLine")*/
                                {
                                    // Parse the FormInformation as JSON
                                    JObject jsonObject = JsonConvert.DeserializeObject<JObject>(formInformation);

                                    if (jsonObject != null)
                                    {
                                        // Extract the "Price" property from the JSON data
                                        decimal? price = jsonObject.Value<decimal?>("Price");
                                        int quantity = jsonObject.Value<int>("Quantity");
                                        int itemId = jsonObject.Value<int>("ItemId");

                                        // Check if "Price" is null
                                        if (price.HasValue)
                                        {
                                            decimal actualPrice = price ?? 0m;

                                            // Now you have the "Price," "Quantity," and "ItemId" properties from the JSON data.
                                            Console.WriteLine("Price: " + actualPrice);
                                            Console.WriteLine("Quantity: " + quantity);
                                            Console.WriteLine("ItemId: " + itemId);
                                            // You can use itemId to search for ItemDescription in the MstItem table.
                                            string itemDescription = GetItemDescriptionById(itemId);
                                            Console.WriteLine("Item Description: " + itemDescription);

                                            // Call TableLayoutPanel1_Paint if "Price" is not null
                                            // Call TableLayoutPanel1_Paint and pass itemDescription, price, and quantity
                                            TableLayoutPanel1_Paint(null, null, itemDescription, actualPrice, quantity);
                                        }
                                        else
                                        {
                                            Console.WriteLine("Price is null. Skipping TableLayoutPanel1_Paint.");
                                        }
                                    }
                                }

                                }
                            }
                    }
                }
            }
            catch (SqlException ex)
            {
                Console.WriteLine($"Error in database operation: {ex.Message}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"General error: {ex.Message}");
            }
        }

        // Function to retrieve ItemDescription from MstItem table based on ItemId
        private string GetItemDescriptionById(int itemId)
        {
            string itemDescription = ""; // Initialize with an empty string

            // Construct and execute a SQL query to retrieve the ItemDescription based on ItemId
            string query = "SELECT ItemDescription FROM MstItem WHERE Id = @ItemId";
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@ItemId", itemId);
                    object result = command.ExecuteScalar();
                    if (result != null)
                    {
                        itemDescription = result.ToString();
                    }
                }
            }

            return itemDescription;
        }

        private void TableLayoutPanel1_Paint(object sender, PaintEventArgs e, string itemDescription, decimal price, int quantity)
        {
            tableLayoutPanel1.Controls.Clear();
            tableLayoutPanel1.ColumnCount = 3;

            int totalRows = 1;  // Since you are displaying a single item, set totalRows to 1
            tableLayoutPanel1.RowCount = totalRows;

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
            tableLayoutPanel1.Controls.Add(CreateLabelWithIncreasedFontSize("Item Name"));
            tableLayoutPanel1.Controls.Add(CreateLabelWithIncreasedFontSize("Quantity"));
            tableLayoutPanel1.Controls.Add(CreateLabelWithIncreasedFontSize("Price"));

            // Use the parameters itemDescription, price, and quantity that were passed to this method
            Console.WriteLine("Price: " + price);
            Console.WriteLine("Quantity: " + quantity);
            Console.WriteLine("Item Description: " + itemDescription);

            totalSub += price * quantity; // Update the accumulator
            netTotalValue += price * quantity; // Update the accumulator

            // Adding row values with increased font size
            tableLayoutPanel1.Controls.Add(CreateLabelWithIncreasedFontSize(itemDescription));
            tableLayoutPanel1.Controls.Add(CreateLabelWithIncreasedFontSize(quantity.ToString()));
            tableLayoutPanel1.Controls.Add(CreateLabelWithIncreasedFontSize(price.ToString("F2")));

            subTotal.Text = totalSub.ToString("N2");
            NetTotal.Text = netTotalValue.ToString("N2");
            decimal gstValue = netTotalValue * 0.06m; // Calculate 6% GST
            GST.Text = gstValue.ToString("N2");
            Balance.Text = (netTotalValue + gstValue).ToString("N2");
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
            string connectionString2 = "Server=DESKTOP-JDQGAO5;Database=easypos;User Id=notifman;Password=root1234;";
            SqlDependency.Stop(connectionString2);

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

            if (e.Type == SqlNotificationType.Change)
            {
                // Changes were made to the SysAuditTrail table
                MessageBox.Show("Changes were made to the SysAuditTrail table.");

            }

            // Check if the change type is an insert, update, or delete.
            if (e.Info == SqlNotificationInfo.Insert || e.Info == SqlNotificationInfo.Update || e.Info == SqlNotificationInfo.Delete)
            {
                MessageBox.Show("Changes were made to the SysAuditTrail table.");

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

        protected override void OnClosed(EventArgs e)
        {
            base.OnClosed(e);

            // Stop the SqlDependency when the form closes
            SqlDependency.Stop(connectionString);
        }
        private void LogError(Exception ex)
        {
            string errorMessage = $"[{DateTime.Now}] - Error: {ex.Message}\nStackTrace: {ex.StackTrace}\n";
            File.AppendAllText(@"path\to\error_log.txt", errorMessage);
        }
    }
}
