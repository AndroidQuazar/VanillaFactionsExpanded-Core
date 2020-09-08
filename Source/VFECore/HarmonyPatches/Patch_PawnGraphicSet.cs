﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using UnityEngine;
using Verse;
using RimWorld;
using HarmonyLib;

namespace VFECore
{
    using Comps;
    using Defs;

    public static class Patch_PawnGraphicSet
    {
        [HarmonyPatch(typeof(PawnGraphicSet), nameof(PawnGraphicSet.ResolveAllGraphics))]
        public static class ResolveAllGraphics
        {

            public static void Postfix(PawnGraphicSet __instance)
            {
                var faction = __instance.pawn.Faction;
                // If the pawn's a pack animal and is part of a medieval faction, use medieval pack texture if applicable
                if (faction != null && __instance.pawn.RaceProps.packAnimal)
                {
                    var factionDefExtension = FactionDefExtension.Get(faction.def);
                    if (!factionDefExtension.packAnimalTexNameSuffix.NullOrEmpty())
                    {
                        var medievalPackTexture = ContentFinder<Texture2D>.Get(__instance.nakedGraphic.path + $"{factionDefExtension.packAnimalTexNameSuffix}_south", false);
                        if (medievalPackTexture != null)
                            __instance.packGraphic = GraphicDatabase.Get<Graphic_Multi>(__instance.nakedGraphic.path + factionDefExtension.packAnimalTexNameSuffix, ShaderDatabase.CutoutComplex, __instance.nakedGraphic.drawSize, __instance.pawn.Faction.Color);
                    }
                }


                PawnComp pawnComp = __instance.pawn.GetComp<PawnComp>();

                if (pawnComp != null)
                {
                    pawnComp.Init();
                    foreach (RenderAddonColorCombination addonColorCombination in pawnComp.renderAddonDictionary.Values)
                        if (addonColorCombination.renderAddonDef.coloring == RenderAddonColoringMode.Hair)
                        {
                            addonColorCombination.color = __instance.pawn.story.hairColor;
                            addonColorCombination.ClearGraphic();
                        }
                }
            }

        }

    }

}
