using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.IO;

namespace мини_CRM
{
    public class DatabaseHelper
    {
        public static string DbFilePath { get; set; } = "crm.db"; // Имя файла базы данных - теперь свойство

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
                            Имя TEXT,
                            Фамилия TEXT,
                            Отчество TEXT,
                            Email TEXT,
                            Телефон TEXT,
                            Адрес TEXT,
                            Компания TEXT,
                            Должность TEXT,
                            ДатаРегистрации DATETIME,
                            ДополнительнаяИнформация TEXT
                        );";
                    SQLiteCommand createClientsTableCommand = new SQLiteCommand(createClientsTableQuery, dbConnection);
                    createClientsTableCommand.ExecuteNonQuery();

                    // Создаем таблицу Orders
                    string createOrdersTableQuery = @"
                        CREATE TABLE Orders (
                            Id INTEGER PRIMARY KEY AUTOINCREMENT,
                            ClientId INTEGER NOT NULL,
                            ДатаРазмещения DATETIME,
                            Описание TEXT,
                            Статус INTEGER,
                            Сумма REAL,
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
                    INSERT INTO Clients (Имя, Фамилия, Отчество, Email, Телефон, Адрес, Компания, Должность, ДатаРегистрации, ДополнительнаяИнформация)
                    VALUES (@Имя, @Фамилия, @Отчество, @Email, @Телефон, @Адрес, @Компания, @Должность, @ДатаРегистрации, @ДополнительнаяИнформация);";

                SQLiteCommand insertClientCommand = new SQLiteCommand(insertClientQuery, dbConnection);
                insertClientCommand.Parameters.AddWithValue("@Имя", client.Имя);
                insertClientCommand.Parameters.AddWithValue("@Фамилия", client.Фамилия);
                insertClientCommand.Parameters.AddWithValue("@Отчество", client.Отчество);
                insertClientCommand.Parameters.AddWithValue("@Email", client.Email);
                insertClientCommand.Parameters.AddWithValue("@Телефон", client.Телефон);
                insertClientCommand.Parameters.AddWithValue("@Адрес", client.Адрес);
                insertClientCommand.Parameters.AddWithValue("@Компания", client.Компания);
                insertClientCommand.Parameters.AddWithValue("@Должность", client.Должность);
                insertClientCommand.Parameters.AddWithValue("@ДатаРегистрации", client.ДатаРегистрации);
                insertClientCommand.Parameters.AddWithValue("@ДополнительнаяИнформация", client.ДополнительнаяИнформация);

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
                    SET Имя = @Имя, Фамилия = @Фамилия, Отчество = @Отчество, Email = @Email, Телефон = @Телефон, Адрес = @Адрес, 
                        Компания = @Компания, Должность = @Должность, ДатаРегистрации = @ДатаРегистрации, ДополнительнаяИнформация = @ДополнительнаяИнформация
                    WHERE Id = @Id;";

                SQLiteCommand updateClientCommand = new SQLiteCommand(updateClientQuery, dbConnection);
                updateClientCommand.Parameters.AddWithValue("@Id", client.Id);
                updateClientCommand.Parameters.AddWithValue("@Имя", client.Имя);
                updateClientCommand.Parameters.AddWithValue("@Фамилия", client.Фамилия);
                updateClientCommand.Parameters.AddWithValue("@Отчество", client.Отчество);
                updateClientCommand.Parameters.AddWithValue("@Email", client.Email);
                updateClientCommand.Parameters.AddWithValue("@Телефон", client.Телефон);
                updateClientCommand.Parameters.AddWithValue("@Адрес", client.Адрес);
                updateClientCommand.Parameters.AddWithValue("@Компания", client.Компания);
                updateClientCommand.Parameters.AddWithValue("@Должность", client.Должность);
                updateClientCommand.Parameters.AddWithValue("@ДатаРегистрации", client.ДатаРегистрации);
                updateClientCommand.Parameters.AddWithValue("@ДополнительнаяИнформация", client.ДополнительнаяИнформация);

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
                            Имя = reader["Имя"].ToString(),
                            Фамилия = reader["Фамилия"].ToString(),
                            Отчество = reader["Отчество"].ToString(),
                            Email = reader["Email"].ToString(),
                            Телефон = reader["Телефон"].ToString(),
                            Адрес = reader["Адрес"].ToString(),
                            Компания = reader["Компания"].ToString(),
                            Должность = reader["Должность"].ToString(),
                            ДатаРегистрации = DateTime.Parse(reader["ДатаРегистрации"].ToString()),
                            ДополнительнаяИнформация = reader["ДополнительнаяИнформация"].ToString()
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
                    INSERT INTO Orders (ClientId, ДатаРазмещения, Описание, Статус, Сумма)
                    VALUES (@ClientId, @ДатаРазмещения, @Описание, @Статус, @Сумма);";

                SQLiteCommand insertOrderCommand = new SQLiteCommand(insertOrderQuery, dbConnection);
                insertOrderCommand.Parameters.AddWithValue("@ClientId", order.ClientId);
                insertOrderCommand.Parameters.AddWithValue("@ДатаРазмещения", order.ДатаРазмещения);
                insertOrderCommand.Parameters.AddWithValue("@Описание", order.Описание);
                insertOrderCommand.Parameters.AddWithValue("@Статус", (int)order.Статус); // Сохраняем как целое число
                insertOrderCommand.Parameters.AddWithValue("@Сумма", order.Сумма);

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
                    SET ClientId = @ClientId, ДатаРазмещения = @ДатаРазмещения, Описание = @Описание, 
                        Статус = @Статус, Сумма = @Сумма
                    WHERE Id = @Id;";

                SQLiteCommand updateOrderCommand = new SQLiteCommand(updateOrderQuery, dbConnection);
                updateOrderCommand.Parameters.AddWithValue("@Id", order.Id);
                updateOrderCommand.Parameters.AddWithValue("@ClientId", order.ClientId);
                updateOrderCommand.Parameters.AddWithValue("@ДатаРазмещения", order.ДатаРазмещения);
                updateOrderCommand.Parameters.AddWithValue("@Описание", order.Описание);
                updateOrderCommand.Parameters.AddWithValue("@Статус", (int)order.Статус); // Сохраняем как целое число
                updateOrderCommand.Parameters.AddWithValue("@Сумма", order.Сумма);

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
                            ДатаРазмещения = DateTime.Parse(reader["ДатаРазмещения"].ToString()),
                            Описание = reader["Описание"].ToString(),
                            Статус = (СтатусЗаказа)Convert.ToInt32(reader["Статус"]), // Преобразуем из int в enum
                            Сумма = Convert.ToDecimal(reader["Сумма"])
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
                            ДатаРазмещения = DateTime.Parse(reader["ДатаРазмещения"].ToString()),
                            Описание = reader["Описание"].ToString(),
                            Статус = (СтатусЗаказа)Convert.ToInt32(reader["Статус"]), // Преобразуем из int в enum
                            Сумма = Convert.ToDecimal(reader["Сумма"])
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