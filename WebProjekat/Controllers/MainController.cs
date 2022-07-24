
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Xml.Serialization;
using WebProjekat.Models;

namespace WebApp.Controllers
{
    public class MainController : Controller
    {
        public void Osvezi()
        {
            XmlSerializer<Aranzman> xmlSerializer = new XmlSerializer<Aranzman>();
            List<Aranzman> sviAranzmani = xmlSerializer.Deserialize(new List<Aranzman>(), "AranzmanLista.xml");

            //List<Aranzman> predstojeciAranzmani = new List<Aranzman>();
            //List<Aranzman> prosliAranzmani = new List<Aranzman>();
            foreach (Aranzman aranzman in sviAranzmani)
            {
                if (DateTime.Compare(aranzman.DatumPocetka, DateTime.Now) <= 0)
                    aranzman.Prosao = true;
                else
                    aranzman.Prosao = false;
            }
            xmlSerializer.Serialize(sviAranzmani, "AranzmanLista.xml");

            //Session["a"] = predstojeciAranzmani;
            //Session["prosliAranzmani"] = prosliAranzmani;

            Session["a"] = sviAranzmani;
        }

        // GET: Main
        public ActionResult Index()
        {
            Osvezi();

            return View();
        }

        [HttpGet]
        public ActionResult Sortiraj(string nacin)
        {
            List<Aranzman> sortiraniAranzmani = new List<Aranzman>();

            if (nacin == "DP_Rastuce")
                sortiraniAranzmani = ((List<Aranzman>)Session["a"]).OrderBy(a => a.DatumPocetka).ToList();
            else if (nacin == "DP_Opadajuce")
                sortiraniAranzmani = ((List<Aranzman>)Session["a"]).OrderByDescending(a => a.DatumPocetka).ToList();
            else if (nacin == "DZ_Rastuce")
                sortiraniAranzmani = ((List<Aranzman>)Session["a"]).OrderBy(a => a.DatumZavrsetka).ToList();
            else if (nacin == "DZ_Opadajuce")
                sortiraniAranzmani = ((List<Aranzman>)Session["a"]).OrderByDescending(a => a.DatumZavrsetka).ToList();
            else if (nacin == "Naziv_Rastuce")
                sortiraniAranzmani = ((List<Aranzman>)Session["a"]).OrderBy(a => a.Naziv).ToList();
            else if (nacin == "Naziv_Opadajuce")
                sortiraniAranzmani = ((List<Aranzman>)Session["a"]).OrderByDescending(a => a.Naziv).ToList();

            Session["a"] = sortiraniAranzmani;

            return View("Index");
        }

        [HttpGet]
        public ActionResult Pogledaj(string id)
        {
            ViewBag.Aranzman = ((List<Aranzman>)Session["a"]).Find(x => x.Id == id);

            XmlSerializer<Komentar> xmlSerializer = new XmlSerializer<Komentar>();
            List<Komentar> sviKomentari = xmlSerializer.Deserialize(new List<Komentar>(), "KomentarLista.xml");
            Session["komentari"] = sviKomentari;

            return View("PogledajAranzman");
        }

        public ActionResult PrikaziKreirajAranzman()
        {
            if (((Korisnik)Session["korisnik"]).Uloga != Uloga.Menadzer)
                return View("Index");

            return View("KreirajAranzman");
        }

