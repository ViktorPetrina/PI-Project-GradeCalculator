﻿using System;
using System.Collections.Generic;

namespace GradeCalculator.Models;

public partial class Log
{
    public int Idocjena { get; set; }

    public string Opis { get; set; } = null!;

    public DateTime Vrijeme { get; set; }
}