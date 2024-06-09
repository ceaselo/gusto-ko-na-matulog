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
    public partial class ElectricalForm : Form
    {
        public int ContactNumber { get; set; }
        public string ContactPassword { get; set; }
        public string ContactName { get; set; }
        private string connectionString = "Data Source=WIN-MAODJ10OMLA;Initial Catalog=REGISTER;Integrated Security=True;Encrypt=False;Trust Server Certificate=True\r\n";

        public ElectricalForm()
        {
            InitializeComponent();
            this.Load += ElectricalForm_Load;
        }

        private void ElectricalForm_Load(object sender, EventArgs e)
        {
            SetElectricBill();
            SetLastPaymentDate();
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

        public void SetElectricBill()
        {
            label5.Text = GetElectricBill(ContactNumber, ContactPassword);
        }

        private string GetElectricBill(int contactNumber, string contactPassword)
        {
            string query = "Data Source=WIN-MAODJ10OMLA;Initial Catalog=REGISTER;Integrated Security=True;Encrypt=False";
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

        private void SetLastPaymentDate()
        {
            string query = "SELECT last_payment_date_electric FROM Registration WHERE contact_number = @ContactNumber AND contact_password = @ContactPassword";
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
                                MessageBox.Show("Your payment due is passed", "Cutting Electricity", MessageBoxButtons.OK, MessageBoxIcon.Warning);
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

        private void UpdateElectricBill(decimal newBillAmount)
        {
            string query = "UPDATE Registration SET electric_bill = @NewElectricBill WHERE contact_number = @ContactNumber AND contact_password = @ContactPassword";
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@NewElectricBill", newBillAmount);
                        command.Parameters.AddWithValue("@ContactNumber", ContactNumber);
                        command.Parameters.AddWithValue("@ContactPassword", ContactPassword);

                        int rowsAffected = command.ExecuteNonQuery();
                        if (rowsAffected > 0)
                        {
                            //GUMAGANA TO GUYS MANIWALA KAYO SAKEN
                        }
                        else
                        {
                            MessageBox.Show("0");
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
            if (decimal.TryParse(textBox2.Text, out decimal paymentAmount) && decimal.TryParse(label6.Text, out decimal currentBillAmount))
            {
                decimal newBillAmount = currentBillAmount - paymentAmount;

                if (newBillAmount < 0)
                {
                    newBillAmount = 0;
                }

                UpdateElectricBill(newBillAmount);
                label6.Text = newBillAmount.ToString();

                string message = $"Dear {ContactName},\n\n";
                message += $"You have successfully paid {paymentAmount:C} for your electric bill.\n";
                message += $"Your remaining electric bill amount is now {newBillAmount:C}.\n";
                message += $"Thank you for your payment.";

                MessageBox.Show(message);
            }
            else
            {
                MessageBox.Show("Please enter a valid payment amount.");
            }
        }
    }
}