        [HttpPost]
        public ActionResult KreirajAranzman(Aranzman aranzman)
        {
            XmlSerializer<Aranzman> xmlSerializer = new XmlSerializer<Aranzman>();
            List<Aranzman> sviAranzmani = xmlSerializer.Deserialize(new List<Aranzman>(), "AranzmanLista.xml");

            XmlSerializer<Menadzer> xmlSerializerM = new XmlSerializer<Menadzer>();
            List<Menadzer> sviMenadzeri = xmlSerializerM.Deserialize(new List<Menadzer>(), "MenadzerLista.xml");

            aranzman.Id = Guid.NewGuid().ToString();
            MestoNalazenja mestoNalazenja = new MestoNalazenja() { Id = Guid.NewGuid().ToString(), Adresa = aranzman.MestoNalazenjaStr };
            aranzman.MestoNalazenja = mestoNalazenja;
            sviAranzmani.Add(aranzman);

            (sviMenadzeri.Find(m => m.Id == ((Korisnik)Session["korisnik"]).Id)).Aranzmani.Add(aranzman);

            xmlSerializer.Serialize(sviAranzmani, "AranzmanLista.xml");
            xmlSerializerM.Serialize(sviMenadzeri, "MenadzerLista.xml");

            Korisnik korisnik = sviMenadzeri.Find(t => t.Id == ((Korisnik)Session["korisnik"]).Id);
            Session["korisnik"] = korisnik;

            Osvezi();
            return View("Index");
        }

        [HttpPost]
        public ActionResult IzmeniAranzman(Aranzman noviAranzman)
        {
            XmlSerializer<Aranzman> xmlSerializer = new XmlSerializer<Aranzman>();
            List<Aranzman> sviAranzmani = xmlSerializer.Deserialize(new List<Aranzman>(), "AranzmanLista.xml");
            Aranzman aranzman = sviAranzmani.Find(a => a.Id == noviAranzman.Id);

            XmlSerializer<Turista> xmlSerializerT = new XmlSerializer<Turista>();
            List<Turista> sviTuristi = xmlSerializerT.Deserialize(new List<Turista>(), "TuristaLista.xml");

            bool mozeSeIzmeniti = true;

            foreach (Turista turista in sviTuristi)
            {
                foreach (Rezervacija rezervacija in turista.Rezervacije)
                {
                    if (aranzman.Smestaj != null)
                    {
                        foreach (SmestajnaJedinica smestajnaJedinica in aranzman.Smestaj.SmestajneJedinice)
                        {
                            if (rezervacija.SmestajnaJedinicaId == smestajnaJedinica.Id)
                            {
                                if (rezervacija.Status == StatusRezervacije.Aktivna)
                                {
                                    if (DateTime.Compare(DateTime.Parse(rezervacija.DatumPocetka), DateTime.Now) > 0)
                                    {
                                        mozeSeIzmeniti = false;
                                    }
                                }
                            }
                        }
                    }
                }
            }
            if (mozeSeIzmeniti)
            {
                aranzman.Naziv = noviAranzman.Naziv;
                aranzman.Tip = noviAranzman.Tip;
                aranzman.Prevoz = noviAranzman.Prevoz;
                aranzman.Lokacija = noviAranzman.Lokacija;
                aranzman.DatumPocetka = noviAranzman.DatumPocetka;
                aranzman.DatumZavrsetka = noviAranzman.DatumZavrsetka;
                aranzman.MestoNalazenja = noviAranzman.MestoNalazenja;
                aranzman.VremeNalazenja = noviAranzman.VremeNalazenja;
                aranzman.MaksimalanBrojPutnika = noviAranzman.MaksimalanBrojPutnika;
                aranzman.OpisAranzmana = noviAranzman.OpisAranzmana;
                aranzman.OpisPutovanja = noviAranzman.OpisPutovanja;
                aranzman.Smestaj = noviAranzman.Smestaj;
                aranzman.Slika = noviAranzman.Slika;
                aranzman.MestoNalazenjaStr = noviAranzman.MestoNalazenjaStr;
                if (aranzman.MestoNalazenja == null)
                    aranzman.MestoNalazenja = new MestoNalazenja() { Id = Guid.NewGuid().ToString(), Adresa = noviAranzman.MestoNalazenjaStr };
                else
                    aranzman.MestoNalazenja.Adresa = noviAranzman.MestoNalazenjaStr;

                xmlSerializer.Serialize(sviAranzmani, "AranzmanLista.xml");

                XmlSerializer<Menadzer> xmlSerializerM = new XmlSerializer<Menadzer>();
                List<Menadzer> sviMenadzeri = xmlSerializerM.Deserialize(new List<Menadzer>(), "MenadzerLista.xml");
                Menadzer menadzer = sviMenadzeri.Find(m => m.Id == aranzman.MenadzerId);
                menadzer.Aranzmani.RemoveAll(a => a.Id == aranzman.Id);
                menadzer.Aranzmani.Add(aranzman);
                xmlSerializerM.Serialize(sviMenadzeri, "MenadzerLista.xml");

                Session["korisnik"] = menadzer;

                Osvezi();
                return View("Index");
            }

            return View("NeuspesnaIzmena");
        }

