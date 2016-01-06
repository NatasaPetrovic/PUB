using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PronadjiUBanovcima.Models
{
    public class Delatnost
    {
        public int Id { get; set; }
        public string Naziv { get; set; }
        public virtual ICollection<Info> ListaKlijenata { get; set; }

        public bool IsSelected { get; set; }
    }
}