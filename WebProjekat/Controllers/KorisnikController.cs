using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebProjekat.Models;

namespace WebApp.Controllers
{
    public class KorisnikController : Controller
    {
        // GET: Korisnik
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Logovanje()
        {
            return View();
        }

        [HttpPost]
        public ActionResult LogovanjeKorisnika(LoginModel loginModel)
        {
            XmlSerializer<Turista> xmlSerializerT = new XmlSerializer<Turista>();
            XmlSerializer<Menadzer> xmlSerializerM = new XmlSerializer<Menadzer>();
            XmlSerializer<Korisnik> xmlSerializerA = new XmlSerializer<Korisnik>();
            List<Turista> sviTuristi = xmlSerializerT.Deserialize(new List<Turista>(), "TuristaLista.xml");
            List<Menadzer> sviMenadzeri = xmlSerializerM.Deserialize(new List<Menadzer>(), "MenadzerLista.xml");
            List<Korisnik> sviAdmini = xmlSerializerA.Deserialize(new List<Korisnik>(), "AdminLista.xml");

            List<Korisnik> sviKorisnici = new List<Korisnik>();
            foreach (Turista turista in sviTuristi)
                sviKorisnici.Add(turista);
            foreach (Menadzer menadzer in sviMenadzeri)
                sviKorisnici.Add(menadzer);
            foreach (Korisnik admin in sviAdmini)
                sviKorisnici.Add(admin);

            Korisnik korisnik = null;
            foreach (Korisnik k in sviKorisnici)
            {
                if (k.KorisnickoIme == loginModel.KorisnickoIme)
                {
                    korisnik = k;
                    break;
                }
            }

            if (korisnik == null)
                return View("NepostojeciKorisnik");

            if (korisnik.Lozinka != loginModel.Lozinka)
                return View("PogresnaLozinka");

            if (korisnik.Uloga == Uloga.Turista)
            {
                Turista turista = sviTuristi.Find(t => t.Id == korisnik.Id);
                if (turista.Blokiran)
                {
                    return View("BlokiranNalog");
                }
            }

            Session["korisnik"] = korisnik;

            return RedirectToAction("Index", "Main");
        }

        public ActionResult NepostojeciKorisnik()
        {
            return View();
        }

        public ActionResult BlokiranNalog()
        {
            return View();
        }

        public ActionResult PogresnaLozinka()
        {
            return View();
        }

        public ActionResult Registracija()
        {
            return View();
        }

        public ActionResult Logout()
        {
            Session["korisnik"] = null;
            return RedirectToAction("Logovanje", "Korisnik");
        }

        [HttpPost]
        public ActionResult RegistrovanjeKorisnika(Turista turista)
        {
            XmlSerializer<Turista> xmlSerializer = new XmlSerializer<Turista>();
            List<Turista> sviTuristi = xmlSerializer.Deserialize(new List<Turista>(), "TuristaLista.xml");

            foreach (Turista t in sviTuristi)
                if (t.KorisnickoIme == turista.KorisnickoIme)
                    return View("NeuspesnaRegistracija");


            turista.Id = Guid.NewGuid().ToString();
            turista.Uloga = Uloga.Turista;
            turista.Blokiran = false;
            sviTuristi.Add(turista);
            xmlSerializer.Serialize(sviTuristi, "TuristaLista.xml");

            return View("UspesnaRegistracija");
        }

