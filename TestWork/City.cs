using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestWork
{
    /// <summary>
    /// Класс, описывающий JSON ответ на запрос получения координат города
    /// </summary>
    public class City
    {
        public string name { get; set; }
        public Dictionary<string, string> local_names { get; set; }
        public float lat { get; set; }
        public float lon { get; set; }
        public string country { get; set; }
        public string state { get; set; }
    }
}
