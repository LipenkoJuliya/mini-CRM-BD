using System;
using System.Windows.Forms;

namespace мини_CRM
{
    public partial class StatusChangeForm : Form
    {
        public OrderStatus SelectedStatus { get; private set; }

        public StatusChangeForm(OrderStatus currentStatus)
        {
            InitializeComponent();

            // Заполняем ComboBox значениями из перечисления OrderStatus
            statusComboBox.DataSource = Enum.GetValues(typeof(OrderStatus));

            // Устанавливаем выбранный статус
            statusComboBox.SelectedItem = currentStatus;
            SelectedStatus = currentStatus;
        }

        private void saveButton_Click(object sender, EventArgs e)
        {
            SelectedStatus = (OrderStatus)statusComboBox.SelectedItem;
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