        public ActionResult Profil()
        {
            if (Session["korisnik"] == null)
                return RedirectToAction("Logovanje", "Korisnik");

            if (((Korisnik)Session["korisnik"]).Uloga == Uloga.Turista)
                ProveriRezervacije();

            //if (((Korisnik)Session["korisnik"]).Uloga == Uloga.Menadzer)
            //{
            //    XmlSerializer<Menadzer> xmlSerializerM = new XmlSerializer<Menadzer>();
            //    List<Menadzer> sviMenadzeri = xmlSerializerM.Deserialize(new List<Menadzer>(), "MenadzerLista.xml");
            //    Menadzer menadzer = sviMenadzeri.Find(m => m.Id == ((Korisnik)Session["korisnik"]).Id);

            //    XmlSerializer<Komentar> xmlSerializer = new XmlSerializer<Komentar>();
            //    List<Komentar> sviKomentari = xmlSerializer.Deserialize(new List<Komentar>(), "KomentarLista.xml");

            //    List<Komentar> komentariNaAranzmaneMenadzera = new List<Komentar>();
            //    foreach (Aranzman aranzman in menadzer.Aranzmani)
            //    {
            //        foreach(Komentar komentar in sviKomentari)
            //        {
            //            if(aranzman.Id == komentar.ApartmanId)
            //            {
            //                komentariNaAranzmaneMenadzera.Add(komentar);
            //            }
            //        }
            //    }

            //    Session["komentari"] = komentariNaAranzmaneMenadzera;
            //}

            if (((Korisnik)Session["korisnik"]).Uloga == Uloga.Menadzer)
            {
                XmlSerializer<Turista> xmlSerializerT = new XmlSerializer<Turista>();
                List<Turista> sviTuristi = xmlSerializerT.Deserialize(new List<Turista>(), "TuristaLista.xml");

                XmlSerializer<Menadzer> xmlSerializerM = new XmlSerializer<Menadzer>();
                List<Menadzer> sviMenadzeri = xmlSerializerM.Deserialize(new List<Menadzer>(), "MenadzerLista.xml");
                Menadzer menadzer = sviMenadzeri.Find(m => m.Id == ((Korisnik)Session["korisnik"]).Id);

                XmlSerializer<Aranzman> xmlSerializerA = new XmlSerializer<Aranzman>();
                List<Aranzman> sviAranzmani = xmlSerializerA.Deserialize(new List<Aranzman>(), "AranzmanLista.xml");

                List<Rezervacija> rezervacije = new List<Rezervacija>();
                //foreach(Turista turista in sviTuristi)
                //{
                //    foreach(Rezervacija rezervacija in turista.Rezervacije)
                //    {
                //        Aranzman aranzman = sviAranzmani.Find(a => a.Id == rezervacija.AranzmanId);
                //        if(aranzman.MenadzerId == menadzer.Id)
                //        {
                //            rezervacije.Add(rezervacija);
                //        }    
                //    }
                //}
                //Session["rezervacije"] = rezervacije;

                foreach (Aranzman aranzman in menadzer.Aranzmani)
                {
                    foreach (Turista turista in sviTuristi)
                    {
                        foreach (Rezervacija rezervacija in turista.Rezervacije)
                        {
                            if (aranzman.Id == rezervacija.AranzmanId)
                            {
                                rezervacije.Add(rezervacija);
                            }
                        }
                    }
                }
                Session["rezervacije"] = rezervacije;
            }

            XmlSerializer<Komentar> xmlSerializer = new XmlSerializer<Komentar>();
            List<Komentar> sviKomentari = xmlSerializer.Deserialize(new List<Komentar>(), "KomentarLista.xml");
            Session["komentari"] = sviKomentari;

            return View();
        }

        [HttpPost]
        public ActionResult PotvrdiIzmene(Korisnik korisnik)
        {
            if (korisnik.Uloga == Uloga.Turista)
            {
                XmlSerializer<Turista> xmlSerializer = new XmlSerializer<Turista>();
                List<Turista> sviTuristi = xmlSerializer.Deserialize(new List<Turista>(), "TuristaLista.xml");
                Turista turista = sviTuristi.Find(t => t.Id == korisnik.Id);
                turista.Lozinka = korisnik.Lozinka;
                turista.Ime = korisnik.Ime;
                turista.Prezime = korisnik.Prezime;
                turista.Email = korisnik.Email;
                xmlSerializer.Serialize(sviTuristi, "TuristaLista.xml");
                Session["korisnik"] = turista;
            }
            else if (korisnik.Uloga == Uloga.Menadzer)
            {
                XmlSerializer<Menadzer> xmlSerializer = new XmlSerializer<Menadzer>();
                List<Menadzer> sviMenadzeri = xmlSerializer.Deserialize(new List<Menadzer>(), "MenadzerLista.xml");
                Menadzer menadzer = sviMenadzeri.Find(m => m.Id == korisnik.Id);
                menadzer.Lozinka = korisnik.Lozinka;
                menadzer.Ime = korisnik.Ime;
                menadzer.Prezime = korisnik.Prezime;
                menadzer.Email = korisnik.Email;
                xmlSerializer.Serialize(sviMenadzeri, "MenadzerLista.xml");
                Session["korisnik"] = menadzer;
            }
            else if (korisnik.Uloga == Uloga.Administrator)
            {
                XmlSerializer<Korisnik> xmlSerializer = new XmlSerializer<Korisnik>();
                List<Korisnik> sviAdmini = xmlSerializer.Deserialize(new List<Korisnik>(), "AdminLista.xml");
                Korisnik admin = sviAdmini.Find(a => a.Id == korisnik.Id);
                admin.Lozinka = korisnik.Lozinka;
                admin.Ime = korisnik.Ime;
                admin.Prezime = korisnik.Prezime;
                admin.Email = korisnik.Email;
                xmlSerializer.Serialize(sviAdmini, "AdminLista.xml");
                Session["korisnik"] = admin;
            }

            return View("Profil");
        }

