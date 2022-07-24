using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebProjekat.Models;

namespace WebApp.Controllers
{
    public class RezervacijeController : Controller
    {
        // GET: Rezervacije
        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Rezervisi(Rezervacija rezervacija)
        {
            Korisnik korisnik = (Korisnik)Session["korisnik"];

            rezervacija.Id = Guid.NewGuid().ToString();
            rezervacija.TuristaId = korisnik.Id;
            rezervacija.Status = StatusRezervacije.Aktivna;

            XmlSerializer<Aranzman> xmlSerializerA = new XmlSerializer<Aranzman>();
            List<Aranzman> sviAranzmani = xmlSerializerA.Deserialize(new List<Aranzman>(), "AranzmanLista.xml");
            Aranzman aranzman = sviAranzmani.Find(a => a.Id == rezervacija.AranzmanId);
            rezervacija.NazivAranzmana = aranzman.Naziv;
            rezervacija.LokacijaAranzmana = aranzman.Lokacija;
            rezervacija.DatumPocetka = aranzman.DatumPocetka.ToString();
            rezervacija.DatumZavrsetka = aranzman.DatumZavrsetka.ToString();
            rezervacija.Cena = aranzman.Smestaj.SmestajneJedinice.Find(sj => sj.Id == rezervacija.SmestajnaJedinicaId).Cena;
            rezervacija.SmestajnaJedinicaId = aranzman.Smestaj.SmestajneJedinice.Find(sj => sj.Id == rezervacija.SmestajnaJedinicaId).Id;
            rezervacija.MozeSeOtkazati = false;
            rezervacija.MozeSeKomentarisati = false;
            rezervacija.Komentarisano = false;
            aranzman.Smestaj.SmestajneJedinice.Find(sj => sj.Id == rezervacija.SmestajnaJedinicaId).Slobodan = false;

            XmlSerializer<Turista> xmlSerializer = new XmlSerializer<Turista>();
            List<Turista> sviTuristi = xmlSerializer.Deserialize(new List<Turista>(), "TuristaLista.xml");
            Turista turista = sviTuristi.Find(t => t.Id == korisnik.Id);
            turista.Rezervacije.Add(rezervacija);
            xmlSerializer.Serialize(sviTuristi, "TuristaLista.xml");
            xmlSerializerA.Serialize(sviAranzmani, "AranzmanLista.xml");

            Session["korisnik"] = turista;
            Session["a"] = sviAranzmani;

            return View("UspesnaRezervacija");
        }

        public ActionResult UspesnaRezervacija()
        {
            return View();
        }

        [HttpPost]
        public ActionResult OtkaziRezervaciju(Rezervacija rezervacija)
        {
            XmlSerializer<Aranzman> xmlSerializerA = new XmlSerializer<Aranzman>();
            List<Aranzman> sviAranzmani = xmlSerializerA.Deserialize(new List<Aranzman>(), "AranzmanLista.xml");
            sviAranzmani.Find(a => a.Id == rezervacija.AranzmanId).Smestaj.SmestajneJedinice.Find(s => s.Id == rezervacija.SmestajnaJedinicaId).Slobodan = true;
            xmlSerializerA.Serialize(sviAranzmani, "AranzmanLista.xml");

            XmlSerializer<Turista> xmlSerializerT = new XmlSerializer<Turista>();
            List<Turista> sviTuristi = xmlSerializerT.Deserialize(new List<Turista>(), "TuristaLista.xml");
            Turista turista = sviTuristi.Find(t => t.Id == rezervacija.TuristaId);
            //turista.Rezervacije.RemoveAll(r => r.Id == rezervacija.Id);
            turista.Rezervacije.Find(r => r.Id == rezervacija.Id).Status = StatusRezervacije.Otkazana;
            turista.Rezervacije.Find(r => r.Id == rezervacija.Id).MozeSeOtkazati = false;
            turista.Rezervacije.Find(r => r.Id == rezervacija.Id).MozeSeKomentarisati = false;
            turista.BrojOtkazivanja++;
            xmlSerializerT.Serialize(sviTuristi, "TuristaLista.xml");

            Session["korisnik"] = turista;
            Session["a"] = sviAranzmani;


            return RedirectToAction("Profil", "Korisnik");
        }

        [HttpPost]
        public ActionResult KomentarisiAranzman(Komentar komentar)
        {
            komentar.Id = Guid.NewGuid().ToString();
            komentar.Odobren = false;
            komentar.Odobren = false;

            XmlSerializer<Komentar> xmlSerializer = new XmlSerializer<Komentar>();
            List<Komentar> sviKomentari = xmlSerializer.Deserialize(new List<Komentar>(), "KomentarLista.xml");
            sviKomentari.Add(komentar);
            xmlSerializer.Serialize(sviKomentari, "KomentarLista.xml");

            XmlSerializer<Turista> xmlSerializerT = new XmlSerializer<Turista>();
            List<Turista> sviTuristi = xmlSerializerT.Deserialize(new List<Turista>(), "TuristaLista.xml");
            Turista turista = sviTuristi.Find(t => t.Id == komentar.TuristaId);
            turista.Rezervacije.Find(r => r.Id == komentar.RezervacijaId).Komentarisano = true;
            xmlSerializerT.Serialize(sviTuristi, "TuristaLista.xml");
            Session["korisnik"] = turista;

            return RedirectToAction("Profil", "Korisnik");
        }

        [HttpPost]
        public ActionResult OdobriKomentar(string id)
        {
            XmlSerializer<Komentar> xmlSerializer = new XmlSerializer<Komentar>();
            List<Komentar> sviKomentari = xmlSerializer.Deserialize(new List<Komentar>(), "KomentarLista.xml");
            sviKomentari.Find(k => k.Id == id).Odobren = true;
            xmlSerializer.Serialize(sviKomentari, "KomentarLista.xml");

            Session["komentari"] = sviKomentari;
            return RedirectToAction("Profil", "Korisnik");
        }

        [HttpPost]
        public ActionResult OdbijKomentar(string id)
        {
            XmlSerializer<Komentar> xmlSerializer = new XmlSerializer<Komentar>();
            List<Komentar> sviKomentari = xmlSerializer.Deserialize(new List<Komentar>(), "KomentarLista.xml");
            sviKomentari.Find(k => k.Id == id).Odbijen = true;
            xmlSerializer.Serialize(sviKomentari, "KomentarLista.xml");

            Session["komentari"] = sviKomentari;
            return RedirectToAction("Profil", "Korisnik");
        }
    }
}