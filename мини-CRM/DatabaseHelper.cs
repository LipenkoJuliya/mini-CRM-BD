using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.IO;

namespace мини_CRM
{
    public class DatabaseHelper
    {
        public static string DbFilePath { get; set; } = "crm.db"; // Imya файла базы данных - теперь свойство

        // Метод для создания базы данных и таблиц (если они еще не существуют)
        public static void CreateDatabase()
        {
            if (!File.Exists(DbFilePath))
            {
                SQLiteConnection.CreateFile(DbFilePath); // Создаем файл базы данных

                using (SQLiteConnection dbConnection = new SQLiteConnection($"Data Source={DbFilePath};Version=3;"))
                {
                    dbConnection.Open();

                    // Создаем таблицу Clients
                    string createClientsTableQuery = @"
                        CREATE TABLE Clients (
                            Id INTEGER PRIMARY KEY AUTOINCREMENT,
                            Imya TEXT,
                            Familiya TEXT,
                            Otchestvo TEXT,
                            Email TEXT,
                            Telephon TEXT,
                            Adress TEXT,
                            Kompaniya TEXT,
                            Dolznost TEXT,
                            DataRegistracii DATETIME,
                            Dop_inf TEXT
                        );";
                    SQLiteCommand createClientsTableCommand = new SQLiteCommand(createClientsTableQuery, dbConnection);
                    createClientsTableCommand.ExecuteNonQuery();

                    // Создаем таблицу Orders
                    string createOrdersTableQuery = @"
                        CREATE TABLE Orders (
                            Id INTEGER PRIMARY KEY AUTOINCREMENT,
                            ClientId INTEGER NOT NULL,
                            DataRazmesheniya DATETIME,
                            Opisanie TEXT,
                            Status INTEGER,
                            Summa REAL,
                            FOREIGN KEY (ClientId) REFERENCES Clients(Id)
                        );";
                    SQLiteCommand createOrdersTableCommand = new SQLiteCommand(createOrdersTableQuery, dbConnection);
                    createOrdersTableCommand.ExecuteNonQuery();

                    dbConnection.Close();
                }
            }
        }

        // Метод для добавления клиента в базу данных
        public static void AddClient(Client client)
        {
            using (SQLiteConnection dbConnection = new SQLiteConnection($"Data Source={DbFilePath};Version=3;"))
            {
                dbConnection.Open();

                string insertClientQuery = @"
                    INSERT INTO Clients (Imya, Familiya, Otchestvo, Email, Telephon, Adress, Kompaniya, Dolznost, DataRegistracii, Dop_inf)
                    VALUES (@Imya, @Familiya, @Otchestvo, @Email, @Telephon, @Adress, @Kompaniya, @Dolznost, @DataRegistracii, @Dop_inf);";

                SQLiteCommand insertClientCommand = new SQLiteCommand(insertClientQuery, dbConnection);
                insertClientCommand.Parameters.AddWithValue("@Imya", client.Imya);
                insertClientCommand.Parameters.AddWithValue("@Familiya", client.Familiya);
                insertClientCommand.Parameters.AddWithValue("@Otchestvo", client.Otchestvo);
                insertClientCommand.Parameters.AddWithValue("@Email", client.Email);
                insertClientCommand.Parameters.AddWithValue("@Telephon", client.Telephon);
                insertClientCommand.Parameters.AddWithValue("@Adress", client.Adress);
                insertClientCommand.Parameters.AddWithValue("@Kompaniya", client.Kompaniya);
                insertClientCommand.Parameters.AddWithValue("@Dolznost", client.Dolznost);
                insertClientCommand.Parameters.AddWithValue("@DataRegistracii", client.DataRegistracii);
                insertClientCommand.Parameters.AddWithValue("@Dop_inf", client.Dop_inf);

                insertClientCommand.ExecuteNonQuery();

                dbConnection.Close();
            }
        }

