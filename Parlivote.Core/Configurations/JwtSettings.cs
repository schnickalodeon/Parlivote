﻿using System;

namespace Parlivote.Core.Configurations;

public class JwtSettings
{
    public string Secret { get; set; }
    public TimeSpan TokenLifeTime { get; set; }
}