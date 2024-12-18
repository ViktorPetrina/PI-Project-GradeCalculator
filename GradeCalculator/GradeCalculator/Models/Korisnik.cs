using System;
using System.Collections.Generic;

namespace GradeCalculator.Models;

public partial class Korisnik
{
    public int Idkorisnik { get; set; }

    public string KorisnickoIme { get; set; } = null!;

    public string Eposta { get; set; } = null!;

    public string LozinkaHash { get; set; } = null!;

    public string LozinkaSalt { get; set; } = null!;

    public double UkupnaOcjena { get; set; }

    public int UlogaId { get; set; }

    public virtual ICollection<Godina> Godinas { get; set; } = new List<Godina>();

    public virtual Uloga Uloga { get; set; } = null!;
}
