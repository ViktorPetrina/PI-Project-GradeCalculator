using System;
using System.Collections.Generic;

namespace GradeCalculator.Models;

public partial class Ocjena
{
    public int Idocjena { get; set; }

    public int? Vrijednost { get; set; }

    public int? PredmetId { get; set; }

    public virtual Predmet? Predmet { get; set; }
}
