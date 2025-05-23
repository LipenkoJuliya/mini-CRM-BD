using System;
using System.Windows.Forms;

namespace мини_CRM
{
    public partial class ClientForm : Form
    {
        public Client Client { get; set; }

        public ClientForm()
        {
            InitializeComponent();
            Client = new Client();
        }

        public ClientForm(Client client)
        {
            InitializeComponent();
            Client = client;

            // Заполняем поля формы данными из объекта client
            nameTextBox.Text = client.Imya;
            lastNameTextBox.Text = client.Familiya;
            middleNameTextBox.Text = client.Otchestvo; // <-- Убедитесь, что это есть
            emailTextBox.Text = client.Email;
            phoneTextBox.Text = client.Telephon;
            addressTextBox.Text = client.Adress;
            companyTextBox.Text = client.Kompaniya;
            positionTextBox.Text = client.Dolznost;
            additionalInfoTextBox.Text = client.Dop_inf;
        }

        private void saveButton_Click(object sender, EventArgs e)
        {
            Client.Imya = nameTextBox.Text;
            Client.Familiya = lastNameTextBox.Text;
            Client.Otchestvo = middleNameTextBox.Text; // <-- Убедитесь, что это есть
            Client.Email = emailTextBox.Text;
            Client.Telephon = phoneTextBox.Text;
            Client.Adress = addressTextBox.Text;
            Client.Kompaniya = companyTextBox.Text;
            Client.Dolznost = positionTextBox.Text;
            Client.Dop_inf = additionalInfoTextBox.Text;

            DialogResult = DialogResult.OK;
            Close();
        }

        private void cancelButton_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            Close();
        }
    }
}