        [HttpPost]
        public ActionResult ObrisiAranzman(string id)
        {
            XmlSerializer<Aranzman> xmlSerializer = new XmlSerializer<Aranzman>();
            List<Aranzman> sviAranzmani = xmlSerializer.Deserialize(new List<Aranzman>(), "AranzmanLista.xml");
            Aranzman aranzman = sviAranzmani.Find(a => a.Id == id);

            XmlSerializer<Turista> xmlSerializerT = new XmlSerializer<Turista>();
            List<Turista> sviTuristi = xmlSerializerT.Deserialize(new List<Turista>(), "TuristaLista.xml");

            bool mozeSeObrisati = true;

            foreach (Turista turista in sviTuristi)
            {
                foreach (Rezervacija rezervacija in turista.Rezervacije)
                {
                    foreach (SmestajnaJedinica smestajnaJedinica in aranzman.Smestaj.SmestajneJedinice)
                    {
                        if (rezervacija.SmestajnaJedinicaId == smestajnaJedinica.Id)
                        {
                            if (rezervacija.Status == StatusRezervacije.Aktivna)
                            {
                                if (DateTime.Compare(DateTime.Parse(rezervacija.DatumPocetka), DateTime.Now) > 0)
                                {
                                    mozeSeObrisati = false;
                                }
                            }
                        }
                    }
                }
            }
            if (mozeSeObrisati)
            {
                aranzman.Obrisan = true;
                xmlSerializer.Serialize(sviAranzmani, "AranzmanLista.xml");

                XmlSerializer<Menadzer> xmlSerializerM = new XmlSerializer<Menadzer>();
                List<Menadzer> sviMenadzeri = xmlSerializerM.Deserialize(new List<Menadzer>(), "MenadzerLista.xml");
                Menadzer menadzer = sviMenadzeri.Find(m => m.Id == aranzman.MenadzerId);
                menadzer.Aranzmani.Find(a => a.Id == aranzman.Id).Obrisan = true;
                xmlSerializerM.Serialize(sviMenadzeri, "MenadzerLista.xml");

                Session["korisnik"] = menadzer;

                Osvezi();
                return View("Index");
            }

            return View("NeuspesnoBrisanje");
        }

        [HttpPost]
        public ActionResult AktivirajAranzman(string id)
        {
            XmlSerializer<Aranzman> xmlSerializer = new XmlSerializer<Aranzman>();
            List<Aranzman> sviAranzmani = xmlSerializer.Deserialize(new List<Aranzman>(), "AranzmanLista.xml");
            sviAranzmani.Find(a => a.Id == id).Obrisan = false;

            XmlSerializer<Menadzer> xmlSerializerM = new XmlSerializer<Menadzer>();
            List<Menadzer> sviMenadzeri = xmlSerializerM.Deserialize(new List<Menadzer>(), "MenadzerLista.xml");

            Menadzer vlasnik = null;
            foreach (Menadzer menadzer in sviMenadzeri)
            {
                foreach (Aranzman ma in menadzer.Aranzmani)
                {
                    if (ma.Id == id)
                    {
                        vlasnik = menadzer;
                        ma.Obrisan = false;
                        break;
                    }
                }
            }

            xmlSerializer.Serialize(sviAranzmani, "AranzmanLista.xml");
            xmlSerializerM.Serialize(sviMenadzeri, "MenadzerLista.xml");
            Session["korisnik"] = vlasnik;

            Osvezi();
            return View("Index");
        }

