using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebProjekat.Models
{
    public class Menadzer : Korisnik
    {
        public List<Aranzman> Aranzmani = new List<Aranzman>();
    }
}