using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace мини_CRM
{
    public enum OrderStatus
    {
        Prinyat,
        V_obrabotke,
        Sobran,
        Otpravlen,
        Dostavlen,
        Otmenen
    }

    public class Order  // <-- Изменили internal на public
    {
        public int Id { get; set; }
        public int ClientId { get; set; } // ID клиента, которому принадлежит заказ
        public DateTime DataRazmesheniya { get; set; } = DateTime.Now;
        public string Opisanie { get; set; }
        public OrderStatus Status { get; set; }
        public decimal Summa { get; set; }
        public List<string> Tovari { get; set; } = new List<string>();

        public Client Client { get; set; } // Ссылка на объект Client (для удобства)

        // Конструктор по умолчанию (необходим для сериализации/десериализации)
        public Order() { }

        public override string ToString()
        {
            return $"Заказ #{Id} - {Opisanie} - {Status}";
        }
    }
}