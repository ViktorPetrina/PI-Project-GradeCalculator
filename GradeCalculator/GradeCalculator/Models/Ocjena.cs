using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace GradeCalculator.Models;

public partial class Ocjena
{
    [JsonIgnore]
    public int Idocjena { get; set; }

    public int? Vrijednost { get; set; }

    public int? PredmetId { get; set; }

    [JsonIgnore]
    public virtual Predmet? Predmet { get; set; }
}
