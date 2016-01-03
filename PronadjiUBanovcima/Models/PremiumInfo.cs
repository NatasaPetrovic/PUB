using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PronadjiUBanovcima.Models
{
    public class PremiumInfo
    {
        public int Id { get; set; }
        public string Ime { get; set; }
        public string Prezime { get; set; }
        public string Email { get; set; }
        public string Telefon { get; set; }
        public string Opis { get; set; }
        public string Sajt { get; set; }
        public virtual ApplicationUser Klijent { get; set; }
    }
}