        public ActionResult PrikaziKreirajSmestaj()
        {
            if (((Korisnik)Session["korisnik"]).Uloga != Uloga.Menadzer)
                return View("Index");

            return View("KreirajSmestaj");
        }

        [HttpPost]
        public ActionResult KreirajSmestaj(Smestaj smestaj)
        {
            smestaj.Id = Guid.NewGuid().ToString();

            XmlSerializer<Aranzman> xmlSerializerAranzman = new XmlSerializer<Aranzman>();
            List<Aranzman> sviAranzmani = xmlSerializerAranzman.Deserialize(new List<Aranzman>(), "AranzmanLista.xml");
            Aranzman aranzman = sviAranzmani.Find(a => a.Id == smestaj.AranzmanId);
            aranzman.Smestaj = smestaj;
            xmlSerializerAranzman.Serialize(sviAranzmani, "AranzmanLista.xml");

            Osvezi();
            return View("Index");
        }

        [HttpPost]
        public ActionResult KreirajSmestajnuJedinicu(SmestajnaJedinica smestajnaJedinica)
        {
            smestajnaJedinica.Id = Guid.NewGuid().ToString();
            smestajnaJedinica.Slobodan = true;
            smestajnaJedinica.Obrisan = false;

            XmlSerializer<Aranzman> xmlSerializerAranzman = new XmlSerializer<Aranzman>();
            List<Aranzman> sviAranzmani = xmlSerializerAranzman.Deserialize(new List<Aranzman>(), "AranzmanLista.xml");
            Aranzman aranzman = sviAranzmani.Find(a => a.Smestaj != null && a.Smestaj.Id == smestajnaJedinica.SmestajId);
            aranzman.Smestaj.SmestajneJedinice.Add(smestajnaJedinica);
            xmlSerializerAranzman.Serialize(sviAranzmani, "AranzmanLista.xml");

            Osvezi();
            return View("Index");
        }
        [HttpPost]
        public ActionResult ObrisiSmestajnuJedinicu(string id)
        {
            XmlSerializer<Aranzman> xmlSerializerAranzman = new XmlSerializer<Aranzman>();
            List<Aranzman> sviAranzmani = xmlSerializerAranzman.Deserialize(new List<Aranzman>(), "AranzmanLista.xml");
            Aranzman aranzman = sviAranzmani.Find(a => a.Smestaj.SmestajneJedinice.Find(sj => sj.Id == id) != null);
            SmestajnaJedinica smestajnaJedinica = null;
            foreach (SmestajnaJedinica sj in aranzman.Smestaj.SmestajneJedinice)
                if (sj.Id == id)
                    smestajnaJedinica = sj;

            XmlSerializer<Turista> xmlSerializerT = new XmlSerializer<Turista>();
            List<Turista> sviTuristi = xmlSerializerT.Deserialize(new List<Turista>(), "TuristaLista.xml");

            bool mozeSeObrisati = true;
            foreach (Turista turista in sviTuristi)
            {
                foreach (Rezervacija rezervacija in turista.Rezervacije)
                {
                    if (rezervacija.SmestajnaJedinicaId == smestajnaJedinica.Id)
                    {
                        if (DateTime.Compare(DateTime.Parse(rezervacija.DatumPocetka), DateTime.Now) > 0)
                        {
                            mozeSeObrisati = false;
                        }
                    }
                }
            }

            if (mozeSeObrisati)
            {
                smestajnaJedinica.Obrisan = true;
                xmlSerializerAranzman.Serialize(sviAranzmani, "AranzmanLista.xml");

                Osvezi();
                return View("Index");
            }

            return View("NeuspesnoBrisanje");
        }

