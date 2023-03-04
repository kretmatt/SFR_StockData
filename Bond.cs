using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace producer
{
    public class Bond
    {
        public string Bondname { get; set; }
        public int Price { get; set; }
        public string TimeStamp { get; set; }

        public Bond(string name, int price, string time)
        {
            this.Bondname = name;
            this.Price = price;
            this.TimeStamp= time;
        }
    } 
}
