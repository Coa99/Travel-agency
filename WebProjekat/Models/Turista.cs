using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebProjekat.Models
{
    public class Turista : Korisnik
    {
        public List<Rezervacija> Rezervacije = new List<Rezervacija>();

        public int BrojOtkazivanja { get; set; }
        public bool Blokiran { get; set; }
    }
}