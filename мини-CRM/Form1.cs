using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data.SQLite;
using System.IO;

namespace мини_CRM
{
    public partial class Form1 : Form
    {
        private List<Client> clients = new List<Client>();
        private List<Order> orders = new List<Order>();

        public Form1()
        {
            InitializeComponent();
            this.Load += Form1_Load;
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            DatabaseHelper.CreateDatabase();  // Создаем БД, если ее нет
            LoadClientsFromDatabase();
            UpdateClientList();
            LoadOrdersFromDatabase();
        }

        private void UpdateClientList()
        {
            clientListBox.Items.Clear();
            foreach (var client in clients)
            {
                clientListBox.Items.Add(client);
            }
        }

        private void DisplayClientDetails(Client client)
        {
            clientDetailsTextBox.Text = $"Imya: {client.Imya}\r\n" +
                                        $"Familiya: {client.Familiya}\r\n" +
                                        $"Otchestvo: {client.Otchestvo}\r\n" +
                                        $"Email: {client.Email}\r\n" +
                                        $"Telephon: {client.Telephon}\r\n" +
                                        $"Kompaniya: {client.Kompaniya}\r\n" +
                                        $"Dolznost: {client.Dolznost}\r\n" +
                                        $"Дата регистрации: {client.DataRegistracii.ToShortDateString()}\r\n" +
                                        $"Дополнительная информация: {client.Dop_inf}";
        }

        private void searchTextBox_TextChanged(object sender, EventArgs e)
        {
            string searchText = searchTextBox.Text.ToLower();
            List<Client> filteredClients = clients.Where(c =>
                c.Imya.ToLower().Contains(searchText) ||
                c.Familiya.ToLower().Contains(searchText) ||
                c.Email.ToLower().Contains(searchText))
                .ToList();

            clientListBox.Items.Clear();
            foreach (var client in filteredClients)
            {
                clientListBox.Items.Add(client);
            }
        }

        private void clientListBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (clientListBox.SelectedItem is Client selectedClient)
            {
                DisplayClientDetails(selectedClient);
                DisplayClientOrders(selectedClient);
            }
        }

        private void DisplayClientOrders(Client client)
        {
            clientOrdersListBox.Items.Clear();
            foreach (var order in orders.Where(o => o.ClientId == client.Id))
            {
                clientOrdersListBox.Items.Add(order);
            }
        }

        // Обработчик события для кнопки "Создать заказ"
        private void createOrderButton_Click(object sender, EventArgs e)
        {
            if (clientListBox.SelectedItem is Client selectedClient)
            {
                OrderForm orderForm = new OrderForm(selectedClient.Id); // Передаем ID клиента
                if (orderForm.ShowDialog() == DialogResult.OK)
                {
                    Order новыйЗаказ = orderForm.Order;

                    // Добавляем заказ в базу данных
                    DatabaseHelper.AddOrder(новыйЗаказ);

                    // Обновляем список заказов после добавления в БД
                    LoadOrdersFromDatabase();
                    DisplayClientOrders(selectedClient); // Обновляем список заказов
                }
            }
            else
            {
                MessageBox.Show("Выберите клиента для создания заказа.");
            }
        }