        [HttpPost]
        public ActionResult IzmeniSmestajnuJedinicu(SmestajnaJedinica novaSmestajnaJedinica)
        {
            XmlSerializer<Aranzman> xmlSerializerAranzman = new XmlSerializer<Aranzman>();
            List<Aranzman> sviAranzmani = xmlSerializerAranzman.Deserialize(new List<Aranzman>(), "AranzmanLista.xml");
            Aranzman aranzman = sviAranzmani.Find(a => a.Smestaj != null && a.Smestaj.SmestajneJedinice.Find(sj => sj.Id == novaSmestajnaJedinica.Id) != null);
            SmestajnaJedinica smestajnaJedinica = null;
            foreach (SmestajnaJedinica sj in aranzman.Smestaj.SmestajneJedinice)
                if (sj.Id == novaSmestajnaJedinica.Id)
                    smestajnaJedinica = sj;

            XmlSerializer<Turista> xmlSerializerT = new XmlSerializer<Turista>();
            List<Turista> sviTuristi = xmlSerializerT.Deserialize(new List<Turista>(), "TuristaLista.xml");

            bool mozeSeIzmeniti = true;
            foreach (Turista turista in sviTuristi)
            {
                foreach (Rezervacija rezervacija in turista.Rezervacije)
                {
                    if (rezervacija.SmestajnaJedinicaId == smestajnaJedinica.Id)
                    {
                        if (DateTime.Compare(DateTime.Parse(rezervacija.DatumPocetka), DateTime.Now) > 0)
                        {
                            mozeSeIzmeniti = false;
                        }
                    }
                }
            }

            if (mozeSeIzmeniti)
            {
                smestajnaJedinica.DozvoljenBrojGostiju = novaSmestajnaJedinica.DozvoljenBrojGostiju;
                smestajnaJedinica.DozvoljenBoravakKucnihLjubimaca = novaSmestajnaJedinica.DozvoljenBoravakKucnihLjubimaca;
                smestajnaJedinica.Cena = novaSmestajnaJedinica.Cena;

                xmlSerializerAranzman.Serialize(sviAranzmani, "AranzmanLista.xml");

                Osvezi();
                return View("Index");
            }

            return View("NeuspesnaIzmena");
        }

        public ActionResult NeuspesnoBrisanje()
        {
            return View();
        }

        public ActionResult NeuspesnaIzmena()
        {
            return View();
        }

        [HttpPost]
        public ActionResult ObrisiSmestaj(string id)
        {
            XmlSerializer<Aranzman> xmlSerializerAranzman = new XmlSerializer<Aranzman>();
            List<Aranzman> sviAranzmani = xmlSerializerAranzman.Deserialize(new List<Aranzman>(), "AranzmanLista.xml");
            Aranzman aranzman = sviAranzmani.Find(a => a.Smestaj.Id == id);

            XmlSerializer<Turista> xmlSerializerT = new XmlSerializer<Turista>();
            List<Turista> sviTuristi = xmlSerializerT.Deserialize(new List<Turista>(), "TuristaLista.xml");

            bool mozeSeObrisati = true;

            foreach (Turista turista in sviTuristi)
            {
                foreach (Rezervacija rezervacija in turista.Rezervacije)
                {
                    if (rezervacija.Status == StatusRezervacije.Aktivna)
                    {
                        if (rezervacija.AranzmanId == aranzman.Id)
                        {
                            if (DateTime.Compare(DateTime.Parse(rezervacija.DatumPocetka), DateTime.Now) > 0)
                            {
                                mozeSeObrisati = false;
                            }
                        }
                    }
                }
            }

            if (mozeSeObrisati)
            {
                aranzman.Smestaj.Obrisan = true;
                xmlSerializerAranzman.Serialize(sviAranzmani, "AranzmanLista.xml");

                Osvezi();
                return View("Index");
            }
            return View("NeuspesnoBrisanje");
        }

