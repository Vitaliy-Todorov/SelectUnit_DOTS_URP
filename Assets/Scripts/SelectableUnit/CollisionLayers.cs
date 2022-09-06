using System;

[Flags]
public enum CollisionLayers
{
    None = 0,
    Defolt = 1,
    Ground = Defolt << 1,
    Unit = Ground << 1,
    Selection = Unit << 1,
}
