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

    public class Knjiga
    {
        public int Id { get; set; }
        public string Naslov { get; set; }
        public string Autor { get; set; }
        public string Zanr { get; set; }
        public bool Dostupna { get; set; } = true;

        public override string ToString()
        {
            return $"{Id}. \"{Naslov}\" - {Autor} [{Zanr}] - {(Dostupna ? "Dostupna" : "Nedostupna")}";
        }
    }

    public class Posudba
    {
        public int Id { get; set; }
        public int KorisnikId { get; set; }
        public int KnjigaId { get; set; }
        public DateTime DatumPosudbe { get; set; }
        public DateTime RokVracanja { get; set; }
        public DateTime? DatumVracanja { get; set; }
        public decimal Kazna { get; set; }
        public bool Aktivna => !DatumVracanja.HasValue;
    }

    public class UpravljanjeKorisnicima
    {
        private List<Korisnik> korisnici;
        private int nextId;

        public UpravljanjeKorisnicima(List<Korisnik> postojećiKorisnici = null)
        {
            korisnici = postojećiKorisnici ?? new List<Korisnik>();
            nextId = korisnici.Any() ? korisnici.Max(k => k.Id) + 1 : 1;
        }

        public List<Korisnik> VratiSveKorisnike()
        {
            return korisnici;
        }

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
                Console.WriteLine("Neispravan email format!");
                return;
            }

            Console.Write("Unesite ulogu (Administrator/Clan): ");
            string uloga = Console.ReadLine()?.Trim();
            if (string.IsNullOrWhiteSpace(uloga) || (uloga.ToLower() != "administrator" && uloga.ToLower() != "clan"))
            {
                Console.WriteLine("Neispravna uloga! Koristite 'Administrator' ili 'Clan'.");
                return;
            }

            var novi = new Korisnik(nextId++, ime, prezime, email, uloga);
            korisnici.Add(novi);
            Console.WriteLine("Korisnik uspješno dodan!");
        }

        public void AzurirajKorisnika()
        {
            PrikaziKorisnike();
            Console.Write("Unesite ID korisnika kojeg želite ažurirati: ");
            if (!int.TryParse(Console.ReadLine(), out int id))
            {
                Console.WriteLine("Neispravan unos!");
                return;
            }

            var korisnik = korisnici.FirstOrDefault(k => k.Id == id);
            if (korisnik == null)
            {
                Console.WriteLine("Korisnik nije pronađen!");
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
                    Console.WriteLine("Email nije validan!");
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
                    Console.WriteLine("Uloga nije validna!");
                    return;
                }
                korisnik.Uloga = uloga;
            }

            Console.WriteLine("Korisnik ažuriran!");
        }

        public void ObrisiKorisnika()
        {
            PrikaziKorisnike();
            Console.Write("Unesite ID korisnika za brisanje: ");
            if (!int.TryParse(Console.ReadLine(), out int id))
            {
                Console.WriteLine("Neispravan unos!");
                return;
            }

            var korisnik = korisnici.FirstOrDefault(k => k.Id == id);
            if (korisnik == null)
            {
                Console.WriteLine("Korisnik nije pronađen!");
                return;
            }

            korisnici.Remove(korisnik);
            Console.WriteLine("Korisnik obrisan!");
        }

        public void PrikaziKorisnike()
        {
            if (korisnici.Count == 0)
            {
                Console.WriteLine("Nema registrovanih korisnika.");
                return;
            }

            Console.WriteLine("\n--- Lista korisnika ---");
            foreach (var k in korisnici)
                Console.WriteLine(k);
            Console.WriteLine();
        }
    }

    public class InventarKnjiga
    {
        private readonly List<Knjiga> knjige;
        private int nextId;

        public InventarKnjiga(List<Knjiga> knjige)
        {
            this.knjige = knjige;
            nextId = knjige.Any() ? knjige.Max(k => k.Id) + 1 : 1;
        }

        public List<Knjiga> VratiSveKnjige()
        {
            return knjige;
        }

        public void DodajKnjigu()
        {
            Console.Write("Unesite naslov: ");
            string naslov = Console.ReadLine()?.Trim();
            Console.Write("Unesite autora: ");
            string autor = Console.ReadLine()?.Trim();
            Console.Write("Unesite žanr: ");
            string zanr = Console.ReadLine()?.Trim();

            if (string.IsNullOrWhiteSpace(naslov) || string.IsNullOrWhiteSpace(autor) || string.IsNullOrWhiteSpace(zanr))
            {
                Console.WriteLine("Svi podaci su obavezni!");
                return;
            }

            var knjiga = new Knjiga
            {
                Id = nextId++,
                Naslov = naslov,
                Autor = autor,
                Zanr = zanr,
                Dostupna = true
            };

            knjige.Add(knjiga);
            Console.WriteLine("Knjiga uspješno dodana!");
        }

        public void AzurirajKnjigu()
        {
            PrikaziSveKnjige();
            Console.Write("Unesite ID knjige za ažuriranje: ");
            if (!int.TryParse(Console.ReadLine(), out int id))
            {
                Console.WriteLine("Neispravan unos!");
                return;
            }

            var knjiga = knjige.FirstOrDefault(k => k.Id == id);
            if (knjiga == null)
            {
                Console.WriteLine("Knjiga nije pronađena!");
                return;
            }

            Console.Write("Novi naslov (prazno = zadrži stari): ");
            string naslov = Console.ReadLine();
            if (!string.IsNullOrWhiteSpace(naslov)) knjiga.Naslov = naslov;

            Console.Write("Novi autor (prazno = zadrži starog): ");
            string autor = Console.ReadLine();
            if (!string.IsNullOrWhiteSpace(autor)) knjiga.Autor = autor;

            Console.Write("Novi žanr (prazno = zadrži stari): ");
            string zanr = Console.ReadLine();
            if (!string.IsNullOrWhiteSpace(zanr)) knjiga.Zanr = zanr;

            Console.WriteLine("Knjiga ažurirana!");
        }

        public void ObrisiKnjigu()
        {
            PrikaziSveKnjige();
            Console.Write("Unesite ID knjige za brisanje: ");
            if (!int.TryParse(Console.ReadLine(), out int id))
            {
                Console.WriteLine("Neispravan unos!");
                return;
            }

            var knjiga = knjige.FirstOrDefault(k => k.Id == id);
            if (knjiga == null)
            {
                Console.WriteLine("Knjiga nije pronađena!");
                return;
            }

            if (!knjiga.Dostupna)
            {
                Console.WriteLine("Knjiga je trenutno posuđena i ne može se obrisati.");
                return;
            }

            knjige.Remove(knjiga);
            Console.WriteLine("Knjiga obrisana!");
        }

        public void PrikaziSveKnjige()
        {
            if (!knjige.Any())
            {
                Console.WriteLine("Nema knjiga u inventaru.");
                return;
            }

            Console.WriteLine("\n--- Inventar knjiga ---");
            foreach (var k in knjige)
                Console.WriteLine(k);
            Console.WriteLine();
        }

        public void PretragaPoNaslovu()
        {
            Console.Write("Unesite dio naslova: ");
            string unos = Console.ReadLine() ?? "";
            var rezultat = knjige
                .Where(k => k.Naslov != null && k.Naslov.IndexOf(unos, StringComparison.OrdinalIgnoreCase) >= 0)
                .ToList();

            if (!rezultat.Any())
            {
                Console.WriteLine("Nema knjiga koje odgovaraju pretrazi.");
                return;
            }

            foreach (var k in rezultat)
                Console.WriteLine(k);
        }

        public void PretragaPoAutoru()
        {
            Console.Write("Unesite ime autora: ");
            string unos = Console.ReadLine() ?? "";
            var rezultat = knjige
                .Where(k => k.Autor != null && k.Autor.IndexOf(unos, StringComparison.OrdinalIgnoreCase) >= 0)
                .ToList();

            if (!rezultat.Any())
            {
                Console.WriteLine("Nema knjiga koje odgovaraju pretrazi.");
                return;
            }

            foreach (var k in rezultat)
                Console.WriteLine(k);
        }

        public void PretragaPoZanru()
        {
            Console.Write("Unesite žanr: ");
            string unos = Console.ReadLine() ?? "";
            var rezultat = knjige
                .Where(k => k.Zanr != null && k.Zanr.IndexOf(unos, StringComparison.OrdinalIgnoreCase) >= 0)
                .ToList();

            if (!rezultat.Any())
            {
                Console.WriteLine("Nema knjiga koje odgovaraju pretrazi.");
                return;
            }

            foreach (var k in rezultat)
                Console.WriteLine(k);
        }
    }

    public class SistemPosudbe
    {
        private readonly List<Posudba> posudbe;
        private readonly List<Korisnik> korisnici;
        private readonly List<Knjiga> knjige;
        private int nextPosudbaId = 1;
        private const decimal KaznaPoDanu = 1m;

        public SistemPosudbe(List<Korisnik> korisnici, List<Knjiga> knjige)
        {
            this.korisnici = korisnici;
            this.knjige = knjige;
            posudbe = new List<Posudba>();
        }

        public void PosudiKnjigu()
        {
            Console.Write("Unesite ID korisnika: ");
            if (!int.TryParse(Console.ReadLine(), out int korisnikId))
            {
                Console.WriteLine("Neispravan unos ID korisnika!");
                return;
            }

            var korisnik = korisnici.FirstOrDefault(k => k.Id == korisnikId);
            if (korisnik == null)
            {
                Console.WriteLine("Korisnik nije pronađen!");
                return;
            }

            Console.Write("Unesite ID knjige: ");
            if (!int.TryParse(Console.ReadLine(), out int knjigaId))
            {
                Console.WriteLine("Neispravan unos ID knjige!");
                return;
            }

            var knjiga = knjige.FirstOrDefault(k => k.Id == knjigaId);
            if (knjiga == null)
            {
                Console.WriteLine("Knjiga nije pronađena!");
                return;
            }

            if (!knjiga.Dostupna)
            {
                Console.WriteLine("Knjiga trenutno nije dostupna za posudbu.");
                return;
            }

            var sada = DateTime.Now;
            var rok = sada.AddDays(14);

            var posudba = new Posudba
            {
                Id = nextPosudbaId++,
                KorisnikId = korisnik.Id,
                KnjigaId = knjiga.Id,
                DatumPosudbe = sada,
                RokVracanja = rok,
                DatumVracanja = null,
                Kazna = 0
            };

            posudbe.Add(posudba);
            knjiga.Dostupna = false;

            Console.WriteLine($"Knjiga \"{knjiga.Naslov}\" je posuđena korisniku {korisnik.Ime} {korisnik.Prezime}.");
            Console.WriteLine($"Rok za vraćanje: {rok:dd.MM.yyyy}.");
        }

        public void VratiKnjigu()
        {
            Console.Write("Unesite ID korisnika: ");
            if (!int.TryParse(Console.ReadLine(), out int korisnikId))
            {
                Console.WriteLine("Neispravan unos ID korisnika!");
                return;
            }

            Console.Write("Unesite ID knjige: ");
            if (!int.TryParse(Console.ReadLine(), out int knjigaId))
            {
                Console.WriteLine("Neispravan unos ID knjige!");
                return;
            }

            var posudba = posudbe
                .LastOrDefault(p => p.KorisnikId == korisnikId && p.KnjigaId == knjigaId && p.Aktivna);

            if (posudba == null)
            {
                Console.WriteLine("Nije pronađena aktivna posudba za zadate podatke.");
                return;
            }

            var knjiga = knjige.FirstOrDefault(k => k.Id == knjigaId);
            var korisnik = korisnici.FirstOrDefault(k => k.Id == korisnikId);

            var danas = DateTime.Now;
            posudba.DatumVracanja = danas;

            if (danas.Date > posudba.RokVracanja.Date)
            {
                int daniKasnjenja = (danas.Date - posudba.RokVracanja.Date).Days;
                posudba.Kazna = daniKasnjenja * KaznaPoDanu;
            }

            if (knjiga != null)
                knjiga.Dostupna = true;

            Console.WriteLine($"Knjiga \"{knjiga?.Naslov}\" vraćena {danas:dd.MM.yyyy} od strane {korisnik?.Ime} {korisnik?.Prezime}.");

            if (posudba.Kazna > 0)
            {
                Console.WriteLine($"Korisnik kasni sa vraćanjem. Kazna iznosi: {posudba.Kazna} KM.");
            }
            else
            {
                Console.WriteLine("Nema kazne za kašnjenje.");
            }
        }

        public void PrikaziAktivnePosudbe()
        {
            var aktivne = posudbe.Where(p => p.Aktivna).ToList();

            if (!aktivne.Any())
            {
                Console.WriteLine("Nema aktivnih posudbi.");
                return;
            }

            Console.WriteLine("\n--- Aktivne posudbe ---");
            foreach (var p in aktivne)
            {
                var korisnik = korisnici.FirstOrDefault(k => k.Id == p.KorisnikId);
                var knjiga = knjige.FirstOrDefault(k => k.Id == p.KnjigaId);

                Console.WriteLine($"[{p.Id}] {korisnik?.Ime} {korisnik?.Prezime} - \"{knjiga?.Naslov}\" | Posuđeno: {p.DatumPosudbe:dd.MM.yyyy} | Rok: {p.RokVracanja:dd.MM.yyyy}");
            }
            Console.WriteLine();
        }

        public void PrikaziHistorijuZaKorisnika()
        {
            Console.Write("Unesite ID korisnika: ");
            if (!int.TryParse(Console.ReadLine(), out int korisnikId))
            {
                Console.WriteLine("Neispravan unos ID korisnika!");
                return;
            }

            var korisnik = korisnici.FirstOrDefault(k => k.Id == korisnikId);
            if (korisnik == null)
            {
                Console.WriteLine("Korisnik nije pronađen!");
                return;
            }

            var historija = posudbe.Where(p => p.KorisnikId == korisnikId).ToList();
            if (!historija.Any())
            {
                Console.WriteLine($"Korisnik {korisnik.Ime} {korisnik.Prezime} nema posudbi u historiji.");
                return;
            }

            Console.WriteLine($"\n--- Historija posudbi za: {korisnik.Ime} {korisnik.Prezime} ---");
            foreach (var p in historija)
            {
                var knjiga = knjige.FirstOrDefault(k => k.Id == p.KnjigaId);
                string status = p.Aktivna ? "AKTIVNA" : "ZAVRŠENA";
                                Console.WriteLine(
                    $"\"{knjiga?.Naslov}\" | Posuđeno: {p.DatumPosudbe:dd.MM.yyyy} | Rok: {p.RokVracanja:dd.MM.yyyy} | " +
                    $"Vraćeno: {(p.DatumVracanja.HasValue ? p.DatumVracanja.Value.ToString("dd.MM.yyyy") : "-")} | " +
                    $"Kazna: {p.Kazna} KM | Status: {status}");
            }
            Console.WriteLine();
        }

        public List<Posudba> VratiSvePosudbe()
        {
            return posudbe;
        }
    }

    class Program
    {
        static void Main()
        {
            var korisniciList = new List<Korisnik>();
            var knjigeList = new List<Knjiga>();

            var upravljanjeKorisnicima = new UpravljanjeKorisnicima(korisniciList);
            var inventar = new InventarKnjiga(knjigeList);
            var sistemPosudbe = new SistemPosudbe(upravljanjeKorisnicima.VratiSveKorisnike(), knjigeList);

            bool radi = true;

            while (radi)
            {
                Console.WriteLine("\n=== PAMETNA BIBLIOTEKA - GLAVNI MENI ===");
                Console.WriteLine("1. Upravljanje korisnicima");
                Console.WriteLine("2. Inventar knjiga");
                Console.WriteLine("3. Sistem posudbe");
                Console.WriteLine("0. Izlaz");
                Console.Write("Izbor: ");

                string izbor = Console.ReadLine();
                Console.WriteLine();

                switch (izbor)
                {
                    case "1":
                        MeniKorisnici(upravljanjeKorisnicima);
                        break;
                    case "2":
                        MeniKnjige(inventar);
                        break;
                    case "3":
                        MeniPosudbe(sistemPosudbe);
                        break;
                    case "0":
                        radi = false;
                        break;
                    default:
                        Console.WriteLine("Neispravan izbor!");
                        break;
                }
            }

            Console.WriteLine("Kraj programa.");
        }

        static void MeniKorisnici(UpravljanjeKorisnicima upravljanje)
        {
            bool ok = true;
            while (ok)
            {
                Console.WriteLine("\n--- UPRAVLJANJE KORISNICIMA ---");
                Console.WriteLine("1. Dodaj korisnika");
                Console.WriteLine("2. Ažuriraj korisnika");
                Console.WriteLine("3. Obriši korisnika");
                Console.WriteLine("4. Prikaži korisnike");
                Console.WriteLine("0. Povratak");
                Console.Write("Izbor: ");

                string izbor = Console.ReadLine();
                Console.WriteLine();

                switch (izbor)
                {
                    case "1": upravljanje.DodajKorisnika(); break;
                    case "2": upravljanje.AzurirajKorisnika(); break;
                    case "3": upravljanje.ObrisiKorisnika(); break;
                    case "4": upravljanje.PrikaziKorisnike(); break;
                    case "0": ok = false; break;
                    default: Console.WriteLine("Neispravan izbor!"); break;
                }
            }
        }

        static void MeniKnjige(InventarKnjiga inventar)
        {
            bool ok = true;
            while (ok)
            {
                Console.WriteLine("\n--- INVENTAR KNJIGA ---");
                Console.WriteLine("1. Dodaj knjigu");
                Console.WriteLine("2. Ažuriraj knjigu");
                Console.WriteLine("3. Obriši knjigu");
                Console.WriteLine("4. Prikaži sve knjige");
                Console.WriteLine("5. Pretraga po naslovu");
                Console.WriteLine("6. Pretraga po autoru");
                Console.WriteLine("7. Pretraga po žanru");
                Console.WriteLine("0. Povratak");
                Console.Write("Izbor: ");

                string izbor = Console.ReadLine();
                Console.WriteLine();

                switch (izbor)
                {
                    case "1": inventar.DodajKnjigu(); break;
                    case "2": inventar.AzurirajKnjigu(); break;
                    case "3": inventar.ObrisiKnjigu(); break;
                    case "4": inventar.PrikaziSveKnjige(); break;
                    case "5": inventar.PretragaPoNaslovu(); break;
                    case "6": inventar.PretragaPoAutoru(); break;
                    case "7": inventar.PretragaPoZanru(); break;
                    case "0": ok = false; break;
                    default: Console.WriteLine("Neispravan izbor!"); break;
                }
            }
        }

        static void MeniPosudbe(SistemPosudbe sistem)
        {
            bool ok = true;
            while (ok)
            {
                Console.WriteLine("\n--- SISTEM POSUDBE ---");
                Console.WriteLine("1. Posudi knjigu");
                Console.WriteLine("2. Vrati knjigu");
                Console.WriteLine("3. Prikaži aktivne posudbe");
                Console.WriteLine("4. Historija posudbi za korisnika");
                Console.WriteLine("0. Povratak");
                Console.Write("Izbor: ");

                string izbor = Console.ReadLine();
                Console.WriteLine();

                switch (izbor)
                {
                    case "1": sistem.PosudiKnjigu(); break;
                    case "2": sistem.VratiKnjigu(); break;
                    case "3": sistem.PrikaziAktivnePosudbe(); break;
                    case "4": sistem.PrikaziHistorijuZaKorisnika(); break;
                    case "0": ok = false; break;
                    default: Console.WriteLine("Neispravan izbor!"); break;
                }
            }
        }
    }
}


