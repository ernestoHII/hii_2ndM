using System.Drawing;

namespace _2ndMonitor
{
    partial class Form1
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>


        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1));
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.pictureBox2 = new System.Windows.Forms.PictureBox();
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.textBox2 = new System.Windows.Forms.TextBox();
            this.textBox3 = new System.Windows.Forms.TextBox();
            this.textBox4 = new System.Windows.Forms.TextBox();
            this.textBox5 = new System.Windows.Forms.TextBox();
            this.textBox6 = new System.Windows.Forms.TextBox();
            this.textBox7 = new System.Windows.Forms.TextBox();
            this.textBox8 = new System.Windows.Forms.TextBox();
            this.textBox9 = new System.Windows.Forms.TextBox();
            this.textBox10 = new System.Windows.Forms.TextBox();
            this.textBox11 = new System.Windows.Forms.TextBox();
            this.textBox12 = new System.Windows.Forms.TextBox();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            float newFontSize = 12.0f; // New font size, e.g., 12 point
            Size newSize = new Size(200, 30); // New size, e.g., 200 pixels wide by 30 pixels tall

            // Updating each TextBox
            this.SalesNumber = new System.Windows.Forms.TextBox();
            this.SalesNumber.Size = newSize;
            this.SalesNumber.Font = new Font(this.SalesNumber.Font.FontFamily, newFontSize);

            this.NetTotal = new System.Windows.Forms.TextBox();
            this.NetTotal.Size = newSize;
            this.NetTotal.Font = new Font(this.NetTotal.Font.FontFamily, newFontSize);

            this.GST = new System.Windows.Forms.TextBox();
            this.GST.Size = newSize;
            this.GST.Font = new Font(this.GST.Font.FontFamily, newFontSize);

            this.Total = new System.Windows.Forms.TextBox();
            this.Total.Size = newSize;
            this.Total.Font = new Font(this.Total.Font.FontFamily, newFontSize);

            this.Paid = new System.Windows.Forms.TextBox();
            this.Paid.Size = newSize;
            this.Paid.Font = new Font(this.Paid.Font.FontFamily, newFontSize);

            this.Discount = new System.Windows.Forms.TextBox();
            this.Discount.Size = newSize;
            this.Discount.Font = new Font(this.Discount.Font.FontFamily, newFontSize);

            this.Balance = new System.Windows.Forms.TextBox();
            this.Balance.Size = newSize;
            this.Balance.Font = new Font(this.Balance.Font.FontFamily, newFontSize);

            this.subTotal = new System.Windows.Forms.TextBox();
            this.subTotal.Size = newSize;
            this.subTotal.Font = new Font(this.subTotal.Font.FontFamily, newFontSize);

            this.Change = new System.Windows.Forms.TextBox();
            this.Change.Size = newSize;
            this.Change.Font = new Font(this.Change.Font.FontFamily, newFontSize);

            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.button1 = new System.Windows.Forms.Button();
            this.pictureBox3 = new System.Windows.Forms.PictureBox();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox2)).BeginInit();
            this.groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox3)).BeginInit();
            this.SuspendLayout();
            // 
            // pictureBox1
            // 
            this.pictureBox1.Location = new System.Drawing.Point(699, 8);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(639, 657);
            this.pictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pictureBox1.TabIndex = 0;
            this.pictureBox1.TabStop = false;
            // 
            // pictureBox2
            // 
            this.pictureBox2.Image = ((System.Drawing.Image)(resources.GetObject("pictureBox2.Image")));
            this.pictureBox2.Location = new System.Drawing.Point(10, 20);
            this.pictureBox2.Name = "pictureBox2";
            this.pictureBox2.Size = new System.Drawing.Size(100, 100);
            this.pictureBox2.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pictureBox2.TabIndex = 1;
            this.pictureBox2.TabStop = false;
            // 
            // textBox1
            // 
            this.textBox1.Enabled = false;
            this.textBox1.Location = new System.Drawing.Point(127, 20);
            this.textBox1.Name = "textBox1";
            this.textBox1.Size = new System.Drawing.Size(246, 20);
            this.textBox1.TabIndex = 2;
            this.textBox1.Text = "Human Incubator POS System";
            // 
            // textBox2
            // 
            this.textBox2.Enabled = false;
            this.textBox2.Location = new System.Drawing.Point(127, 46);
            this.textBox2.Name = "textBox2";
            this.textBox2.Size = new System.Drawing.Size(246, 20);
            this.textBox2.TabIndex = 3;
            this.textBox2.Text = "10/25/2023 9:00 AM";
            // 
            // textBox3
            // 
            this.textBox3.Enabled = false;
            this.textBox3.Location = new System.Drawing.Point(127, 72);
            this.textBox3.Name = "textBox3";
            this.textBox3.Size = new System.Drawing.Size(84, 20);
            this.textBox3.TabIndex = 4;
            this.textBox3.Text = "Transaction No.";
            // 
            // textBox4
            // 
            this.textBox4.Enabled = false;
            this.textBox4.Location = new System.Drawing.Point(127, 100);
            this.textBox4.Name = "textBox4";
            this.textBox4.Size = new System.Drawing.Size(246, 20);
            this.textBox4.TabIndex = 5;
            this.textBox4.Text = "Store: 01-A";
            // 
            // textBox5
            // 
            this.textBox5.Enabled = false;
            this.textBox5.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.textBox5.Location = new System.Drawing.Point(419, 620);
            this.textBox5.Name = "textBox5";
            this.textBox5.Size = new System.Drawing.Size(119, 20);
            this.textBox5.TabIndex = 9;
            this.textBox5.Text = "Change";
            // 
            // textBox6
            // 
            this.textBox6.Enabled = false;
            this.textBox6.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.textBox6.Location = new System.Drawing.Point(417, 568);
            this.textBox6.Name = "textBox6";
            this.textBox6.Size = new System.Drawing.Size(119, 20);
            this.textBox6.TabIndex = 8;
            this.textBox6.Text = "Balance";
            // 
            // textBox7
            // 
            this.textBox7.Enabled = false;
            this.textBox7.Location = new System.Drawing.Point(419, 515);
            this.textBox7.Name = "textBox7";
            this.textBox7.Size = new System.Drawing.Size(119, 20);
            this.textBox7.TabIndex = 7;
            this.textBox7.Text = "Paid";
            // 
            // textBox8
            // 
            this.textBox8.Enabled = false;
            this.textBox8.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.textBox8.Location = new System.Drawing.Point(419, 459);
            this.textBox8.Name = "textBox8";
            this.textBox8.Size = new System.Drawing.Size(119, 20);
            this.textBox8.TabIndex = 6;
            this.textBox8.Text = "Total";
            // 
            // textBox9
            // 
            this.textBox9.Enabled = false;
            this.textBox9.Location = new System.Drawing.Point(419, 405);
            this.textBox9.Name = "textBox9";
            this.textBox9.Size = new System.Drawing.Size(119, 20);
            this.textBox9.TabIndex = 13;
            this.textBox9.Text = "GST Inc.";
            // 
            // textBox10
            // 
            this.textBox10.Enabled = false;
            this.textBox10.Location = new System.Drawing.Point(419, 351);
            this.textBox10.Name = "textBox10";
            this.textBox10.Size = new System.Drawing.Size(119, 20);
            this.textBox10.TabIndex = 12;
            this.textBox10.Text = "Net Total";
            // 
            // textBox11
            // 
            this.textBox11.Enabled = false;
            this.textBox11.Location = new System.Drawing.Point(419, 290);
            this.textBox11.Name = "textBox11";
            this.textBox11.Size = new System.Drawing.Size(117, 20);
            this.textBox11.TabIndex = 11;
            this.textBox11.Text = "Discount";
            // 
            // textBox12
            // 
            this.textBox12.Enabled = false;
            this.textBox12.Location = new System.Drawing.Point(419, 233);
            this.textBox12.Name = "textBox12";
            this.textBox12.Size = new System.Drawing.Size(117, 20);
            this.textBox12.TabIndex = 10;
            this.textBox12.Text = "Sub Total:";
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.pictureBox3);
            this.groupBox1.Controls.Add(this.SalesNumber);
            this.groupBox1.Controls.Add(this.textBox2);
            this.groupBox1.Controls.Add(this.pictureBox2);
            this.groupBox1.Controls.Add(this.textBox1);
            this.groupBox1.Controls.Add(this.groupBox2);
            this.groupBox1.Controls.Add(this.textBox3);
            this.groupBox1.Controls.Add(this.textBox4);
            this.groupBox1.Location = new System.Drawing.Point(12, 8);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(681, 209);
            this.groupBox1.TabIndex = 15;
            this.groupBox1.TabStop = false;
            // 
            // SalesNumber
            // 
            this.SalesNumber.Enabled = false;
            this.SalesNumber.Location = new System.Drawing.Point(217, 72);
            this.SalesNumber.Name = "SalesNumber";
            this.SalesNumber.Size = new System.Drawing.Size(156, 20);
            this.SalesNumber.TabIndex = 6;
            // 
            // NetTotal
            // 
            this.NetTotal.Enabled = false;
            this.NetTotal.Location = new System.Drawing.Point(544, 351);
            this.NetTotal.Name = "NetTotal";
            this.NetTotal.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
            this.NetTotal.Size = new System.Drawing.Size(109, 20);
            this.NetTotal.TabIndex = 21;
            this.NetTotal.Text = "0.00";
            // 
            // GST
            // 
            this.GST.Enabled = false;
            this.GST.Location = new System.Drawing.Point(544, 405);
            this.GST.Name = "GST";
            this.GST.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
            this.GST.Size = new System.Drawing.Size(109, 20);
            this.GST.TabIndex = 22;
            this.GST.Text = "0.00";
            // 
            // Total
            // 
            this.Total.Enabled = false;
            this.Total.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Total.Location = new System.Drawing.Point(544, 459);
            this.Total.Name = "Total";
            this.Total.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
            this.Total.Size = new System.Drawing.Size(109, 20);
            this.Total.TabIndex = 15;
            this.Total.Text = "0.00";
            // 
            // Paid
            // 
            this.Paid.Enabled = false;
            this.Paid.Location = new System.Drawing.Point(544, 515);
            this.Paid.Name = "Paid";
            this.Paid.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
            this.Paid.Size = new System.Drawing.Size(109, 20);
            this.Paid.TabIndex = 16;
            this.Paid.Text = "0.00";
            // 
            // Discount
            // 
            this.Discount.Enabled = false;
            this.Discount.Location = new System.Drawing.Point(544, 290);
            this.Discount.Name = "Discount";
            this.Discount.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
            this.Discount.Size = new System.Drawing.Size(109, 20);
            this.Discount.TabIndex = 20;
            this.Discount.Text = "0.00";
            // 
            // Balance
            // 
            this.Balance.Enabled = false;
            this.Balance.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Balance.Location = new System.Drawing.Point(542, 568);
            this.Balance.Name = "Balance";
            this.Balance.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
            this.Balance.Size = new System.Drawing.Size(109, 20);
            this.Balance.TabIndex = 17;
            this.Balance.Text = "0.00";
            // 
            // subTotal
            // 
            this.subTotal.Enabled = false;
            this.subTotal.Location = new System.Drawing.Point(548, 233);
            this.subTotal.Name = "subTotal";
            this.subTotal.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
            this.subTotal.Size = new System.Drawing.Size(105, 20);
            this.subTotal.TabIndex = 19;
            this.subTotal.Text = "0.00";
            // 
            // Change
            // 
            this.Change.Enabled = false;
            this.Change.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Change.Location = new System.Drawing.Point(544, 620);
            this.Change.Name = "Change";
            this.Change.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
            this.Change.Size = new System.Drawing.Size(109, 20);
            this.Change.TabIndex = 18;
            this.Change.Text = "0.00";
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.AutoScroll = true;
            this.tableLayoutPanel1.ColumnCount = 4;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel1.Location = new System.Drawing.Point(13, 144);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 8;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.Size = new System.Drawing.Size(372, 528);
            this.tableLayoutPanel1.TabIndex = 17;
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(12, 678);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(1326, 23);
            this.button1.TabIndex = 7;
            this.button1.Text = "^";
            this.button1.UseVisualStyleBackColor = true;
            // 
            // pictureBox3
            // 
            this.pictureBox3.Image = ((System.Drawing.Image)(resources.GetObject("pictureBox3.Image")));
            this.pictureBox3.Location = new System.Drawing.Point(433, 9);
            this.pictureBox3.Name = "pictureBox3";
            this.pictureBox3.Size = new System.Drawing.Size(190, 191);
            this.pictureBox3.SizeMode = System.Windows.Forms.PictureBoxSizeMode.AutoSize;
            this.pictureBox3.TabIndex = 14;
            this.pictureBox3.TabStop = false;
            // 
            // groupBox2
            // 
            this.groupBox2.Location = new System.Drawing.Point(353, 206);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(328, 458);
            this.groupBox2.TabIndex = 16;
            this.groupBox2.TabStop = false;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1372, 705);
            this.Controls.Add(this.NetTotal);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.GST);
            this.Controls.Add(this.tableLayoutPanel1);
            this.Controls.Add(this.Total);
            this.Controls.Add(this.Paid);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.Discount);
            this.Controls.Add(this.pictureBox1);
            this.Controls.Add(this.Balance);
            this.Controls.Add(this.subTotal);
            this.Controls.Add(this.textBox5);
            this.Controls.Add(this.Change);
            this.Controls.Add(this.textBox12);
            this.Controls.Add(this.textBox6);
            this.Controls.Add(this.textBox10);
            this.Controls.Add(this.textBox11);
            this.Controls.Add(this.textBox9);
            this.Controls.Add(this.textBox7);
            this.Controls.Add(this.textBox8);
            this.Name = "Form1";
            this.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
            this.Text = "Form1";
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox2)).EndInit();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox3)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.PictureBox pictureBox2;
        private System.Windows.Forms.TextBox textBox1;
        private System.Windows.Forms.TextBox textBox2;
        private System.Windows.Forms.TextBox textBox3;
        private System.Windows.Forms.TextBox textBox4;
        private System.Windows.Forms.TextBox textBox5;
        private System.Windows.Forms.TextBox textBox6;
        private System.Windows.Forms.TextBox textBox7;
        private System.Windows.Forms.TextBox textBox8;
        private System.Windows.Forms.TextBox textBox9;
        private System.Windows.Forms.TextBox textBox10;
        private System.Windows.Forms.TextBox textBox11;
        private System.Windows.Forms.TextBox textBox12;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.TextBox NetTotal;
        private System.Windows.Forms.TextBox GST;
        private System.Windows.Forms.TextBox Total;
        private System.Windows.Forms.TextBox Paid;
        private System.Windows.Forms.TextBox Discount;
        private System.Windows.Forms.TextBox Balance;
        private System.Windows.Forms.TextBox subTotal;
        private System.Windows.Forms.TextBox Change;
        private System.Windows.Forms.TextBox SalesNumber;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.PictureBox pictureBox3;
        private System.Windows.Forms.GroupBox groupBox2;
    }
}

