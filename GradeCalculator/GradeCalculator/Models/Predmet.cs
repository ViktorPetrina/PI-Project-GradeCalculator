using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace GradeCalculator.Models;

public partial class Predmet
{
    [JsonIgnore]
    public int Idpredmet { get; set; }

    public string Naziv { get; set; } = null!;

    public double? Prosjek { get; set; }

    public int? GodinaId { get; set; }

    [JsonIgnore]
    public virtual Godina? Godina { get; set; }

    public virtual ICollection<Ocjena> Ocjenas { get; set; } = new List<Ocjena>();
}
