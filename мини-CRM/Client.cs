using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace мини_CRM
{
    public class Client
    {
        public int Id { get; set; }
        public string Imya { get; set; }
        public string Familiya { get; set; }
        public string Otchestvo { get; set; }
        public string Email { get; set; }
        public string Telephon { get; set; }
        public string Adress { get; set; }
        public string Kompaniya { get; set; }
        public string Dolznost { get; set; }
        public DateTime DataRegistracii { get; set; } = DateTime.Now;
        public string Dop_inf { get; set; }

        // Конструктор по умолчанию (необходим для сериализации/десериализации)
        public Client() { }

        public override string ToString()
        {
            return $"{Familiya} {Imya} {Otchestvo}"; // Как будет отображаться клиент в ListBox
        }
    }
}