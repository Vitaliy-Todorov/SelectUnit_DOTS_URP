using System;

[Flags]
public enum CollisionLayers
{
    None = 0,
    Selection = None << 1,
    Ground = Selection << 1,
    Unit = Ground << 1,
}
