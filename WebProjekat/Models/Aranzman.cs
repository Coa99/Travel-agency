using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebProjekat.Models
{
    [Serializable]
    public class Aranzman
    {
        public string Id { get; set; }
        public string Naziv { get; set; }
        public TipAranzmana Tip { get; set; }
        public TipPrevoza Prevoz { get; set; }
        public string Lokacija { get; set; }
        public DateTime DatumPocetka { get; set; }
        public DateTime DatumZavrsetka { get; set; }
        public MestoNalazenja MestoNalazenja { get; set; }
        public DateTime VremeNalazenja { get; set; }
        public int MaksimalanBrojPutnika { get; set; }
        public string OpisAranzmana { get; set; }
        public string OpisPutovanja { get; set; }
        public string Slika { get; set; }
        public Smestaj Smestaj { get; set; }

        public string MestoNalazenjaStr { get; set; }
        public bool Obrisan { get; set; }
        public bool Slobodan { get; set; }
        public string MenadzerId { get; set; }
        public bool Prosao { get; set; }
    }
    
}