        // Обработчик для кнопки "Изменить статус заказа"
        private void updateOrderStatusButton_Click(object sender, EventArgs e)
        {
            if (clientOrdersListBox.SelectedItem is Order selectedOrder)
            {
                // Отображаем форму для изменения статуса
                StatusChangeForm statusChangeForm = new StatusChangeForm(selectedOrder.Status);
                if (statusChangeForm.ShowDialog() == DialogResult.OK)
                {
                    // Получаем новый статус
                    selectedOrder.Status = statusChangeForm.SelectedStatus;

                    // Обновляем статус заказа в базе данных
                    DatabaseHelper.UpdateOrder(selectedOrder);

                    // Обновляем отображение заказов для выбранного клиента
                    DisplayClientOrders(selectedOrder.Client);
                    LoadOrdersFromDatabase(); //Обновляем список заказов из БД.
                }
            }
            else
            {
                MessageBox.Show("Выберите заказ для изменения статуса.");
            }
        }
        private void clientOrdersListBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (clientOrdersListBox.SelectedItem is Order selectedOrder)
            {
                // Отображаем информацию о заказе (пример)
                clientDetailsTextBox.Text = $"Заказ №{selectedOrder.Id}\r\n" +
                                            $"Opisanie: {selectedOrder.Opisanie}\r\n" +
                                            $"Summa: {selectedOrder.Summa}\r\n" +
                                            $"Status: {selectedOrder.Status}";
            }
        }

        private void addClientButton_Click(object sender, EventArgs e)
        {
            ClientForm clientForm = new ClientForm(); // Создаем форму для ввода данных
            if (clientForm.ShowDialog() == DialogResult.OK) // Отображаем форму модально
            {
                Client новыйКлиент = clientForm.Client;  // Получаем клиента из формы

                // Добавляем клиента в базу данных
                DatabaseHelper.AddClient(новыйКлиент);

                // Обновляем список клиентов
                LoadClientsFromDatabase();
                UpdateClientList();
            }
        }

        private void editClientButton_Click(object sender, EventArgs e)
        {
            if (clientListBox.SelectedItem is Client selectedClient)
            {
                ClientForm clientForm = new ClientForm(selectedClient); // Передаем выбранного клиента
                if (clientForm.ShowDialog() == DialogResult.OK)
                {
                    //  Обновление информации о клиенте (не нужно, если используем привязку данных или свойства)
                    DatabaseHelper.UpdateClient(selectedClient); // Обновляем клиента в БД
                    LoadClientsFromDatabase(); // Загружаем клиентов из базы (для обновления данных)
                    UpdateClientList();
                    DisplayClientDetails(selectedClient); // Обновляем отображение деталей
                }
            }
            else
            {
                MessageBox.Show("Выберите клиента для редактирования.");
            }
        }

        private void deleteClientButton_Click(object sender, EventArgs e)
        {
            if (clientListBox.SelectedItem is Client selectedClient)
            {
                if (MessageBox.Show($"Удалить клиента {selectedClient.Imya} {selectedClient.Familiya}?", "Подтверждение", MessageBoxButtons.YesNo) == DialogResult.Yes)
                {
                    // Удаляем заказы клиента из базы данных
                    // (Удаление заказов будет происходить при удалении клиента, сделано в DatabaseHelper)

                    // Удаляем клиента из базы данных
                    DatabaseHelper.DeleteClient(selectedClient.Id);

                    // Обновляем список клиентов
                    LoadClientsFromDatabase();
                    UpdateClientList();
                }
            }
            else
            {
                MessageBox.Show("Выберите клиента для удаления.");
            }
        }

        // Загрузка клиентов из базы данных
        private void LoadClientsFromDatabase()
        {
            clients = DatabaseHelper.GetAllClients();
        }
        // Загрузка заказов из базы данных
        private void LoadOrdersFromDatabase()
        {
            orders = DatabaseHelper.GetAllOrders();
            // Связываем заказы с клиентами
            foreach (var order in orders)
            {
                order.Client = clients.FirstOrDefault(c => c.Id == order.ClientId);
            }
        }

        private void saveDbAsButton_Click(object sender, EventArgs e)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Filter = "SQLite Database (*.db)|*.db|All files (*.*)|*.*";
            saveFileDialog.Title = "Сохранить базу данных как...";
            saveFileDialog.FileName = DatabaseHelper.DbFilePath; // Используем DbFilePath

            if (saveFileDialog.ShowDialog() == DialogResult.OK)
            {
                string filePath = saveFileDialog.FileName;
                CopyDatabase(filePath);
            }
        }

        private void CopyDatabase(string destinationPath)
        {
            try
            {
                File.Copy(DatabaseHelper.DbFilePath, destinationPath, true); // Копируем файл БД
                MessageBox.Show("База данных успешно сохранена.", "Сохранение", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при сохранении базы данных: {ex.Message}", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void openDbButton_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "SQLite Database (*.db)|*.db|All files (*.*)|*.*";
            openFileDialog.Title = "Открыть базу данных";

            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                string filePath = openFileDialog.FileName;
                ChangeDatabase(filePath);
            }
        }

        private void ChangeDatabase(string newDbPath)
        {
            try
            {
                if (File.Exists(newDbPath))
                {
                    // Закрываем текущее соединение с базой данных (если оно есть)
                    // (Нужно убедиться, что соединения закрыты, чтобы файл можно было заменить)

                    // Обновляем путь к файлу базы данных в DatabaseHelper
                    DatabaseHelper.DbFilePath = newDbPath;

                    // Загружаем данные из новой базы данных
                    LoadClientsFromDatabase();
                    UpdateClientList();
                    LoadOrdersFromDatabase();

                    MessageBox.Show("База данных успешно открыта.", "Открытие", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else
                {
                    MessageBox.Show("Выбранный файл не существует.", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при открытии базы данных: {ex.Message}", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}