        // Метод для обновления информации о клиенте
        public static void UpdateClient(Client client)
        {
            using (SQLiteConnection dbConnection = new SQLiteConnection($"Data Source={DbFilePath};Version=3;"))
            {
                dbConnection.Open();

                string updateClientQuery = @"
                    UPDATE Clients 
                    SET Imya = @Imya, Familiya = @Familiya, Otchestvo = @Otchestvo, Email = @Email, Telephon = @Telephon, Adress = @Adress, 
                        Kompaniya = @Kompaniya, Dolznost = @Dolznost, DataRegistracii = @DataRegistracii, Dop_inf = @Dop_inf
                    WHERE Id = @Id;";

                SQLiteCommand updateClientCommand = new SQLiteCommand(updateClientQuery, dbConnection);
                updateClientCommand.Parameters.AddWithValue("@Id", client.Id);
                updateClientCommand.Parameters.AddWithValue("@Imya", client.Imya);
                updateClientCommand.Parameters.AddWithValue("@Familiya", client.Familiya);
                updateClientCommand.Parameters.AddWithValue("@Otchestvo", client.Otchestvo);
                updateClientCommand.Parameters.AddWithValue("@Email", client.Email);
                updateClientCommand.Parameters.AddWithValue("@Telephon", client.Telephon);
                updateClientCommand.Parameters.AddWithValue("@Adress", client.Adress);
                updateClientCommand.Parameters.AddWithValue("@Kompaniya", client.Kompaniya);
                updateClientCommand.Parameters.AddWithValue("@Dolznost", client.Dolznost);
                updateClientCommand.Parameters.AddWithValue("@DataRegistracii", client.DataRegistracii);
                updateClientCommand.Parameters.AddWithValue("@Dop_inf", client.Dop_inf);

                updateClientCommand.ExecuteNonQuery();

                dbConnection.Close();
            }
        }

        // Метод для удаления клиента из базы данных
        public static void DeleteClient(int clientId)
        {
            using (SQLiteConnection dbConnection = new SQLiteConnection($"Data Source={DbFilePath};Version=3;"))
            {
                dbConnection.Open();

                // Сначала удаляем заказы, связанные с клиентом
                string deleteOrdersQuery = "DELETE FROM Orders WHERE ClientId = @ClientId;";
                SQLiteCommand deleteOrdersCommand = new SQLiteCommand(deleteOrdersQuery, dbConnection);
                deleteOrdersCommand.Parameters.AddWithValue("@ClientId", clientId);
                deleteOrdersCommand.ExecuteNonQuery();

                // Затем удаляем самого клиента
                string deleteClientQuery = "DELETE FROM Clients WHERE Id = @Id;";
                SQLiteCommand deleteClientCommand = new SQLiteCommand(deleteClientQuery, dbConnection);
                deleteClientCommand.Parameters.AddWithValue("@Id", clientId);
                deleteClientCommand.ExecuteNonQuery();

                dbConnection.Close();
            }
        }

        // Метод для получения списка всех клиентов из базы данных
        public static List<Client> GetAllClients()
        {
            List<Client> clients = new List<Client>();

            using (SQLiteConnection dbConnection = new SQLiteConnection($"Data Source={DbFilePath};Version=3;"))
            {
                dbConnection.Open();

                string selectClientsQuery = "SELECT * FROM Clients;";
                SQLiteCommand selectClientsCommand = new SQLiteCommand(selectClientsQuery, dbConnection);

                using (SQLiteDataReader reader = selectClientsCommand.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        Client client = new Client
                        {
                            Id = Convert.ToInt32(reader["Id"]),
                            Imya = reader["Imya"].ToString(),
                            Familiya = reader["Familiya"].ToString(),
                            Otchestvo = reader["Otchestvo"].ToString(),
                            Email = reader["Email"].ToString(),
                            Telephon = reader["Telephon"].ToString(),
                            Adress = reader["Adress"].ToString(),
                            Kompaniya = reader["Kompaniya"].ToString(),
                            Dolznost = reader["Dolznost"].ToString(),
                            DataRegistracii = DateTime.Parse(reader["DataRegistracii"].ToString()),
                            Dop_inf = reader["Dop_inf"].ToString()
                        };
                        clients.Add(client);
                    }
                }

                dbConnection.Close();
            }

            return clients;
        }

        // Метод для добавления заказа в базу данных
        public static void AddOrder(Order order)
        {
            using (SQLiteConnection dbConnection = new SQLiteConnection($"Data Source={DbFilePath};Version=3;"))
            {
                dbConnection.Open();

                string insertOrderQuery = @"
                    INSERT INTO Orders (ClientId, DataRazmesheniya, Opisanie, Status, Summa)
                    VALUES (@ClientId, @DataRazmesheniya, @Opisanie, @Status, @Summa);";

                SQLiteCommand insertOrderCommand = new SQLiteCommand(insertOrderQuery, dbConnection);
                insertOrderCommand.Parameters.AddWithValue("@ClientId", order.ClientId);
                insertOrderCommand.Parameters.AddWithValue("@DataRazmesheniya", order.DataRazmesheniya);
                insertOrderCommand.Parameters.AddWithValue("@Opisanie", order.Opisanie);
                insertOrderCommand.Parameters.AddWithValue("@Status", (int)order.Status); // Сохраняем как целое число
                insertOrderCommand.Parameters.AddWithValue("@Summa", order.Summa);

                insertOrderCommand.ExecuteNonQuery();

                dbConnection.Close();
            }
        }