        [HttpPost]
        public ActionResult IzmeniSmestaj(Smestaj noviSmestaj)
        {
            XmlSerializer<Aranzman> xmlSerializerAranzman = new XmlSerializer<Aranzman>();
            List<Aranzman> sviAranzmani = xmlSerializerAranzman.Deserialize(new List<Aranzman>(), "AranzmanLista.xml");
            Aranzman aranzman = sviAranzmani.Find(a => a.Smestaj != null && a.Smestaj.Id == noviSmestaj.Id);

            XmlSerializer<Turista> xmlSerializerT = new XmlSerializer<Turista>();
            List<Turista> sviTuristi = xmlSerializerT.Deserialize(new List<Turista>(), "TuristaLista.xml");

            bool mozeSeObrisati = true;

            foreach (Turista turista in sviTuristi)
            {
                foreach (Rezervacija rezervacija in turista.Rezervacije)
                {
                    if (rezervacija.Status == StatusRezervacije.Aktivna)
                    {
                        if (rezervacija.AranzmanId == aranzman.Id)
                        {
                            if (DateTime.Compare(DateTime.Parse(rezervacija.DatumPocetka), DateTime.Now) > 0)
                            {
                                mozeSeObrisati = false;
                            }
                        }
                    }
                }
            }

            if (mozeSeObrisati)
            {
                aranzman.Smestaj.Tip = noviSmestaj.Tip;
                aranzman.Smestaj.Naziv = noviSmestaj.Naziv;
                aranzman.Smestaj.BrojZvezdica = noviSmestaj.BrojZvezdica;
                aranzman.Smestaj.PostojanjeBazena = noviSmestaj.PostojanjeBazena;
                aranzman.Smestaj.PostojanjeSpaCentra = noviSmestaj.PostojanjeSpaCentra;
                aranzman.Smestaj.PrilagodjenoZaOsobeSaInvaliditetom = noviSmestaj.PrilagodjenoZaOsobeSaInvaliditetom;
                aranzman.Smestaj.PostojanjeWiFiKonekcije = noviSmestaj.PostojanjeWiFiKonekcije;

                xmlSerializerAranzman.Serialize(sviAranzmani, "AranzmanLista.xml");

                Osvezi();
                return View("Index");
            }
            return View("NeuspesnoBrisanje");
        }

        [HttpPost]
        public ActionResult AktivirajSmestaj(string id)
        {
            XmlSerializer<Aranzman> xmlSerializerAranzman = new XmlSerializer<Aranzman>();
            List<Aranzman> sviAranzmani = xmlSerializerAranzman.Deserialize(new List<Aranzman>(), "AranzmanLista.xml");

            sviAranzmani.Find(a => a.Smestaj.Id == id).Smestaj.Obrisan = false;
            xmlSerializerAranzman.Serialize(sviAranzmani, "AranzmanLista.xml");
            Session["a"] = sviAranzmani;

            return View("Index");
        }

        [HttpPost]
        public ActionResult PretraziAranzmane(DateTime? minPocetak, DateTime? maxPocetak, DateTime? minKraj, DateTime? maxKraj, string tipPrevoza, string tipAranzmana, string naziv)
        {
            XmlSerializer<Aranzman> xmlSerializerAranzman = new XmlSerializer<Aranzman>();
            List<Aranzman> sviAranzmani = xmlSerializerAranzman.Deserialize(new List<Aranzman>(), "AranzmanLista.xml");
            List<Aranzman> odabraniAranzmani = new List<Aranzman>();

            odabraniAranzmani = ((sviAranzmani.Where(a => (Enum.TryParse(tipPrevoza, out TipPrevoza tipP) ? a.Prevoz == tipP : true)
                && (Enum.TryParse(tipAranzmana, out TipAranzmana tipA) ? a.Tip == tipA : true)
                && (a.DatumPocetka >= (minPocetak ?? DateTime.MinValue))
                && (a.DatumPocetka <= (maxPocetak ?? DateTime.MaxValue))
                && (a.DatumZavrsetka >= (minKraj ?? DateTime.MinValue))
                && (a.DatumZavrsetka <= (maxKraj ?? DateTime.MaxValue))
                && (!String.IsNullOrWhiteSpace(naziv) ? a.Naziv.Contains(naziv) : true)))).ToList();

            Session["a"] = odabraniAranzmani;

            return View("Index");
        }

