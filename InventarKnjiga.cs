    public override string ToString()
    {
        return $"{Id}. \"{Naslov}\" - {Autor} [{Zanr}] - {(Dostupna ? "Dostupna" : "Nedostupna")}";
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

    public List<Knjiga> VratiSveKnjige()
    {
        return knjige;
