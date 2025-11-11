using System;
using System.Collections.Generic;
using System.Linq;

namespace PametnaBiblioteka
{
    public class Korisnik
    {
        public int Id { get; set; }
        public string Ime { get; set; }
        public string Prezime { get; set; }
        public string Email { get; set; }
        public string Uloga { get; set; }

        public Korisnik(int id, string ime, string prezime, string email, string uloga)
        {
            Id = id;
            Ime = ime;
            Prezime = prezime;
            Email = email;
            Uloga = uloga;
        }

        public override string ToString()
        {
            return $"{Id}. {Ime} {Prezime} - {Email} ({Uloga})";
        }
    }

    public class UpravljanjeKorisnicima
    {
        private List<Korisnik> korisnici = new List<Korisnik>();
        private int nextId = 1;

        public void DodajKorisnika()
        {
            Console.Write("Unesite ime: ");
            string ime = Console.ReadLine()?.Trim();
            Console.Write("Unesite prezime: ");
            string prezime = Console.ReadLine()?.Trim();
            Console.Write("Unesite email: ");
            string email = Console.ReadLine()?.Trim();

            if (string.IsNullOrWhiteSpace(ime) || string.IsNullOrWhiteSpace(prezime) || string.IsNullOrWhiteSpace(email))
            {
                Console.WriteLine("Svi podaci su obavezni!");
                return;
            }
            if (!email.Contains("@"))
            {
                Console.WriteLine(" Neispravan email format!");
                return;
            }

            Console.Write("Unesite ulogu (Administrator/Clan): ");
            string uloga = Console.ReadLine()?.Trim();
            if (uloga.ToLower() != "administrator" && uloga.ToLower() != "clan")
            {
                Console.WriteLine("Neispravna uloga! Koristite 'Administrator' ili 'Clan'.");
                return;
            }

            var novi = new Korisnik(nextId++, ime, prezime, email, uloga);
            korisnici.Add(novi);
            Console.WriteLine(" Korisnik uspješno dodan!");
        }

        public void AzurirajKorisnika()
        {
            PrikaziKorisnike();
            Console.Write("Unesite ID korisnika kojeg želite ažurirati: ");
            if (!int.TryParse(Console.ReadLine(), out int id))
            {
                Console.WriteLine(" Neispravan unos!");
                return;
            }

            var korisnik = korisnici.FirstOrDefault(k => k.Id == id);
            if (korisnik == null)
            {
                Console.WriteLine(" Korisnik nije pronađen!");
                return;
            }

            Console.Write("Novo ime (prazno = zadrži staro): ");
            string ime = Console.ReadLine();
            if (!string.IsNullOrWhiteSpace(ime)) korisnik.Ime = ime;

            Console.Write("Novo prezime (prazno = zadrži staro): ");
            string prezime = Console.ReadLine();
            if (!string.IsNullOrWhiteSpace(prezime)) korisnik.Prezime = prezime;

            Console.Write("Novi email (prazno = zadrži stari): ");
            string email = Console.ReadLine();
            if (!string.IsNullOrWhiteSpace(email))
            {
                if (!email.Contains("@"))
                {
                    Console.WriteLine(" Email nije validan!");
                    return;
                }
                korisnik.Email = email;
            }

            Console.Write("Nova uloga (Administrator/Clan, prazno = zadrži staru): ");
            string uloga = Console.ReadLine();
            if (!string.IsNullOrWhiteSpace(uloga))
            {
                if (uloga.ToLower() != "administrator" && uloga.ToLower() != "clan")
                {
                    Console.WriteLine(" Uloga nije validna!");
                    return;
                }
                korisnik.Uloga = uloga;
            }

            Console.WriteLine(" Korisnik ažuriran!");
        }

        public void ObrisiKorisnika()
        {
            PrikaziKorisnike();
            Console.Write("Unesite ID korisnika za brisanje: ");
            if (!int.TryParse(Console.ReadLine(), out int id))
            {
                Console.WriteLine(" Neispravan unos!");
                return;
            }

            var korisnik = korisnici.FirstOrDefault(k => k.Id == id);
            if (korisnik == null)
            {
                Console.WriteLine(" Korisnik nije pronađen!");
                return;
            }

            korisnici.Remove(korisnik);
            Console.WriteLine(" Korisnik obrisan!");
        }

        public void PrikaziKorisnike()
        {
            if (korisnici.Count == 0)
            {
                Console.WriteLine(" Nema registrovanih korisnika.");
                return;
            }

            Console.WriteLine("\n--- Lista korisnika ---");
            foreach (var k in korisnici)
                Console.WriteLine(k);
            Console.WriteLine();
        }
    }

    class Program
    {
        static void Main()
        {
            UpravljanjeKorisnicima sistem = new UpravljanjeKorisnicima();
            bool radi = true;

            while (radi)
            {
                Console.WriteLine("\n=== SISTEM ZA UPRAVLJANJE KORISNICIMA ===");
                Console.WriteLine("1. Dodaj korisnika");
                Console.WriteLine("2. Ažuriraj korisnika");
                Console.WriteLine("3. Obriši korisnika");
                Console.WriteLine("4. Prikaži korisnike");
                Console.WriteLine("0. Izlaz");
                Console.Write("Izbor: ");

                string izbor = Console.ReadLine();
                Console.WriteLine();

                switch (izbor)
                {
                    case "1": sistem.DodajKorisnika(); break;
                    case "2": sistem.AzurirajKorisnika(); break;
                    case "3": sistem.ObrisiKorisnika(); break;
                    case "4": sistem.PrikaziKorisnike(); break;
                    case "0": radi = false; break;
                    default: Console.WriteLine(" Neispravan izbor!"); break;
                }
            }

            Console.WriteLine(" Kraj programa.");
        }
    }
}