        [HttpPost]
        public ActionResult PretraziSmestajneJedinice(string id, int? minBrojGostiju, int? maxBrojGostiju, bool? kucniLjubimci, double? cena)
        {
            XmlSerializer<Aranzman> xmlSerializerAranzman = new XmlSerializer<Aranzman>();
            List<Aranzman> sviAranzmani = xmlSerializerAranzman.Deserialize(new List<Aranzman>(), "AranzmanLista.xml");

            Aranzman aranzman = sviAranzmani.Find(a => a.Id == id);
            List<SmestajnaJedinica> odabraneSmestajneJedinice = new List<SmestajnaJedinica>();

            odabraneSmestajneJedinice = aranzman.Smestaj.SmestajneJedinice.Where(sj => (sj.DozvoljenBrojGostiju > (minBrojGostiju ?? int.MinValue))
            && (sj.DozvoljenBrojGostiju < (maxBrojGostiju ?? int.MaxValue))
            && (kucniLjubimci != null ? sj.DozvoljenBoravakKucnihLjubimaca == kucniLjubimci : true)
            && (sj.Cena < (cena ?? double.MaxValue))).ToList();

            aranzman.Smestaj.SmestajneJedinice = odabraneSmestajneJedinice;

            ViewBag.Aranzman = aranzman;

            return View("PogledajAranzman");
        }

        [HttpGet]
        public ActionResult SortirajSmestajneJedinice(string id, string nacin)
        {
            XmlSerializer<Aranzman> xmlSerializerAranzman = new XmlSerializer<Aranzman>();
            List<Aranzman> sviAranzmani = xmlSerializerAranzman.Deserialize(new List<Aranzman>(), "AranzmanLista.xml");

            Aranzman aranzman = sviAranzmani.Find(a => a.Id == id);
            List<SmestajnaJedinica> sortiraneSmestajneJedinice = new List<SmestajnaJedinica>();

            if (nacin == "DBG_Rastuce")
            {
                sortiraneSmestajneJedinice = aranzman.Smestaj.SmestajneJedinice.OrderBy(sj => sj.DozvoljenBrojGostiju).ToList();
            }
            else if (nacin == "DBG_Opadajuce")
            {
                sortiraneSmestajneJedinice = aranzman.Smestaj.SmestajneJedinice.OrderByDescending(sj => sj.DozvoljenBrojGostiju).ToList();
            }
            else if (nacin == "cena_Rastuce")
            {
                sortiraneSmestajneJedinice = aranzman.Smestaj.SmestajneJedinice.OrderBy(sj => sj.Cena).ToList();
            }
            else if (nacin == "cena_Opadajuce")
            {
                sortiraneSmestajneJedinice = aranzman.Smestaj.SmestajneJedinice.OrderByDescending(sj => sj.Cena).ToList();
            }

            aranzman.Smestaj.SmestajneJedinice = sortiraneSmestajneJedinice;

            ViewBag.Aranzman = aranzman;

            return View("PogledajAranzman");
        }

