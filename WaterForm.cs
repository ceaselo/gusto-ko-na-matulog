    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Data;
    using System.Data.SqlClient;
    using System.Drawing;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using System.Windows.Forms;


namespace BillingSystemDesgin
{
    public partial class WaterForm : Form
    {
        public int ContactNumber { get; set; }
        public string ContactPassword { get; set; }
        public string ContactName { get; set; }

        private string connectionString = "Data Source=WIN-MAODJ10OMLA;Initial Catalog=REGISTER;Integrated Security=True;Encrypt=False";

        public WaterForm()
        {
            InitializeComponent();
            this.Load += WaterForm_Load;
        }

        private void WaterForm_Load(object sender, EventArgs e)
        {
            SetWaterBill();
            SetLastPaymentDate();
        }

        public void SetWaterBill()
        {
            label4.Text = GetWaterBill(ContactNumber, ContactPassword);
        }

        private string GetWaterBill(int contactNumber, string contactPassword)
        {
            string query = "SELECT water_bill FROM Registration WHERE contact_number = @ContactNumber AND contact_password = @ContactPassword";
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@ContactNumber", contactNumber);
                        command.Parameters.AddWithValue("@ContactPassword", contactPassword);

                        object result = command.ExecuteScalar();
                        if (result != null)
                        {
                            return result.ToString();
                        }
                        else
                        {
                            return "0";
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("An error occurred: " + ex.Message);
                return "Error";
            }
        }

        private void UpdateWaterBill(decimal newBillAmount)
        {
            string query = "UPDATE Registration SET water_bill = @NewWaterBill WHERE contact_number = @ContactNumber AND contact_password = @ContactPassword";
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@NewWaterBill", newBillAmount);
                        command.Parameters.AddWithValue("@ContactNumber", ContactNumber);
                        command.Parameters.AddWithValue("@ContactPassword", ContactPassword);

                        int rowsAffected = command.ExecuteNonQuery();
                        if (rowsAffected > 0)
                        {
                            //PLEASE GUMANA TO WAG NYO NA PAKELAMAN 
                        }
                        else
                        {
                            MessageBox.Show("Failed to update water bill.");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("An error occurred: " + ex.Message);
            }
        }

        private void SetLastPaymentDate()
        {
            string query = "SELECT last_payment_date_water FROM Registration WHERE contact_number = @ContactNumber AND contact_password = @ContactPassword";
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@ContactNumber", ContactNumber);
                        command.Parameters.AddWithValue("@ContactPassword", ContactPassword);

                        object result = command.ExecuteScalar();
                        if (result != null)
                        {
                            DateTime lastPaymentDate = Convert.ToDateTime(result);
                            label6.Text = lastPaymentDate.ToString("yyyy-MM-dd");

                            if (DateTime.Now > lastPaymentDate)
                            {
                                MessageBox.Show("Your payment due is passed", "Cutting water", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            }
                        }
                        else
                        {
                            label6.Text = "0";
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("An error occurred: " + ex.Message);
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (decimal.TryParse(textBox2.Text, out decimal paymentAmount) && decimal.TryParse(label4.Text, out decimal currentBillAmount))
            {
                decimal newBillAmount = currentBillAmount - paymentAmount;

                if (newBillAmount < 0)
                {
                    newBillAmount = 0;
                }

                UpdateWaterBill(newBillAmount);
                label4.Text = newBillAmount.ToString();

                string message = $"Dear {ContactName},\n\n";
                message += $"You have successfully paid {paymentAmount:C} for your water bill.\n";
                message += $"Your remaining water bill amount is now {newBillAmount:C}.\n";
                message += $"Thank you for your payment.";

                MessageBox.Show(message);
            }
            else
            {
                MessageBox.Show("Please enter a valid payment amount.");
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            PaymentPage paymentPage = new PaymentPage();
            paymentPage.ContactNumber = ContactNumber;
            paymentPage.ContactPassword = ContactPassword;
            paymentPage.SetContactName();
            paymentPage.Show();
            this.Close();
        }

        private void WaterForm_Load_1(object sender, EventArgs e)
        {

        }
    }
}