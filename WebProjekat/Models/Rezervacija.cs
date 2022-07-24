using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebProjekat.Models
{
    public class Rezervacija
    {
        public string Id { get; set; }
        public string TuristaId { get; set; }
        public StatusRezervacije Status { get; set; }
        public string AranzmanId { get; set; }
        public string SmestajnaJedinicaId { get; set; }

        public string NazivAranzmana { get; set; }
        public string LokacijaAranzmana { get; set; }
        public string DatumPocetka { get; set; }
        public string DatumZavrsetka { get; set; }
        public double Cena { get; set; }

        public bool MozeSeOtkazati { get; set; }
        public bool MozeSeKomentarisati { get; set; }
        public bool Komentarisano { get; set; }
    }
}