        public void ProveriRezervacije()
        {
            XmlSerializer<Turista> xmlSerializerT = new XmlSerializer<Turista>();
            List<Turista> sviTuristi = xmlSerializerT.Deserialize(new List<Turista>(), "TuristaLista.xml");

            foreach (Turista turista in sviTuristi)
            {
                foreach (Rezervacija rezervacija in turista.Rezervacije)
                {
                    if (DateTime.Compare(DateTime.Now, DateTime.Parse(rezervacija.DatumPocetka)) < 0)
                        rezervacija.MozeSeOtkazati = true;
                    if (DateTime.Compare(DateTime.Now, DateTime.Parse(rezervacija.DatumZavrsetka)) > 0)
                        rezervacija.MozeSeKomentarisati = true;
                }
            }
            xmlSerializerT.Serialize(sviTuristi, "TuristaLista.xml");
            Korisnik korisnik = sviTuristi.Find(t => t.Id == ((Korisnik)Session["korisnik"]).Id);
            Session["korisnik"] = korisnik;
        }

        public ActionResult RegistracijaMenadzera()
        {
            return View();
        }

        [HttpPost]
        public ActionResult RegistrujMenadzera(Menadzer menadzer)
        {
            XmlSerializer<Menadzer> xmlSerializer = new XmlSerializer<Menadzer>();
            List<Menadzer> sviMenadzeri = xmlSerializer.Deserialize(new List<Menadzer>(), "MenadzerLista.xml");

            foreach (Menadzer m in sviMenadzeri)
                if (m.KorisnickoIme == menadzer.KorisnickoIme)
                    return View("NeuspesnaRegistracija");

            menadzer.Id = Guid.NewGuid().ToString();
            menadzer.Uloga = Uloga.Menadzer;
            sviMenadzeri.Add(menadzer);
            xmlSerializer.Serialize(sviMenadzeri, "MenadzerLista.xml");

            return View("UspesnaRegistracija");
        }

        public ActionResult PrikazKorisnika()
        {
            return View();
        }

        [HttpGet]
        public ActionResult PrikaziKorisnike()
        {
            if (Session["korisnik"] == null || ((Korisnik)Session["korisnik"]).Uloga != Uloga.Administrator)
                return View("Index");

            XmlSerializer<Turista> xmlSerializerT = new XmlSerializer<Turista>();
            List<Turista> sviTuristi = xmlSerializerT.Deserialize(new List<Turista>(), "TuristaLista.xml");

            XmlSerializer<Menadzer> xmlSerializerM = new XmlSerializer<Menadzer>();
            List<Menadzer> sviMenadzeri = xmlSerializerM.Deserialize(new List<Menadzer>(), "MenadzerLista.xml");

            Session["turisti"] = sviTuristi;
            Session["menadzeri"] = sviMenadzeri;

            return View("PrikazKorisnika");
        }

        [HttpPost]
        public ActionResult BlokirajKorisnika(string id)
        {
            XmlSerializer<Turista> xmlSerializerT = new XmlSerializer<Turista>();
            List<Turista> sviTuristi = xmlSerializerT.Deserialize(new List<Turista>(), "TuristaLista.xml");
            sviTuristi.Find(t => t.Id == id).Blokiran = true;
            xmlSerializerT.Serialize(sviTuristi, "TuristaLista.xml");

            Session["turisti"] = sviTuristi;

            return View("PrikazKorisnika");
        }