        [HttpPost]
        public ActionResult PretraziSmestaje(string tip, bool? bazen, bool? spa, bool? osobeSaInvaliditetom, bool? wifi, string naziv)
        {
            XmlSerializer<Aranzman> xmlSerializerAranzman = new XmlSerializer<Aranzman>();
            List<Aranzman> sviAranzmani = xmlSerializerAranzman.Deserialize(new List<Aranzman>(), "AranzmanLista.xml");

            List<Aranzman> odabraniSmestaji = new List<Aranzman>();
            //bool b = Enum.TryParse(tip, out TipSmestaja tipp    );
            //odabraniSmestaji = sviAranzmani.Where(a => a.Smestaj== null ||(
            //b ? a.Smestaj.Tip == tipp : true
            //&& bazen != null ? a.Smestaj.PostojanjeBazena == bazen : true
            //&& spa != null ? a.Smestaj.PostojanjeSpaCentra == spa : true
            //&& (!String.IsNullOrWhiteSpace(naziv) ? a.Smestaj.Naziv.Contains(naziv):true)
            //&& osobeSaInvaliditetom != null ? a.Smestaj.PrilagodjenoZaOsobeSaInvaliditetom == osobeSaInvaliditetom : true
            //&& wifi != null ? a.Smestaj.PostojanjeWiFiKonekcije == wifi : true)).ToList();

            bool b = Enum.TryParse(tip, out TipSmestaja tipp);
            odabraniSmestaji = sviAranzmani.Where(a => a.Smestaj != null && (
            b ? a.Smestaj.Tip == tipp : true
            && bazen != null ? a.Smestaj.PostojanjeBazena == bazen : true
            && spa != null ? a.Smestaj.PostojanjeSpaCentra == spa : true
            && (!String.IsNullOrWhiteSpace(naziv) ? a.Smestaj.Naziv.Contains(naziv) : true)
            && osobeSaInvaliditetom != null ? a.Smestaj.PrilagodjenoZaOsobeSaInvaliditetom == osobeSaInvaliditetom : true
            && wifi != null ? a.Smestaj.PostojanjeWiFiKonekcije == wifi : true)).ToList();


            Session["a"] = odabraniSmestaji;

            return View("Index");
        }

        [HttpGet]
        public ActionResult SortirajSmestaje(string nacin)
        {
            XmlSerializer<Aranzman> xmlSerializerAranzman = new XmlSerializer<Aranzman>();
            List<Aranzman> sviAranzmani = xmlSerializerAranzman.Deserialize(new List<Aranzman>(), "AranzmanLista.xml");

            List<Aranzman> sviAranzmaniSaSmestajima = sviAranzmani.Where(a => a.Smestaj != null).ToList();
            List<Aranzman> sortiraniSmestaji = new List<Aranzman>();

            if (nacin == "naziv_Rastuce")
            {
                sortiraniSmestaji = sviAranzmaniSaSmestajima.OrderBy(a => a.Smestaj.Naziv).ToList();
            }
            else if (nacin == "naziv_Opadajuce")
            {
                sortiraniSmestaji = sviAranzmaniSaSmestajima.OrderByDescending(a => a.Smestaj.Naziv).ToList();
            }
            else if (nacin == "bs_Rastuce")
            {
                sortiraniSmestaji = sviAranzmaniSaSmestajima.OrderBy(a => a.Smestaj.SmestajneJedinice.Count()).ToList();
            }
            else if (nacin == "bs_Opadajuce")
            {
                sortiraniSmestaji = sviAranzmaniSaSmestajima.OrderByDescending(a => a.Smestaj.SmestajneJedinice.Count()).ToList();
            }
            else if (nacin == "bss_Rastuce")
            {
                sortiraniSmestaji = sviAranzmaniSaSmestajima.OrderBy(a => a.Smestaj.SmestajneJedinice.Where(sj => sj.Slobodan).Count()).ToList();
            }
            else if (nacin == "bss_Opadajuce")
            {
                sortiraniSmestaji = sviAranzmaniSaSmestajima.OrderByDescending(a => a.Smestaj.SmestajneJedinice.Where(sj => sj.Slobodan).Count()).ToList();
            }

            Session["a"] = sortiraniSmestaji;

            return View("Index");
        }
    }
}