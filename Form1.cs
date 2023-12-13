using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Drawing.Imaging;
using System.Windows.Forms;
using System.IO;
using Newtonsoft.Json.Linq;
using ImageSettingsGUI;
using System.Text;
using Microsoft.AspNetCore.SignalR.Client;

namespace _2ndMonitor
{
    public partial class Form1 : Form
    {
        private DataTable dataTable;
        private Timer imageSliderTimer;
        private List<string> imagePaths;  // List of paths to your images
        private int currentImageIndex = 0;
        private string connectionString = "Server=localhost;Database=easypos;User Id=notifman;Password=root1234;";

        /*        private string connectionString = "Server=DESKTOP-JDQGAO5\\SQL2008;Database=easypos;User Id=notifman;Password=root1234;";
        */
        /*        private string connectionString = "Server=DESKTOP-J5EHGKE\\SQLEXPRESS;Database=easypos;User Id=notifman;Password=root1234;";
        */
        /*        private string connectionString = "Server=DESKTOP-J5EHGKE\\SQL2008;Database=easypos;User Id=notifman;Password=root1234;";
        */
/*        private string connectionString = "Server=localhost;Database=easypos;User Id=notifman;Password=root1234;";
*/
/*        private string connectionString = "Server=localhost;Database=easypos;User Id=sa;Password=easyfis;";
*/

        public Form1()
        {
            CheckConnection();
            InitializeComponent();
            CheckPermission();
            CheckServiceBroker();
            this.Load += new EventHandler(Form1_Load);
            ShowImageSettingsForm();
            InitializeImageSlider();
            SetupSqlDependency();
            InitialFetchData();
        }

        private void InitializeImageSlider()
        {
            ReadImagePathsFromConfig();
            imageSliderTimer = new Timer();
            imageSliderTimer.Tick += ImageSliderTimer_Tick;
            SetTimerIntervalFromConfig(); // This function should set imageSliderTimer.Interval
            imageSliderTimer.Start();
            CheckImage();
        }

        private void CheckServiceBroker()
        {
            // Check if Service Broker is enabled
            bool serviceBrokerEnabled = IsServiceBrokerEnabled();
            if (!serviceBrokerEnabled)
            {
                Console.WriteLine(serviceBrokerEnabled);
                MessageBox.Show("Service Broker is not enabled. The application will now exit.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                Environment.Exit(0);
                return; // Ensures that the rest of the constructor code is not executed
            }
            else
            {
                Console.WriteLine("Service Broker is enabled.", serviceBrokerEnabled);
            }
        }
        private void CheckImage()
        {
            try
            {
                string appDirectory = AppDomain.CurrentDomain.BaseDirectory;
                string[] imageFiles = Directory.GetFiles(appDirectory, "*.png"); // You can add more formats if needed

                if (imageFiles.Length == 0)
                {
                    // If no images in the application directory, try the working directory
                    string workingDirectory = Environment.CurrentDirectory;
                    imageFiles = Directory.GetFiles(workingDirectory, "*.png"); // Search for PNG images in the working directory

                    if (imageFiles.Length == 0)
                    {
                        // If no images in the working directory either, throw an exception
                        throw new FileNotFoundException("No PNG images found in the application or working directory.");
                    }
                }

                pictureBox1.Image = Image.FromFile(imageFiles[0]);
            }
            catch (FileNotFoundException fnfEx)
            {
                LogError("File not found: " + fnfEx.Message);
                ShowFallbackImage();
            }
            catch (Exception ex)
            {
                LogError("Unexpected error: " + ex.Message);
                ShowFallbackImage();
            }
        }

        private void LogError(string message)
        {
            // Implement logging logic here (e.g., write to a file or console)
            Console.WriteLine(message); // Example: logging to console
        }

        private void ShowFallbackImage()
        {
            int width = 800;
            int height = 600;
            using (Bitmap bmp = new Bitmap(width, height))
            {
                using (Graphics g = Graphics.FromImage(bmp))
                {
                    g.DrawLine(Pens.Red, 0, 0, width, height);
                    g.DrawString("No Image Found", new Font("Arial", 40), Brushes.Blue, new PointF(200, 200));
                }
                pictureBox1.Image = bmp; // Display the fallback image in the picture box
            }
        }

        private void CheckPermission()
        {
            // Make sure client has permissions 
            try
            {
                SqlClientPermission perm = new SqlClientPermission(System.Security.Permissions.PermissionState.Unrestricted);
                perm.Demand();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Permission error: " + ex.Message, "Permission Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                throw new ApplicationException("No permission");
            }
        }
        private void CheckConnection()
        {

            // Attempt to open a connection to the database
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open(); // Try to open the connection
                                 // If successful, the connection will be closed when exiting the using block
                }

                // Other code that should run after successful connection...
            }
            catch (SqlException ex)
            {
                // Log the error details for more in-depth analysis
                var errorMessages = new StringBuilder();
                for (int i = 0; i < ex.Errors.Count; i++)
                {
                    errorMessages.AppendLine($"Index #{i}\n" +
                                             $"Message: {ex.Errors[i].Message}\n" +
                                             $"LineNumber: {ex.Errors[i].LineNumber}\n" +
                                             $"Source: {ex.Errors[i].Source}\n" +
                                             $"Procedure: {ex.Errors[i].Procedure}\n");
                }
                MessageBox.Show(errorMessages.ToString(), "SQL Exception Details", MessageBoxButtons.OK, MessageBoxIcon.Error);
                Environment.Exit(1); // Exit the application if the connection is not successful
            }

