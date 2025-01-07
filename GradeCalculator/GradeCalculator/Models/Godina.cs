using System;
using System.Collections.Generic;

namespace GradeCalculator.Models;

public partial class Godina
{
    public int Idgodina { get; set; }

    public string Naziv { get; set; } = null!;

    public double Prosjek { get; set; }

    public int? KorisnikId { get; set; }

    public virtual Korisnik? Korisnik { get; set; }

    public virtual ICollection<Predmet> Predmets { get; set; } = new List<Predmet>();
}
