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
        private readonly List<Posudba> _posudbe;
        private readonly List<Korisnik> _korisnici;
        private readonly List<Knjiga> _knjige;

        private int _nextPosudbaId = 1;

        private const decimal KaznaPoDanu = 1m;

        public SistemPosudbe(List<Korisnik> korisnici, List<Knjiga> knjige)
        {
            _korisnici = korisnici ?? throw new ArgumentNullException(nameof(korisnici));
            _knjige = knjige ?? throw new ArgumentNullException(nameof(knjige));
            _posudbe = new List<Posudba>();
        }

        public bool PosudiKnjigu(int korisnikId, int knjigaId)
        {
            var korisnik = _korisnici.FirstOrDefault(k => k.Id == korisnikId);
            if (korisnik == null)
            {
                Console.WriteLine("❌ Korisnik sa zadatim ID nije pronađen.");
                return false;
            }

            var knjiga = _knjige.FirstOrDefault(k => k.Id == knjigaId);
            if (knjiga == null)
            {
                Console.WriteLine("❌ Knjiga sa zadatim ID nije pronađena.");
                return false;
            }

            if (!knjiga.Dostupna)
            {
                Console.WriteLine("❌ Knjiga trenutno nije dostupna za posudbu.");
                return false;
            }

            var datumPosudbe = DateTime.Now;
            var rok = datumPosudbe.AddDays(14); // npr. rok vraćanja 14 dana

            var novaPosudba = new Posudba
            {
                Id = _nextPosudbaId++,
                KorisnikId = korisnik.Id,
                KnjigaId = knjiga.Id,
                DatumPosudbe = datumPosudbe,
                RokVracanja = rok,
                DatumVracanja = null,
                Kazna = 0
            };

            _posudbe.Add(novaPosudba);
            knjiga.Dostupna = false;

            Console.WriteLine($"✅ Knjiga \"{knjiga.Naslov}\" je posuđena korisniku {korisnik.Ime} {korisnik.Prezime}.");
            Console.WriteLine($"   Rok za vraćanje: {rok:dd.MM.yyyy}.");

            return true;
        }

        public bool VratiKnjigu(int korisnikId, int knjigaId)
        {
            var posudba = _posudbe
                .LastOrDefault(p => p.KorisnikId == korisnikId &&
                                    p.KnjigaId == knjigaId &&
                                    p.Aktivna);

            if (posudba == null)
            {
                Console.WriteLine("❌ Nije pronađena aktivna posudba za datog korisnika i knjigu.");
                return false;
            }

            var knjiga = _knjige.FirstOrDefault(k => k.Id == knjigaId);
            var korisnik = _korisnici.FirstOrDefault(k => k.Id == korisnikId);

            var danas = DateTime.Now;
            posudba.DatumVracanja = danas;

            if (danas.Date > posudba.RokVracanja.Date)
            {
                int daniKasnjenja = (danas.Date - posudba.RokVracanja.Date).Days;
                posudba.Kazna = daniKasnjenja * KaznaPoDanu;
            }

            if (knjiga != null)
                knjiga.Dostupna = true;

            Console.WriteLine($"📚 Knjiga \"{knjiga?.Naslov}\" vraćena {danas:dd.MM.yyyy} od strane {korisnik?.Ime} {korisnik?.Prezime}.");

            if (posudba.Kazna > 0)
            {
                Console.WriteLine($"💰 Korisnik kasni sa vraćanjem. Kazna iznosi: {posudba.Kazna} KM.");
            }
            else
            {
                Console.WriteLine("✅ Nema kazne za kašnjenje.");
            }

            return true;
        }

        public void PrikaziAktivnePosudbe()
        {
            var aktivne = _posudbe.Where(p => p.Aktivna).ToList();

            if (!aktivne.Any())
            {
                Console.WriteLine("📭 Nema aktivnih posudbi.");
                return;
            }

            Console.WriteLine("=== AKTIVNE POSUDBE ===");
            foreach (var p in aktivne)
            {
                var korisnik = _korisnici.FirstOrDefault(k => k.Id == p.KorisnikId);
                var knjiga = _knjige.FirstOrDefault(k => k.Id == p.KnjigaId);

                Console.WriteLine(
                    $"[PosudbaID: {p.Id}] Korisnik: {korisnik?.Ime} {korisnik?.Prezime} | " +
                    $"Knjiga: \"{knjiga?.Naslov}\" | " +
                    $"Posuđeno: {p.DatumPosudbe:dd.MM.yyyy} | Rok: {p.RokVracanja:dd.MM.yyyy}");
            }
        }

        public void PrikaziHistorijuZaKorisnika(int korisnikId)
        {
            var korisnik = _korisnici.FirstOrDefault(k => k.Id == korisnikId);
            if (korisnik == null)
            {
                Console.WriteLine("❌ Korisnik nije pronađen.");
                return;
            }

            var historija = _posudbe.Where(p => p.KorisnikId == korisnikId).ToList();
            if (!historija.Any())
            {
                Console.WriteLine($"Korisnik {korisnik.Ime} {korisnik.Prezime} nema posudbi u historiji.");
                return;
            }

            Console.WriteLine($"=== HISTORIJA POSUDBI ZA: {korisnik.Ime} {korisnik.Prezime} ===");
            foreach (var p in historija)
            {
                var knjiga = _knjige.FirstOrDefault(k => k.Id == p.KnjigaId);
                string status = p.Aktivna ? "AKTIVNA" : "ZAVRŠENA";

                Console.WriteLine(
                    $"Knjiga: \"{knjiga?.Naslov}\" | " +
                    $"Posuđeno: {p.DatumPosudbe:dd.MM.yyyy} | " +
                    $"Rok: {p.RokVracanja:dd.MM.yyyy} | " +
                    $"Vraćeno: {(p.DatumVracanja.HasValue ? p.DatumVracanja.Value.ToString("dd.MM.yyyy") : "-")} | " +
                    $"Kazna: {p.Kazna} KM | Status: {status}");
            }
        }

        public IReadOnlyList<Posudba> VratiSvePosudbe()
        {
            return _posudbe.AsReadOnly();
        }

        public List<Posudba> VratiAktivnePosudbe()
        {
            return _posudbe.Where(p => p.Aktivna).ToList();
        }

        public List<Posudba> VratiHistorijuPosudbiZaKorisnika(int korisnikId)
        {
            return _posudbe.Where(p => p.KorisnikId == korisnikId).ToList();
        }
    }
}