            catch (Exception ex)
            {
                MessageBox.Show($"An unexpected error occurred: {ex.Message}", "General Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                // Handle non-SQL exceptions if necessary or log them
                // You may decide not to exit the application for non-SQL exceptions depending on the context
            }
        }

        private async void Form1_Load(object sender, EventArgs e)
        {

            // Initialize the SignalR connection
            var hubConnection = new HubConnectionBuilder()
                .WithUrl("http://localhost:55430/myhub")
                .Build();

            hubConnection.On<string, string>("ReceiveMessage", (user, hubmessage) =>
            {
                // Handle the message received from the hub
                // Make sure to marshal back to the UI thread if updating the UI
                this.Invoke((Action)(() =>
                {
                    // Update your UI here
                }));
            });

            try
            {
                await hubConnection.StartAsync();
                // SignalR connection successfully started
            }
            catch (Exception ex)
            {
                // Handle exceptions (e.g., log them or show a message to the user)
                MessageBox.Show("Could not connect to the SignalR hub: " + ex.Message);
                return;
            }

            // Read the configuration file
            bool enableSecondMonitorFeature = ReadConfig();

            // Get the screens available
            Screen[] screens = Screen.AllScreens;

            // Create a message to display the information
            string message = "enableSecondMonitorFeature is set to: " + enableSecondMonitorFeature + "\n";

            if (screens.Length > 0)
            {
                message += "Number of screens detected: " + screens.Length + "\n";

                foreach (Screen screen in screens)
                {
                    message += "Screen " + screen.DeviceName + ": " + screen.Bounds.Width + "x" + screen.Bounds.Height + "\n";
                }
            }
            else
            {
                message += "No screens detected.";
            }

            // Show the message box with the information
            // MessageBox.Show(message);

            // Check if the second monitor feature is enabled and if there is a second monitor
            if (enableSecondMonitorFeature && screens.Length > 1)
            {
                Console.WriteLine("A second monitor is detected.");
                Rectangle secondScreenBounds = screens[1].Bounds;
                this.StartPosition = FormStartPosition.Manual;
                this.Location = new Point(secondScreenBounds.Left, secondScreenBounds.Top);
                this.Size = new System.Drawing.Size(1366, 768);

                // Check if "EasyPOS" is running
                if (IsEasyPOSRunning())
                {
                    // Move the "EasyPOS" window to the second screen (if it exists)
                    MoveEasyPOSWindowToSecondScreen();
                }
                else
                {
                    // Can't Move the "EasyPOS" window to the second screen (coz it doesn't exists)
                    MessageBox.Show("EasyPOS GUI was not found running.");
                }
            }
            else
            {
                // Check if "EasyPOS" is running
                if (IsEasyPOSRunning())
                {
                    // Move the "EasyPOS" window to the second screen (if it exists)
                    MoveEasyPOSWindowToSecondScreen();
                }
                else
                {
                    // Can't Move the "EasyPOS" window to the second screen (coz it doesn't exists)
                    MessageBox.Show("EasyPOS GUI was not found running.");
                }
                Rectangle secondScreenBounds = screens[0].Bounds;
                this.StartPosition = FormStartPosition.Manual;
                this.Location = new Point(secondScreenBounds.Left, secondScreenBounds.Top);
                this.Size = new System.Drawing.Size(1366, 768);
                Console.WriteLine("Second monitor feature is disabled or only one monitor detected.");
            }
        }