        [HttpPost]
        public ActionResult PretraziKorisnike(string ime, string prezime, string uloga)
        {
            if (uloga == "Menadzer")
            {
                XmlSerializer<Menadzer> xmlSerializerM = new XmlSerializer<Menadzer>();
                List<Menadzer> sviMenadzeri = xmlSerializerM.Deserialize(new List<Menadzer>(), "MenadzerLista.xml");

                List<Menadzer> odabraniMenadzeri = new List<Menadzer>();

                if (!string.IsNullOrWhiteSpace(ime) && !string.IsNullOrWhiteSpace(prezime))
                {
                    odabraniMenadzeri.AddRange(sviMenadzeri.Where(k => k.Uloga == Uloga.Menadzer && k.Ime.ToLower().Contains(ime.ToLower()) && k.Prezime.ToLower().Contains(prezime.ToLower())));
                }
                else if (!string.IsNullOrWhiteSpace(ime) && string.IsNullOrWhiteSpace(prezime))
                {
                    odabraniMenadzeri.AddRange(sviMenadzeri.Where(k => k.Uloga == Uloga.Menadzer && k.Ime.ToLower().Contains(ime.ToLower())));
                }
                else if (string.IsNullOrWhiteSpace(ime) && !string.IsNullOrWhiteSpace(prezime))
                {
                    odabraniMenadzeri.AddRange(sviMenadzeri.Where(k => k.Uloga == Uloga.Menadzer && k.Prezime.ToLower().Contains(prezime.ToLower())));
                }
                else
                {
                    odabraniMenadzeri.AddRange(sviMenadzeri.Where(k => k.Uloga == Uloga.Menadzer));
                }

                Session["menadzeri"] = odabraniMenadzeri;
                Session["turisti"] = new List<Turista>();
            }
            else if (uloga == "Turista")
            {
                XmlSerializer<Turista> xmlSerializerT = new XmlSerializer<Turista>();
                List<Turista> sviTuristi = xmlSerializerT.Deserialize(new List<Turista>(), "TuristaLista.xml");

                List<Turista> odabraniTuristi = new List<Turista>();

                if (!string.IsNullOrWhiteSpace(ime) && !string.IsNullOrWhiteSpace(prezime))
                {
                    odabraniTuristi.AddRange(sviTuristi.Where(k => k.Uloga == Uloga.Turista && k.Ime.ToLower().Contains(ime.ToLower()) && k.Prezime.ToLower().Contains(prezime.ToLower())));
                }
                else if (!string.IsNullOrWhiteSpace(ime) && string.IsNullOrWhiteSpace(prezime))
                {
                    odabraniTuristi.AddRange(sviTuristi.Where(k => k.Uloga == Uloga.Turista && k.Ime.ToLower().Contains(ime.ToLower())));
                }
                else if (string.IsNullOrWhiteSpace(ime) && !string.IsNullOrWhiteSpace(prezime))
                {
                    odabraniTuristi.AddRange(sviTuristi.Where(k => k.Uloga == Uloga.Turista && k.Prezime.ToLower().Contains(prezime.ToLower())));
                }
                else
                {
                    odabraniTuristi.AddRange(sviTuristi.Where(k => k.Uloga == Uloga.Turista));
                }

                Session["turisti"] = odabraniTuristi;
                Session["menadzeri"] = new List<Menadzer>();
            }
            else
            {
                XmlSerializer<Menadzer> xmlSerializerM = new XmlSerializer<Menadzer>();
                List<Menadzer> sviMenadzeri = xmlSerializerM.Deserialize(new List<Menadzer>(), "MenadzerLista.xml");
                XmlSerializer<Turista> xmlSerializerT = new XmlSerializer<Turista>();
                List<Turista> sviTuristi = xmlSerializerT.Deserialize(new List<Turista>(), "TuristaLista.xml");

                List<Menadzer> odabraniMenadzer = new List<Menadzer>();


                if (!string.IsNullOrWhiteSpace(ime) && !string.IsNullOrWhiteSpace(prezime))
                {
                    odabraniMenadzer.AddRange(sviMenadzeri.Where(k => k.Uloga == Uloga.Menadzer && k.Ime.ToLower().Contains(ime.ToLower()) && k.Prezime.ToLower().Contains(prezime.ToLower())));
                }
                else if (!string.IsNullOrWhiteSpace(ime) && string.IsNullOrWhiteSpace(prezime))
                {
                    odabraniMenadzer.AddRange(sviMenadzeri.Where(k => k.Uloga == Uloga.Menadzer && k.Ime.ToLower().Contains(ime.ToLower())));
                }
                else if (string.IsNullOrWhiteSpace(ime) && !string.IsNullOrWhiteSpace(prezime))
                {
                    odabraniMenadzer.AddRange(sviMenadzeri.Where(k => k.Uloga == Uloga.Menadzer && k.Prezime.ToLower().Contains(prezime.ToLower())));
                }
                else
                {
                    odabraniMenadzer.AddRange(sviMenadzeri.Where(k => k.Uloga == Uloga.Menadzer));
                }

                List<Turista> odabraniTuristi = new List<Turista>();

                if (!string.IsNullOrWhiteSpace(ime) && !string.IsNullOrWhiteSpace(prezime))
                {
                    odabraniTuristi.AddRange(sviTuristi.Where(k => k.Uloga == Uloga.Turista && k.Ime.ToLower().Contains(ime.ToLower()) && k.Prezime.ToLower().Contains(prezime.ToLower())));
                }
                else if (!string.IsNullOrWhiteSpace(ime) && string.IsNullOrWhiteSpace(prezime))
                {
                    odabraniTuristi.AddRange(sviTuristi.Where(k => k.Uloga == Uloga.Turista && k.Ime.ToLower().Contains(ime.ToLower())));
                }
                else if (string.IsNullOrWhiteSpace(ime) && !string.IsNullOrWhiteSpace(prezime))
                {
                    odabraniTuristi.AddRange(sviTuristi.Where(k => k.Uloga == Uloga.Turista && k.Prezime.ToLower().Contains(prezime.ToLower())));
                }
                else
                {
                    odabraniTuristi.AddRange(sviTuristi.Where(k => k.Uloga == Uloga.Turista));
                }

                Session["turisti"] = odabraniTuristi;
                Session["menadzeri"] = odabraniMenadzer;
            }

            return View("PrikazKorisnika");
        }

