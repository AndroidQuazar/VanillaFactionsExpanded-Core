using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Verse;
using RimWorld;

namespace VFECore
{
    using Comps;

    [StaticConstructorOnStartup]
    public static class StaticConstructorClass
    {

        static StaticConstructorClass()
        {
            // Cache setters
            PawnShieldGenerator.Reset();
            ScenPartUtility.SetCache();

            var thingDefs = DefDatabase<ThingDef>.AllDefsListForReading;
            for (int i = 0; i < thingDefs.Count; i++)
            {
                var tDef = thingDefs[i];
                // Add beard comps to all eligible ThingDefs
                if (typeof(Pawn).IsAssignableFrom(tDef.thingClass) && tDef.race != null && tDef.race.Humanlike)
                {
                    if (tDef.comps == null)
                        tDef.comps = new List<CompProperties>();
                    tDef.comps.Add(new CompProperties(typeof(PawnComp)));
                }
            }
        }

    }

}
