using System;
using System.Collections.Generic;
using System.Linq;

namespace PametnaBiblioteka
{
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

    public class SistemPosudbe
    {
        private readonly List<Posudba> posudbe;
        private readonly List<Korisnik> korisnici;
        private readonly List<Knjiga> knjige;
        private int nextPosudbaId;
        private const decimal KaznaPoDanu = 1m;

        public SistemPosudbe(List<Korisnik> korisnici, List<Knjiga> knjige, List<Posudba> postojecePosudbe = null)
        {
            this.korisnici = korisnici;
            this.knjige = knjige;
            posudbe = postojecePosudbe ?? new List<Posudba>();
            nextPosudbaId = posudbe.Any() ? posudbe.Max(p => p.Id) + 1 : 1;
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

                Console.WriteLine($"[{p.Id}] {korisnik?.Ime} {korisnik?.Prezime} - \"{knjiga?.Naslov}\" | Posuđeno: {p.DatumPosudbe:dd.MM.yyyy} | Rok: {p.RokVracanja:dd.MM.yYYY}");
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
}
