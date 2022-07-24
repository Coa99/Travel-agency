using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebProjekat.Models
{
    public class Smestaj
    {
        public string Id { get; set; }
        public TipSmestaja Tip { get; set; }
        public string Naziv { get; set; }
        public int BrojZvezdica { get; set; }
        public bool PostojanjeBazena { get; set; }
        public bool PostojanjeSpaCentra { get; set; }
        public bool PrilagodjenoZaOsobeSaInvaliditetom { get; set; }
        public bool PostojanjeWiFiKonekcije { get; set; }

        public List<SmestajnaJedinica> SmestajneJedinice = new List<SmestajnaJedinica>();

        public bool Obrisan { get; set; }
        public bool Slobodan { get; set; }
        public string AranzmanId { get; set; }
    }
}