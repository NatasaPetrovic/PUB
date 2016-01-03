using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PronadjiUBanovcima.Models
{
    public class Tag
    {
        public int Id { get; set; }
        public string Naziv { get; set; }
        public virtual ICollection<Info> ListaKlijenata { get; set; }
        public Tag()
        {
            this.ListaKlijenata = new List<Info>();
        }
    }
}