        [HttpGet]
        public ActionResult Sortiraj(string nacin)
        {
            XmlSerializer<Menadzer> xmlSerializerM = new XmlSerializer<Menadzer>();
            List<Menadzer> sviMenadzeri = xmlSerializerM.Deserialize(new List<Menadzer>(), "MenadzerLista.xml");
            XmlSerializer<Turista> xmlSerializerT = new XmlSerializer<Turista>();
            List<Turista> sviTuristi = xmlSerializerT.Deserialize(new List<Turista>(), "TuristaLista.xml");

            List<Menadzer> sortiraniMenadzeri = new List<Menadzer>();
            List<Turista> sortiraniTuristi = new List<Turista>();

            if (nacin == "ime_Rastuce")
            {
                sortiraniMenadzeri = ((List<Menadzer>)Session["menadzeri"]).OrderBy(k => k.Ime).ToList();
                sortiraniTuristi = ((List<Turista>)Session["turisti"]).OrderBy(k => k.Ime).ToList();
            }
            else if (nacin == "ime_Opadajuce")
            {
                sortiraniMenadzeri = ((List<Menadzer>)Session["menadzeri"]).OrderByDescending(k => k.Ime).ToList();
                sortiraniTuristi = ((List<Turista>)Session["turisti"]).OrderByDescending(k => k.Ime).ToList();
            }
            else if (nacin == "prezime_Rastuce")
            {
                sortiraniMenadzeri = ((List<Menadzer>)Session["menadzeri"]).OrderBy(k => k.Prezime).ToList();
                sortiraniTuristi = ((List<Turista>)Session["turisti"]).OrderBy(k => k.Prezime).ToList();
            }
            else if (nacin == "prezime_Opadajuce")
            {
                sortiraniMenadzeri = ((List<Menadzer>)Session["menadzeri"]).OrderByDescending(k => k.Prezime).ToList();
                sortiraniTuristi = ((List<Turista>)Session["turisti"]).OrderByDescending(k => k.Prezime).ToList();
            }
            else if (nacin == "uloga_Rastuce")
            {
                sortiraniMenadzeri = ((List<Menadzer>)Session["menadzeri"]).OrderBy(k => k.Uloga).ToList();
                sortiraniTuristi = ((List<Turista>)Session["turisti"]).OrderBy(k => k.Uloga).ToList();
            }
            else if (nacin == "uloga_Opadajuce")
            {
                sortiraniMenadzeri = ((List<Menadzer>)Session["menadzeri"]).OrderByDescending(k => k.Uloga).ToList();
                sortiraniTuristi = ((List<Turista>)Session["turisti"]).OrderByDescending(k => k.Uloga).ToList();
            }

            Session["turisti"] = sortiraniTuristi;
            Session["menadzeri"] = sortiraniMenadzeri;

            return View("PrikazKorisnika");
        }

    }
}