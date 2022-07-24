using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebProjekat.Models
{
    public class SmestajnaJedinica
    {
        public string Id { get; set; }
        public int DozvoljenBrojGostiju { get; set; }
        public bool DozvoljenBoravakKucnihLjubimaca { get; set; }
        public double Cena { get; set; }

        public bool Obrisan { get; set; }
        public bool Slobodan { get; set; }
        public string SmestajId { get; set; }
    }
}