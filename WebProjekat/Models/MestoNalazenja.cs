using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebProjekat.Models
{
    public class MestoNalazenja
    {
        public string Id { get; set; }
        public string Adresa { get; set; }
        public double GeografskaSirina { get; set; }
        public double GeografskaDuzina { get; set; }

    }
}