        private bool IsEasyPOSRunning()
        {
            string processName = "EasyPOS"; // Change this to the actual process name of EasyPOS
            Process[] processes = Process.GetProcessesByName(processName);
            return processes.Length > 0;
        }

        private void MoveEasyPOSWindowToSecondScreen()
        {
            string processName = "EasyPOS"; // Change this to the actual process name of EasyPOS
            Process[] processes = Process.GetProcessesByName(processName);

            if (processes.Length > 0)
            {
                IntPtr handle = processes[0].MainWindowHandle;
                Screen[] screens = Screen.AllScreens;

                if (screens.Length > 1)
                {
                    Rectangle secondScreenBounds = screens[1].Bounds;

                    // Check if the window is maximized
                    if (IsMaximized(handle))
                    {
                        // Restore the window to a normal state
                        ShowWindow(handle, SW_RESTORE);
                    }

                    // Move the window to the second screen
                    MoveWindow(handle, secondScreenBounds.Left, secondScreenBounds.Top, secondScreenBounds.Width, secondScreenBounds.Height, true);

                    // Maximize the window again (if it was maximized)
                    if (IsMaximized(handle))
                    {
                        ShowWindow(handle, SW_MAXIMIZE);
                    }
                }
            }
        }

        // Constants for ShowWindow method
        private const int SW_RESTORE = 9;
        private const int SW_MAXIMIZE = 3;

        // P/Invoke methods
        [DllImport("user32.dll")]
        public static extern bool MoveWindow(IntPtr hWnd, int X, int Y, int nWidth, int nHeight, bool bRepaint);

        [DllImport("user32.dll")]
        public static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);

        // Function to check if a window is maximized
        private bool IsMaximized(IntPtr hWnd)
        {
            WINDOWPLACEMENT placement = new WINDOWPLACEMENT();
            placement.length = Marshal.SizeOf(placement);
            GetWindowPlacement(hWnd, ref placement);
            return placement.showCmd == SW_MAXIMIZE;
        }

        // P/Invoke method to get window placement information
        [DllImport("user32.dll")]
        public static extern bool GetWindowPlacement(IntPtr hWnd, ref WINDOWPLACEMENT lpwndpl);

        // Structure for WINDOWPLACEMENT
        [Serializable]
        [StructLayout(LayoutKind.Sequential)]
        public struct WINDOWPLACEMENT
        {
            public int length;
            public int flags;
            public int showCmd;
            public Point ptMinPosition;
            public Point ptMaxPosition;
            public Rectangle rcNormalPosition;
        }

        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            if (keyData == Keys.F12)
            {
                ShowImageSettingsForm();
                return true; // Indicate that you've handled this key
            }

