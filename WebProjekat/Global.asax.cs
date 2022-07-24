using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using WebProjekat.Models;

namespace WebProjekat
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            RouteConfig.RegisterRoutes(RouteTable.Routes);

            List<Korisnik> admini = new List<Korisnik>()
            {
                new Korisnik(){ Id = "1", KorisnickoIme = "a1", Lozinka = "pass", Ime = "Franc", Prezime = "Kafka", Pol = Pol.Muski, Email = "fk@gmail.com", DatumRodjenja = new DateTime(1883, 7, 3), Uloga = Uloga.Administrator },
                new Korisnik(){ Id = "2", KorisnickoIme = "a2", Lozinka = "pass", Ime = "Desanka", Prezime = "Maksimovic", Pol = Pol.Zenski, Email = "dm@gmail.com", DatumRodjenja = new DateTime(1896, 5, 16), Uloga = Uloga.Administrator },
            };
            XmlSerializer<Korisnik> xmlSerializer = new XmlSerializer<Korisnik>();
            xmlSerializer.Serialize(admini, "AdminLista.xml");

            XmlSerializer<Menadzer> xmlSerializerM = new XmlSerializer<Menadzer>();
            List<Menadzer> sviMenadzeri = xmlSerializerM.Deserialize(new List<Menadzer>(), "MenadzerLista.xml");
            if (sviMenadzeri.Count() == 0)
            {
                List<Menadzer> menadzeri = new List<Menadzer>();

                Menadzer m1 = new Menadzer() { Id = Guid.NewGuid().ToString(), KorisnickoIme = "m1", Lozinka = "pass", Ime = "meni1", Prezime = "boss1", Pol = Pol.Muski, Email = "mb1@gmail.com", DatumRodjenja = new DateTime(1883, 7, 3), Uloga = Uloga.Menadzer };
                Menadzer m2 = new Menadzer() { Id = Guid.NewGuid().ToString(), KorisnickoIme = "m2", Lozinka = "pass", Ime = "meni2", Prezime = "boss2", Pol = Pol.Zenski, Email = "mb2@gmail.com", DatumRodjenja = new DateTime(1896, 5, 16), Uloga = Uloga.Menadzer };

                Aranzman a1 = new Aranzman()
                {
                    Id = Guid.NewGuid().ToString(),
                    Naziv = "Hurghada",
                    Tip = TipAranzmana.AllInclusive,
                    Prevoz = TipPrevoza.AutobusAvion,
                    Lokacija = "Egipat",
                    DatumPocetka = new DateTime(2021, 7, 13),
                    DatumZavrsetka = new DateTime(2021, 7, 31),
                    MestoNalazenjaStr = "Aerodrom Nikola Tesla",
                    MestoNalazenja = new MestoNalazenja() { Id = Guid.NewGuid().ToString(), Adresa = "Aerodrom Nikola Tesla" },
                    VremeNalazenja = new DateTime(2021, 7, 13, 5, 20, 0),
                    MaksimalanBrojPutnika = 150,
                    OpisAranzmana = "Top destinacija",
                    OpisPutovanja = "Ekskluzivan smestaj",
                    Slika = "Hurghada.jpg",
                    Smestaj = null,

                    Obrisan = false,
                    Slobodan = true,
                    MenadzerId = m1.Id,
                    Prosao = false
                };

                Aranzman a2 = new Aranzman()
                {
                    Id = Guid.NewGuid().ToString(),
                    Naziv = "Ibiza",
                    Tip = TipAranzmana.AllInclusive,
                    Prevoz = TipPrevoza.AutobusAvion,
                    Lokacija = "Spanija",
                    DatumPocetka = new DateTime(2021, 7, 20),
                    DatumZavrsetka = new DateTime(2021, 7, 30),
                    MestoNalazenjaStr = "Aerodrom Nikola Tesla",
                    MestoNalazenja = new MestoNalazenja() { Id = Guid.NewGuid().ToString(), Adresa = "Aerodrom Nikola Tesla" },
                    VremeNalazenja = new DateTime(2021, 7, 20, 7, 50, 0),
                    MaksimalanBrojPutnika = 220,
                    OpisAranzmana = "Ekskluzivna destinacija",
                    OpisPutovanja = "Top smestaj",
                    Slika = "Ibiza.jpg",
                    Smestaj = null,

                    Obrisan = false,
                    Slobodan = true,
                    MenadzerId = m2.Id,
                    Prosao = false
                };

                Aranzman a3 = new Aranzman()
                {
                    Id = Guid.NewGuid().ToString(),
                    Naziv = "Pefkohori",
                    Tip = TipAranzmana.NajamApartmana,
                    Prevoz = TipPrevoza.Individualan,
                    Lokacija = "Grcka",
                    DatumPocetka = new DateTime(2021, 6, 1),
                    DatumZavrsetka = new DateTime(2021, 6, 15),
                    MestoNalazenjaStr = "Negde u Grckoj",
                    MestoNalazenja = new MestoNalazenja() { Id = Guid.NewGuid().ToString(), Adresa = "Negde u Grckoj" },
                    VremeNalazenja = new DateTime(2021, 6, 1, 14, 0, 0),
                    MaksimalanBrojPutnika = 4,
                    OpisAranzmana = "Jako lep aranzman",
                    OpisPutovanja = "Top putovanje",
                    Slika = "Pefkohori.jpg",
                    Smestaj = null,

                    Obrisan = false,
                    Slobodan = true,
                    MenadzerId = m1.Id,
                    Prosao = true
                };

                m1.Aranzmani.Add(a1);
                m1.Aranzmani.Add(a3);
                m2.Aranzmani.Add(a2);

                menadzeri.Add(m1);
                menadzeri.Add(m2);
                xmlSerializerM.Serialize(menadzeri, "MenadzerLista.xml");


                XmlSerializer<Aranzman> xmlSerializerArr = new XmlSerializer<Aranzman>();
                List<Aranzman> sviAranzmani = xmlSerializerArr.Deserialize(new List<Aranzman>(), "AranzmanLista.xml");

                Smestaj s1 = new Smestaj()
                {
                    Id = Guid.NewGuid().ToString(),
                    Tip = TipSmestaja.Hotel,
                    Naziv = "Smestaj1",
                    BrojZvezdica = 4,
                    PostojanjeBazena = true,
                    PostojanjeSpaCentra = false,
                    PrilagodjenoZaOsobeSaInvaliditetom = true,
                    PostojanjeWiFiKonekcije = true,

                    Obrisan = false,
                    Slobodan = true,
                    AranzmanId = a1.Id
                };

                Smestaj s2 = new Smestaj()
                {
                    Id = Guid.NewGuid().ToString(),
                    Tip = TipSmestaja.Hotel,
                    Naziv = "Smestaj2",
                    BrojZvezdica = 5,
                    PostojanjeBazena = true,
                    PostojanjeSpaCentra = true,
                    PrilagodjenoZaOsobeSaInvaliditetom = true,
                    PostojanjeWiFiKonekcije = true,

                    Obrisan = false,
                    Slobodan = true,
                    AranzmanId = a2.Id
                };

                Smestaj s3 = new Smestaj()
                {
                    Id = Guid.NewGuid().ToString(),
                    Tip = TipSmestaja.Vila,
                    Naziv = "grcki smestaj",
                    BrojZvezdica = 2,
                    PostojanjeBazena = false,
                    PostojanjeSpaCentra = false,
                    PrilagodjenoZaOsobeSaInvaliditetom = false,
                    PostojanjeWiFiKonekcije = true,

                    Obrisan = false,
                    Slobodan = true,
                    AranzmanId = a3.Id
                };

                SmestajnaJedinica sj1 = new SmestajnaJedinica()
                {
                    Id = Guid.NewGuid().ToString(),
                    DozvoljenBrojGostiju = 2,
                    DozvoljenBoravakKucnihLjubimaca = false,
                    Cena = 650,

                    Obrisan = false,
                    Slobodan = true,
                    SmestajId = s1.Id
                };

                SmestajnaJedinica sj2 = new SmestajnaJedinica()
                {
                    Id = Guid.NewGuid().ToString(),
                    DozvoljenBrojGostiju = 2,
                    DozvoljenBoravakKucnihLjubimaca = false,
                    Cena = 780,

                    Obrisan = false,
                    Slobodan = true,
                    SmestajId = s2.Id
                };

                SmestajnaJedinica sj3 = new SmestajnaJedinica()
                {
                    Id = Guid.NewGuid().ToString(),
                    DozvoljenBrojGostiju = 4,
                    DozvoljenBoravakKucnihLjubimaca = false,
                    Cena = 170,

                    Obrisan = false,
                    Slobodan = true,
                    SmestajId = s3.Id
                };

                s1.SmestajneJedinice.Add(sj1);
                s2.SmestajneJedinice.Add(sj2);
                s3.SmestajneJedinice.Add(sj3);

                a1.Smestaj = s1;
                a2.Smestaj = s2;
                a3.Smestaj = s3;

                sviAranzmani.Add(a1);
                sviAranzmani.Add(a2);
                sviAranzmani.Add(a3);

                xmlSerializerArr.Serialize(sviAranzmani, "AranzmanLista.xml");

                XmlSerializer<Turista> xmlSerializerT = new XmlSerializer<Turista>();
                List<Turista> sviTuristi = xmlSerializerT.Deserialize(new List<Turista>(), "TuristaLista.xml");

                Turista turista = new Turista() { Id = Guid.NewGuid().ToString(), KorisnickoIme = "t1", Lozinka = "pass", Ime = "turista", Prezime = "turistic", Pol = Pol.Muski, Email = "t@gmail.com", DatumRodjenja = new DateTime(1985, 9, 5), Uloga = Uloga.Turista, BrojOtkazivanja = 0, Blokiran = false };

                sviTuristi.Add(turista);
                xmlSerializerT.Serialize(sviTuristi, "TuristaLista.xml");
            }
        }
    }
}
