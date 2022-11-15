﻿using System.Collections.Generic;
using Newtonsoft.Json;

namespace Auto.Data.Entities;

public class Manufacturer
{
    public Manufacturer()
    {
        Models = new HashSet<Model>();
    }

    public string Code { get; set; }
    public string Name { get; set; }

    [JsonIgnore] public virtual ICollection<Model> Models { get; set; }
}