            return base.ProcessCmdKey(ref msg, keyData);
        }

        private void ShowImageSettingsForm()
        {
            ImageSettingsForm settingsForm = new ImageSettingsForm();
            settingsForm.ShowDialog(); // or use .Show() for a non-modal form
        }

        private void SetupSqlDependency()
        {
            // Start the SqlDependency listener.
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
                MessageBox.Show("Image is not found or list is empty.", "Error2", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        //====================//====================//====================//====================//====================//====================//====================//====================        
        private void InitialFetchData()
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    using (SqlCommand command = new SqlCommand("SELECT FormInformation, ActionInformation FROM dbo.SysAuditTrail", connection))
                    {
                        var dependency = new SqlDependency(command);
                        dependency.OnChange += new OnChangeEventHandler(OnDataChanged);
                        command.ExecuteReader();
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
                using (SqlConnection connection = new SqlConnection(connectionString))
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

                            if (actionInformation == "AddSales" || recordInformation == "AddSales")
                            {
                                totalAmount = 0; // Reset the totalAmount
                                Total.Text = $"Total: {totalAmount.ToString("F2")}"; // Update the Total label
                                Paid.Text = "0.00"; // Update the Paid label to "0.00"
                                Change.Text = "0.00"; // Update the Paid label to "0.00"
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

                            else if (actionInformation == "AddSalesLine" || recordInformation == "AddSalesLine")
                            {
                                MessageBox.Show("AddSalesLine");
                                // Remove existing labels in the second column (index 1) and rows 0 to 7
                                for (int row = 0; row < 9; row++)
                                {
                                    foreach (Control control in tableLayoutPanel1.Controls)
                                    {
                                        if (tableLayoutPanel1.GetColumn(control) == 1 && tableLayoutPanel1.GetRow(control) == row)
                                        {
                                            tableLayoutPanel1.Controls.Remove(control);
                                            MessageBox.Show("AddSalesLine2");
                                            control.Dispose(); // Dispose of the removed control
                                            break; // Exit the loop after removing one control
                                        }
                                    }
                                }

                                // Rest of your code for AddSalesLine and UpdateSalesLine
                                JObject jsonObject = JsonConvert.DeserializeObject<JObject>(formInformation);
                                if (jsonObject != null)
                                {
                                    int quantity = jsonObject.Value<int>("Quantity");
                                    int itemId = jsonObject.Value<int>("ItemId");
                                    int salesId = jsonObject.Value<int>("SalesId");
                                    // Fetch the price from the TrnSales table using SalesId
                                    decimal? price = GetAmountFromSalesId(salesId);
                                    MessageBox.Show("Error reading config.txt: " + price);

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
                            else if (actionInformation == "UpdateSalesLine" || recordInformation == "UpdateSalesLine")
                            {
                                MessageBox.Show("UpdateSalesLine");
                                JObject jsonObject = JsonConvert.DeserializeObject<JObject>(formInformation);
                                if (jsonObject != null)
                                {
                                    int quantity = jsonObject.Value<int>("Quantity");
                                    int itemId = jsonObject.Value<int>("ItemId");
                                    int salesId = jsonObject.Value<int>("SalesId");
                                    // Fetch the price from the TrnSales table using SalesId
                                    decimal? price = GetAmountFromSalesId(salesId);
                                    MessageBox.Show("Error reading config.txt: " + price);

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

                            else if (actionInformation == "DeleteSalesLine" || recordInformation == "DeleteSalesLine")
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

                            else if (actionInformation == "DeleteSales" || recordInformation == "DeleteSalesLine")
                            {
                                totalAmount = 0; // Reset the totalAmount
                                Total.Text = $"Total: {totalAmount.ToString("F2")}"; // Update the Total label
                                tableLayoutPanel1.Controls.Clear();
                            }
                            else if (actionInformation == "TenderSales" || recordInformation == "TenderSales")
                            {
                                JObject jsonObject = JsonConvert.DeserializeObject<JObject>(formInformation);
                                if (jsonObject != null)
                                {
                                    // Extract the "TenderAmount" property from the JSON data
                                    decimal? tenderAmount = jsonObject.Value<decimal?>("TenderAmount");
                                    decimal? ChangeAmount = jsonObject.Value<decimal?>("ChangeAmount");
                                    Paid.Text = tenderAmount.Value.ToString("0.00");
                                    Change.Text = ChangeAmount.HasValue ? ChangeAmount.Value.ToString("0.00") : "0.00";
                                    Console.WriteLine(tenderAmount);
                                    Console.WriteLine(ChangeAmount);
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

        //====================//====================//====================//====================//====================//====================//====================

        // Method to get the Amount from the TrnSales table using SalesId
        private decimal? GetAmountFromSalesId(int salesId)
        {
            decimal? amount = null;

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                // Open the connection
                connection.Open();

                // Define the query
                string query = $"SELECT Amount FROM TrnSales WHERE SalesId = @SalesId";

                // Create the command
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    // Use parameters to prevent SQL injection
                    command.Parameters.AddWithValue("@SalesId", salesId);

                    // Execute the command and process the results
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        if (reader.Read()) // If there are rows
                        {
                            amount = reader.IsDBNull(reader.GetOrdinal("Amount")) ? (decimal?)null : reader.GetDecimal(reader.GetOrdinal("Amount"));
                        }
                    }
                }
            }

            return amount;
        }



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

                        if (isReadingPaths && !string.IsNullOrWhiteSpace(line.Trim()))
                        {
                            // Add image paths and print them for debugging
                            imagePaths.Add(line.Trim());
                            Console.WriteLine("Image path read from config: " + line.Trim()); // Print each path for debugging
                        }
                    }
                }

                // If no image paths were added, use 'test.png' from the working directory
                if (imagePaths.Count == 0)
                {
                    string workingDirectory = Environment.CurrentDirectory;
                    string defaultImagePath = Path.Combine(workingDirectory, "test.png");
                    if (File.Exists(defaultImagePath))
                    {
                        imagePaths.Add(defaultImagePath);
                    }
                    else
                    {
                        throw new FileNotFoundException("Default image 'test.png' not found in the working directory.");
                    }
                }

                // Initialize and start the timer with the interval read from the config file
                imageSliderTimer = new Timer
                {
                    Interval = intervalSeconds * 1000 // Convert to milliseconds
                };
                imageSliderTimer.Tick += ImageSliderTimer_Tick;
                imageSliderTimer.Start();
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

        private bool ReadConfig()
        {
            try
            {
                string configText = File.ReadAllText("config.txt");
                // Assuming the config file contains a line like "EnableSecondMonitor=true"
                var configLines = configText.Split(new[] { Environment.NewLine }, StringSplitOptions.None);
                foreach (var line in configLines)
                {
                    if (line.StartsWith("EnableSecondMonitor="))
                    {
                        return bool.Parse(line.Split('=')[1]);                        
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error reading config file: " + ex.Message);
            }
            return false; // Default to false if the setting is not found or any error occurs
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

        private bool IsServiceBrokerEnabled()
        {
            string commandText = "SELECT is_broker_enabled FROM sys.databases WHERE name = 'easypos';";
            bool isBrokerEnabled = false;

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                SqlCommand command = new SqlCommand(commandText, connection);

                try
                {
                    connection.Open();
                    object result = command.ExecuteScalar();

                    if (result != null)
                    {
                        isBrokerEnabled = Convert.ToBoolean(result);
                        Console.WriteLine("Service Broker enabled status: " + isBrokerEnabled);
                    }
                    else
                    {
                        Console.WriteLine("Service Broker status check returned no results.");
                    }
                }
                catch (SqlException ex)
                {
                    Console.WriteLine("SQL Error: " + ex.Message);
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Error: " + ex.Message);
                }
            }

            return isBrokerEnabled;
        }

        private void Total_TextChanged(object sender, EventArgs e)
        {

        }
    }
}

namespace ImageSettingsGUI
{
    public class ImageSettingsForm : Form
    {
        private ListBox imagePathsListBox;
        private Button addButton;
        private Button removeButton;
        private NumericUpDown timerIntervalNumericUpDown;
        private Button saveButton;
        private OpenFileDialog openFileDialog;
        private TextBox serverTextBox;
        private TextBox databaseTextBox;
        private TextBox userIdTextBox;
        private TextBox passwordTextBox;

        public ImageSettingsForm()
        {
            InitializeComponents();
            LoadConfig();
        }

        private void InitializeComponents()
        {
            // List box for displaying image paths
            imagePathsListBox = new ListBox { Left = 20, Top = 20, Width = 200, Height = 200 };
            Controls.Add(imagePathsListBox);

            // Add button to add image paths
            addButton = new Button { Text = "Add", Left = 230, Width = 100, Top = 20 };
            addButton.Click += AddButton_Click;
            Controls.Add(addButton);

            // Remove button to remove selected image path
            removeButton = new Button { Text = "Remove", Left = 230, Width = 100, Top = 50 };
            removeButton.Click += RemoveButton_Click;
            Controls.Add(removeButton);

            // Numeric up-down for timer interval
            timerIntervalNumericUpDown = new NumericUpDown { Left = 20, Top = 230, Width = 100 };
            timerIntervalNumericUpDown.Minimum = 1;
            timerIntervalNumericUpDown.Maximum = 60; // example range from 1 to 60 seconds
            Controls.Add(timerIntervalNumericUpDown);

            // Save button to save settings
            saveButton = new Button { Text = "Save", Left = 230, Width = 100, Top = 80 };
            saveButton.Click += SaveButton_Click;
            Controls.Add(saveButton);

            // Adjust the size of the form to accommodate new fields
            Size = new System.Drawing.Size(350, 300);
            // File dialog for selecting images
            openFileDialog = new OpenFileDialog { Multiselect = true, Filter = "Image files (*.png;*.jpeg;*.jpg)|*.png;*.jpeg;*.jpg" };

            // Form settings
            Text = "Image Settings";
        }

        private void LoadConfig()
        {
            string configFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "config.txt");
            if (File.Exists(configFilePath))
            {
                string[] lines = File.ReadAllLines(configFilePath);
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
                        if (int.TryParse(line.Replace("TimerIntervalSeconds:", "").Trim(), out int intervalSeconds))
                        {
                            timerIntervalNumericUpDown.Value = intervalSeconds;
                        }
                        continue;
                    }

                    if (isReadingPaths)
                    {
                        imagePathsListBox.Items.Add(line.Trim());
                    }
                }
            }
            else
            {
                MessageBox.Show("Config file not found.");
            }
        }

        private void AddButton_Click(object sender, EventArgs e)
        {
            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                foreach (var file in openFileDialog.FileNames)
                {
                    imagePathsListBox.Items.Add(file);
                }
            }
        }

        private void RemoveButton_Click(object sender, EventArgs e)
        {
            var selectedItem = imagePathsListBox.SelectedItem;
            if (selectedItem != null)
            {
                imagePathsListBox.Items.Remove(selectedItem);
            }
        }

        private void SaveButton_Click(object sender, EventArgs e)
        {
            // ... existing code for SaveButton_Click ...

            // Additional logic to save the current settings to config.txt
            SaveConfig();
        }

        private void SaveConfig()
        {
            string configFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "config.txt");
            using (StreamWriter writer = new StreamWriter(configFilePath))
            {
                writer.WriteLine("ImagePaths:");
                foreach (var item in imagePathsListBox.Items)
                {
                    writer.WriteLine(item.ToString());
                }
                writer.WriteLine($"TimerIntervalSeconds: {timerIntervalNumericUpDown.Value}");
            }
        }

    }
}