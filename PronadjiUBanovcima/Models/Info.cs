using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PronadjiUBanovcima.Models
{
    public class Info
    {
        
        public int Id { get; set; }
        public string Firma { get; set; }
       // public string Opis { get; set; }
        public string Telefon { get; set; }
        public string Adresa { get; set; }
       // public string Sajt { get; set; }

        public string Email { get; set; }
        public virtual ApplicationUser Klijent { get; set; }


        public virtual List<Slika> ListaSlika { get; set; }
        public virtual List<Delatnost> ListaDelatnosti { get; set; }
        public virtual List<Tag> ListaTagova { get; set; }
    }
}