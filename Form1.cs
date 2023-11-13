﻿using System;
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
            InitialFetchData();
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

        //====================//====================//====================//====================//====================//====================//====================//====================        
        private void InitialFetchData()
        {
            try
            {
                string connectionString2 = "Server=DESKTOP-JDQGAO5;Database=easypos;User Id=notifman;Password=root1234;";
                using (SqlConnection connection = new SqlConnection(connectionString2))
                {
                    connection.Open();
                    using (SqlCommand command = new SqlCommand("SELECT FormInformation, ActionInformation FROM dbo.SysAuditTrail", connection))
                    {
                        // Setup the SQL dependency
                        var dependency = new SqlDependency(command);
                        dependency.OnChange += new OnChangeEventHandler(OnDataChanged);

                        // Execute the command to establish the dependency
                        command.ExecuteReader();
                        /*                        DisplayToTable();
                        */
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
        private void FetchData()
        {
            try
            {
                string connectionString2 = "Server=DESKTOP-JDQGAO5;Database=easypos;User Id=notifman;Password=root1234;";
                using (SqlConnection connection = new SqlConnection(connectionString2))
                {
                    connection.Open();
                    using (SqlCommand command = new SqlCommand("SELECT FormInformation, ActionInformation FROM dbo.SysAuditTrail", connection))
                    {
                        // Setup the SQL dependency
                        var dependency = new SqlDependency(command);
                        dependency.OnChange += new OnChangeEventHandler(OnDataChanged);

                        // Execute the command to establish the dependency
                        command.ExecuteReader();
                        DisplayToTable();
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

        //====================//====================//====================//====================//====================//====================//====================//====================
        private void DisplayToTable()
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    using (SqlCommand command = new SqlCommand("SELECT TOP 1 FormInformation, RecordInformation, ActionInformation FROM dbo.SysAuditTrail ORDER BY Id DESC", connection))

                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        dataTable = new DataTable();
                        dataTable.Load(reader);

                        // Check the value of ActionInformation in the last row
                        if (dataTable.Rows.Count > 0)
                        {
                            string formInformation = dataTable.Rows[0]["FormInformation"].ToString();
                            string actionInformation = dataTable.Rows[0]["ActionInformation"].ToString();
                            string recordInformation = dataTable.Rows[0]["RecordInformation"].ToString();

                            if (actionInformation == "AddSales")
                            {
                                tableLayoutPanel1.Controls.Clear();

                                // Loop to add empty rows up to the 7th row
                                for (int row = 0; row < 8; row++)
                                {
                                    Label emptyLabel = CreateLabelWithIncreasedFontSize(" ", 14, true);
                                    tableLayoutPanel1.Controls.Add(emptyLabel, 1, row);
                                }

                                // Create a label with the specified text and font properties
                                Label label = CreateLabelWithIncreasedFontSize("Welcome back! Great to see you again!", 11, true);

                                // Add the label to the 8th row in the second column (index 1)
                                tableLayoutPanel1.Controls.Add(label, 1, 8);
                            }

                            else if (actionInformation == "AddSalesLine")
                            {
                                // Remove existing labels in the second column (index 1) and rows 0 to 7
                                for (int row = 0; row < 9; row++)
                                {
                                    foreach (Control control in tableLayoutPanel1.Controls)
                                    {
                                        if (tableLayoutPanel1.GetColumn(control) == 1 && tableLayoutPanel1.GetRow(control) == row)
                                        {
                                            tableLayoutPanel1.Controls.Remove(control);
                                            control.Dispose(); // Dispose of the removed control
                                            break; // Exit the loop after removing one control
                                        }
                                    }
                                }

                                // Rest of your code for AddSalesLine and UpdateSalesLine
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

                                        // Use the modified GetItemInfoById to retrieve both ItemDescription and Category
                                        (string itemDescription, string category) = GetItemInfoById(itemId);


                                        // Call TableLayoutPanel1_Paint and pass itemDescription, category, actualPrice, and quantity
                                        TableLayoutPanel1_Paint(null, null, itemDescription, category, actualPrice, quantity);
                                    }
                                    else
                                    {
                                        Console.WriteLine("Price is null. Skipping TableLayoutPanel1_Paint.");
                                    }
                                }
                            }
                            else if (actionInformation == "UpdateSalesLine")
                            {
                                JObject jsonObject = JsonConvert.DeserializeObject<JObject>(formInformation);
                                if (jsonObject != null)
                                {
                                    decimal? price = jsonObject.Value<decimal?>("Price");
                                    int quantity = jsonObject.Value<int>("Quantity");
                                    int itemId = jsonObject.Value<int>("ItemId");

                                    if (price.HasValue)
                                    {
                                        decimal actualPrice = price ?? 0m;
                                        (string itemDescription, string category) = GetItemInfoById(itemId);
                                        // Update the TableLayoutPanel instead of calling TableLayoutPanel1_Paint
                                        UpdateTableLayoutPanel(itemDescription, quantity, actualPrice);
                                    }
                                    else
                                    {
                                        Console.WriteLine("Price is null. Skipping TableLayoutPanel1_Paint.");
                                    }
                                }
                            }

                            else if (actionInformation == "DeleteSalesLine")
                            {
                                JObject jsonObject = JsonConvert.DeserializeObject<JObject>(recordInformation);
                                if (jsonObject != null)
                                {
                                    int itemId = jsonObject.Value<int>("ItemId");
                                    (string itemDescription, _) = GetItemInfoById(itemId);  // Assuming GetItemInfoById returns item description

                                    // Call method to delete the row from the TableLayoutPanel
                                    DeleteRowFromTableLayoutPanel(itemDescription);
                                }
                            }

                            else if (actionInformation == "DeleteSales")
                            {
                                tableLayoutPanel1.Controls.Clear();
                            }
                            else if (actionInformation == "TenderSales")
                            {
                                // Code for TenderSales
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
        //====================//====================//====================//====================//====================//====================//====================
        private void DeleteRowFromTableLayoutPanel(string itemDescription)
        {
            for (int row = 0; row < tableLayoutPanel1.RowCount; row++)
            {
                Label descriptionLabel = tableLayoutPanel1.GetControlFromPosition(1, row) as Label;

                if (descriptionLabel != null && descriptionLabel.Text == itemDescription)
                {
                    // Subtract the amount of this row from totalAmount
                    Label amountLabel = tableLayoutPanel1.GetControlFromPosition(3, row) as Label;
                    if (amountLabel != null && decimal.TryParse(amountLabel.Text, out decimal amount))
                    {
                        totalAmount -= amount;
                        Total.Text = $"Total: {totalAmount.ToString("F2")}";
                    }

                    // Remove all controls in the row
                    for (int column = 0; column < tableLayoutPanel1.ColumnCount; column++)
                    {
                        var control = tableLayoutPanel1.GetControlFromPosition(column, row);
                        if (control != null)
                        {
                            tableLayoutPanel1.Controls.Remove(control);
                            control.Dispose();
                        }
                    }

                    // Shift all rows above one step down
                    for (int i = row + 1; i < tableLayoutPanel1.RowCount; i++)
                    {
                        for (int column = 0; column < tableLayoutPanel1.ColumnCount; column++)
                        {
                            var control = tableLayoutPanel1.GetControlFromPosition(column, i);
                            if (control != null)
                            {
                                tableLayoutPanel1.SetRow(control, i - 1);
                            }
                        }
                    }

                    // Remove the last row
                    tableLayoutPanel1.RowCount--;

                    break; // Row deleted, exit the loop
                }
            }

            // Refresh the display
            tableLayoutPanel1.Invalidate();
        }


        private void UpdateTotalAmount()
        {
            totalAmount = 0;
            // Recalculate the total amount based on the remaining rows
            for (int row = 0; row < tableLayoutPanel1.RowCount; row++)
            {
                Label amountLabel = tableLayoutPanel1.GetControlFromPosition(3, row) as Label;
                if (amountLabel != null && decimal.TryParse(amountLabel.Text, out decimal amount))
                {
                    totalAmount += amount;
                }
            }
            Total.Text = $"Total: {totalAmount.ToString("F2")}";
        }

        private void UpdateTableLayoutPanel(string itemDescription, int quantity, decimal price)
        {
            // Iterate through each row in the tableLayoutPanel
            for (int row = 0; row < tableLayoutPanel1.RowCount; row++)
            {
                // Assuming the second cell (index 1) of each row contains the itemDescription Label
                Label descriptionLabel = tableLayoutPanel1.GetControlFromPosition(1, row) as Label;

                // Check if this row corresponds to the itemDescription
                if (descriptionLabel != null && descriptionLabel.Text == itemDescription)
                {
                    // Get the old amount label (assuming it's in the fourth cell, index 3)
                    Label amountLabel = tableLayoutPanel1.GetControlFromPosition(3, row) as Label;
                    decimal oldAmount = 0m;
                    if (amountLabel != null && decimal.TryParse(amountLabel.Text, out oldAmount))
                    {
                        // Subtract the old amount from the total
                        totalAmount -= oldAmount;
                    }

                    // Update the quantity label (assuming it's in the first cell, index 0)
                    Label quantityLabel = tableLayoutPanel1.GetControlFromPosition(0, row) as Label;
                    if (quantityLabel != null)
                    {
                        quantityLabel.Text = quantity.ToString();
                    }

                    // Calculate the new amount and update the amount label
                    decimal newAmount = quantity * price;
                    if (amountLabel != null)
                    {
                        amountLabel.Text = newAmount.ToString("F2");
                    }

                    // Add the new amount to the total
                    totalAmount += newAmount;
                    Total.Text = $"Total: {totalAmount.ToString("F2")}";

                    break; // Break the loop as we have found and updated the row
                }
            }

            // Refresh the display
            tableLayoutPanel1.Invalidate();
        }



        private decimal totalAmount = 0; // Class-level variable to keep track of the total amount

        private void TableLayoutPanel1_Paint(object sender, EventArgs e, string itemDescription, string category, decimal price, int quantity)
        {
            // Set the column style for the fourth column (index 3) for right alignment
            if (tableLayoutPanel1.ColumnStyles.Count < 4)
            {
                // Adding extra columns if they do not exist
                while (tableLayoutPanel1.ColumnStyles.Count < 4)
                {
                    tableLayoutPanel1.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));
                }
            }
            tableLayoutPanel1.ColumnStyles[3] = new ColumnStyle(SizeType.AutoSize);

            // Ensure the row count is sufficient
            tableLayoutPanel1.RowCount = tableLayoutPanel1.RowCount + 1;
            int totalRows = tableLayoutPanel1.RowCount - 1;

            // Function to create a new label with increased font size
            Label CreateLabelWithIncreasedFontSize(string text, float scaleFactor = 1.30f, AnchorStyles anchor = AnchorStyles.Left | AnchorStyles.Top)
            {
                var label = new Label
                {
                    Text = text,
                    Dock = DockStyle.None,
                    Anchor = anchor,
                    AutoSize = true
                };
                label.Font = new Font(Label.DefaultFont.Name, Label.DefaultFont.Size * scaleFactor, Label.DefaultFont.Style);
                return label;
            }

            Label CreateLabelWithIncreasedFontSize2(string text, float scaleFactor = 1.00f, AnchorStyles anchor = AnchorStyles.Left | AnchorStyles.Top)
            {
                var label = new Label
                {
                    Text = text,
                    Dock = DockStyle.None,
                    Anchor = anchor,
                    AutoSize = true
                };
                label.Font = new Font(Label.DefaultFont.Name, Label.DefaultFont.Size * scaleFactor, Label.DefaultFont.Style);
                return label;
            }

            if ((category == "ADD-ONS" || category == "OTHERS") && price != 0)
            {
                // Create a label for itemId and add it to the first column
                tableLayoutPanel1.Controls.Add(CreateLabelWithIncreasedFontSize(quantity.ToString()), 0, totalRows);
                tableLayoutPanel1.Controls.Add(CreateLabelWithIncreasedFontSize2(itemDescription), 1, totalRows);
                decimal amount = quantity * price; // Calculate the amount
                tableLayoutPanel1.Controls.Add(CreateLabelWithIncreasedFontSize(amount.ToString("F2"), 1.30f, AnchorStyles.Top | AnchorStyles.Right), 3, totalRows);
                tableLayoutPanel1.Controls.Add(CreateLabelWithIncreasedFontSize($"@{price.ToString("F2")} /pc"), 2, totalRows);
                totalAmount += amount; // Add the amount to the total
            }
            else if (price != 0)
            {
                tableLayoutPanel1.Controls.Add(CreateLabelWithIncreasedFontSize(quantity.ToString()), 0, totalRows);
                tableLayoutPanel1.Controls.Add(CreateLabelWithIncreasedFontSize(itemDescription), 1, totalRows);
                decimal amount = quantity * price; // Calculate the amount
                tableLayoutPanel1.Controls.Add(CreateLabelWithIncreasedFontSize(amount.ToString("F2"), 1.30f, AnchorStyles.Top | AnchorStyles.Right), 3, totalRows);
                tableLayoutPanel1.Controls.Add(CreateLabelWithIncreasedFontSize($"@{price.ToString("F2")} /pc"), 2, totalRows);
                totalAmount += amount; // Add the amount to the total
            }
            Total.Text = $"Total: {totalAmount.ToString("F2")}";
        }

        private Label CreateLabelWithIncreasedFontSize(string text, int fontSize, bool bold)
        {
            var label = new Label
            {
                Text = text,
                TextAlign = ContentAlignment.MiddleCenter,
                Dock = DockStyle.Fill
            };

            // Set the font properties
            label.Font = new Font(label.Font.FontFamily, fontSize, bold ? FontStyle.Bold : FontStyle.Regular);

            return label;

        }
        // Function to retrieve ItemDescription from MstItem table based on ItemId
        private (string ItemDescription, string Category) GetItemInfoById(int itemId)
        {
            string itemDescription = ""; // Initialize with an empty string
            string category = ""; // Initialize with an empty string

            // Construct and execute a SQL query to retrieve the ItemDescription and Category based on ItemId
            string query = "SELECT ItemDescription, Category FROM MstItem WHERE Id = @ItemId";
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@ItemId", itemId);
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            itemDescription = reader["ItemDescription"].ToString();
                            category = reader["Category"].ToString();
                        }
                    }
                }
            }

            return (itemDescription, category);
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
                /*                MessageBox.Show("Changes were made to the SysAuditTrail table.");
                */
            }

            // Check if the change type is an insert, update, or delete.
            if (e.Info == SqlNotificationInfo.Insert || e.Info == SqlNotificationInfo.Update || e.Info == SqlNotificationInfo.Delete)
            {
                /*                MessageBox.Show("Changes were made to the SysAuditTrail table.");
                */
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

        private void Balance_TextChanged(object sender, EventArgs e)
        {

        }

        private void textBox6_TextChanged(object sender, EventArgs e)
        {

        }
    }
}