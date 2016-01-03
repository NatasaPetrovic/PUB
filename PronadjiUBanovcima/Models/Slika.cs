using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PronadjiUBanovcima.Models
{
    public class Slika
    {
        public int Id { get; set; }
        public string Putanja { get; set; }
        public string Alt { get; set; }

        public int KlijentId { get; set; }

        public virtual Info Klijent { get; set; }
    }
}