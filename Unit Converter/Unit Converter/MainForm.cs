using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Reflection.Metadata.Ecma335;
using System.Runtime.CompilerServices;
using System.Windows.Forms;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace UnitConverter
{
    public class MainForm : Form
    {
        //controls
        private readonly ComboBox cmbCategory = new();
        private readonly RadioButton rbMetric = new();
        private readonly RadioButton rbImperial = new();
        private readonly TextBox txtValue = new();
        private readonly CheckBox chkPrecision = new();
        private readonly ListBox lstHistory = new();
        private readonly Label lblResult = new();
        private readonly Label lblStatistics = new();
        
        private readonly List<int> categories = new();

        public MainForm() 
        { 
            Text = "Unit Converter - Dilyan Yanev - F117729";
            Size = new Size(750, 650);
            StartPosition = FormStartPosition.CenterScreen;
            MaximizeBox = false;

            CreateInterfae();
            UpdateStatistics();
        }

        private void CreateInterfae()
        {
            Label lblCategory = new()
            {
                Text = "Conversion Category",
                Location = new Point(30, 25),
                AutoSize = true
            };

            cmbCategory.Location = new Point(30, 55);
            cmbCategory.Size = new Size(280, 30);
            cmbCategory.DropDownStyle = ComboBoxStyle.DropDownList;

            cmbCategory.Items.AddRange(new object[] {
                "Length",
                "Weight",
                "Volume",
                "Temperature"
            });

            cmbCategory.SelectedIndex = 0;

            GroupBox grpDirection = new()
            {
                Text = "Conversion Category",
                Location = new Point(30, 110),
                Size = new Size(650, 90)
            };

            rbMetric.Text = "Metric to Imperial";
            rbMetric.Location = new Point(20, 35);
            rbMetric.AutoSize = true;
            rbMetric.Checked = true;

            rbImperial.Text = "Imperial to Metric";
            rbImperial.Location = new Point(320, 35);
            rbImperial.AutoSize = true;

            grpDirection.Controls.Add(rbMetric);
            grpDirection.Controls.Add(rbImperial);

            Label lblInput = new()
            {
                Text = "Enter value:",
                Location = new Point(30, 220),
                AutoSize = true
            };

            txtValue.Location = new Point(30, 250);
            txtValue.Size = new Size(280, 30);

            chkPrecision.Text = "4 decimal precision";
            chkPrecision.Location = new Point(30, 300);
            chkPrecision.AutoSize = true;

            Button btnConvert = new()
            {
                Text = "Convert",
                Location = new Point(30, 340),
                Size = new Size(140, 40)
            };

            btnConvert.Click += BtnConvert_Click; 

            lblResult.Text = "Result:";
            lblResult.Location = new Point(200, 350);
            lblResult.Size = new Size(480, 30);
            lblResult.Font = new Font("Arial", 11, FontStyle.Bold);

            Label lblHistory = new()
            {
                Text = "Conversion History:",
                Location = new Point(30, 400),
                AutoSize = true
            };

            lstHistory.Location = new Point(30, 430);
            lstHistory.Size = new Size(650, 110);

            Button btnDelete = new()
            {
                Text = "Delete Selection:",
                Location = new Point(30, 555),
                Size = new Size(150, 35)
            };

            btnDelete.Click += BtnDelete_Click; 

            lblStatistics.Location = new Point(205, 555);
            lblStatistics.Size = new Size(475, 50);

            Controls.Add(lblCategory);
            Controls.Add(cmbCategory);
            Controls.Add(grpDirection);
            Controls.Add(lblInput);
            Controls.Add(txtValue);
            Controls.Add(chkPrecision);
            Controls.Add(lblResult);
            Controls.Add(btnConvert);
            Controls.Add(lblResult);
            Controls.Add(lblHistory);
            Controls.Add(lstHistory);
            Controls.Add(btnDelete);
            Controls.Add(lblStatistics);
        }

        private static double ConvertUnit(double v, int c, bool mti) // mti = metricToImperial
        {
            switch (c) 
            {
                case 0:
                    return mti ? v * 3.28084 : v / 3.28084;
                case 1:
                    return mti ? v * 2.20462 : v / 2.20462;
                case 2:
                    return mti ? v * 0.264172 : v / 0.264172;
                case 3:
                    return mti ? v * 9 / 5 + 32 : (v - 32) * 5 / 9;
                default:
                    throw new ArgumentException("Invalid Category!!!");
            }        
        }

        private void BtnConvert_Click(object? sender, EventArgs e) 
        {
            if (!double.TryParse(
                txtValue.Text, out double value)
                || double.IsNaN(value)
                || double.IsInfinity(value)) 
            {
                MessageBox.Show(
                    "Please enter valdi nummber :D",
                    "Invalid Input! Try Again :D",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);
                return;
            }

            int category = cmbCategory.SelectedIndex;

            if (category != 3 && value < 0)
            {
                MessageBox.Show(
                    "Negative values not allowed",
                    "Invalid INput! Try again :D",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);
                return;
            }

            bool mti = rbMetric.Checked;

            double result = ConvertUnit(value, category, mti);

            if (double.IsNaN(result) || double.IsInfinity(result)) 
            {
                MessageBox.Show(
                    "Too big result",
                    "Conversion error .....");
                return;
            }

            int decimals = chkPrecision.Checked ? 4 : 2;

            string[] metricU = 
            {
                "m", "kg", "L", "°C"
            };
            string[] imperialU =
            {
                "ft", "lbs", "US gal", "°F"
            };

            string fromU = mti ? metricU[category] : imperialU[category];
            string toU = mti ? imperialU[category] : metricU[category];

            string formattedValue = value.ToString($"F{decimals}");
            string formattedResult = result.ToString($"F{decimals}");
            string description = $"{formattedValue} {fromU} = " + $"{formattedResult} {toU}";

            lblResult.Text = $"Result: {formattedResult} {toU}";

            lstHistory.Items.Add(description);
            categories.Add(category);

            UpdateStatistics();
        }

        private void BtnDelete_Click(object? sender, EventArgs e) 
        {
            int index = lstHistory.SelectedIndex;

            if (index == -1)
            {
                MessageBox.Show(
                    "Select Conversion to delete");
                return;
            }

            lstHistory.Items.RemoveAt(index);
            categories.RemoveAt(index);

            UpdateStatistics(); 
        }

        private void UpdateStatistics() 
        {
            int[] counts = new int[4];

            foreach (int category in categories) 
            {
                counts[category]++;
            }

            lblStatistics.Text = $"length: {counts[0]} | " + $"weigth: {counts[1]}\n" + $"volume: {counts[2]} | " + $"temperature: {counts[3]} | ";
        }

    }

}