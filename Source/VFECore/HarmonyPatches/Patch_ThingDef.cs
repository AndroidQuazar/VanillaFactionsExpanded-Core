﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using UnityEngine;
using Verse;
using RimWorld;
using Harmony;

namespace VFECore
{

    public static class Patch_ThingDef
    {

        [HarmonyPatch(typeof(ThingDef), nameof(ThingDef.SpecialDisplayStats))]
        public static class SetFaction
        {

            public static void Postfix(ThingDef __instance, ref IEnumerable<StatDrawEntry> __result)
            {
                // Weapons get a readout for if they are usable with shields
                if (__instance.IsWeapon)
                {
                    __result = __result.Add(new StatDrawEntry(StatCategoryDefOf.Weapon, "VanillaFactionsExpanded.UsableWithShield".Translate(), ThingDefExtension.Get(__instance).usableWithShields.ToStringYesNo()));
                }
            }

        }

    }

}