using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebProjekat.Models
{
    public class Komentar
    {
        public string Id { get; set; }
        public string TuristaId { get; set; }
        public string Tekst { get; set; }
        public int Ocena { get; set; }

        public string ApartmanId { get; set; }
        public bool Odobren { get; set; }
        public bool Odbijen { get; set; }
        public string RezervacijaId { get; set; }
    }
}