        // Метод для обновления информации о заказе
        public static void UpdateOrder(Order order)
        {
            using (SQLiteConnection dbConnection = new SQLiteConnection($"Data Source={DbFilePath};Version=3;"))
            {
                dbConnection.Open();

                string updateOrderQuery = @"
                    UPDATE Orders 
                    SET ClientId = @ClientId, DataRazmesheniya = @DataRazmesheniya, Opisanie = @Opisanie, 
                        Status = @Status, Summa = @Summa
                    WHERE Id = @Id;";

                SQLiteCommand updateOrderCommand = new SQLiteCommand(updateOrderQuery, dbConnection);
                updateOrderCommand.Parameters.AddWithValue("@Id", order.Id);
                updateOrderCommand.Parameters.AddWithValue("@ClientId", order.ClientId);
                updateOrderCommand.Parameters.AddWithValue("@DataRazmesheniya", order.DataRazmesheniya);
                updateOrderCommand.Parameters.AddWithValue("@Opisanie", order.Opisanie);
                updateOrderCommand.Parameters.AddWithValue("@Status", (int)order.Status); // Сохраняем как целое число
                updateOrderCommand.Parameters.AddWithValue("@Summa", order.Summa);

                updateOrderCommand.ExecuteNonQuery();

                dbConnection.Close();
            }
        }

        // Метод для удаления заказа из базы данных
        public static void DeleteOrder(int orderId)
        {
            using (SQLiteConnection dbConnection = new SQLiteConnection($"Data Source={DbFilePath};Version=3;"))
            {
                dbConnection.Open();

                string deleteOrderQuery = "DELETE FROM Orders WHERE Id = @Id;";
                SQLiteCommand deleteOrderCommand = new SQLiteCommand(deleteOrderQuery, dbConnection);
                deleteOrderCommand.Parameters.AddWithValue("@Id", orderId);
                deleteOrderCommand.ExecuteNonQuery();

                dbConnection.Close();
            }
        }

        // Метод для получения списка всех заказов из базы данных
        public static List<Order> GetAllOrders()
        {
            List<Order> orders = new List<Order>();

            using (SQLiteConnection dbConnection = new SQLiteConnection($"Data Source={DbFilePath};Version=3;"))
            {
                dbConnection.Open();

                string selectOrdersQuery = "SELECT * FROM Orders;";
                SQLiteCommand selectOrdersCommand = new SQLiteCommand(selectOrdersQuery, dbConnection);

                using (SQLiteDataReader reader = selectOrdersCommand.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        Order order = new Order
                        {
                            Id = Convert.ToInt32(reader["Id"]),
                            ClientId = Convert.ToInt32(reader["ClientId"]),
                            DataRazmesheniya = DateTime.Parse(reader["DataRazmesheniya"].ToString()),
                            Opisanie = reader["Opisanie"].ToString(),
                            Status = (OrderStatus)Convert.ToInt32(reader["Status"]), // Преобразуем из int в enum
                            Summa = Convert.ToDecimal(reader["Summa"])
                        };
                        orders.Add(order);
                    }
                }

                dbConnection.Close();
            }

            return orders;
        }

        // Метод для получения заказов по Id клиента
        public static List<Order> GetOrdersByClientId(int clientId)
        {
            List<Order> orders = new List<Order>();

            using (SQLiteConnection dbConnection = new SQLiteConnection($"Data Source={DbFilePath};Version=3;"))
            {
                dbConnection.Open();

                string selectOrdersQuery = "SELECT * FROM Orders WHERE ClientId = @ClientId;";
                SQLiteCommand selectOrdersCommand = new SQLiteCommand(selectOrdersQuery, dbConnection);
                selectOrdersCommand.Parameters.AddWithValue("@ClientId", clientId);

                using (SQLiteDataReader reader = selectOrdersCommand.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        Order order = new Order
                        {
                            Id = Convert.ToInt32(reader["Id"]),
                            ClientId = Convert.ToInt32(reader["ClientId"]),
                            DataRazmesheniya = DateTime.Parse(reader["DataRazmesheniya"].ToString()),
                            Opisanie = reader["Opisanie"].ToString(),
                            Status = (OrderStatus)Convert.ToInt32(reader["Status"]), // Преобразуем из int в enum
                            Summa = Convert.ToDecimal(reader["Summa"])
                        };
                        orders.Add(order);
                    }
                }

                dbConnection.Close();
            }

            return orders;
        }
    }
}