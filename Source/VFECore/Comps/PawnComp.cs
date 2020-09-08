using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VFECore.Comps
{
    using Defs;
    using UnityEngine;
    using Verse;

    public class PawnComp : ThingComp
    {
        private bool initialized = false;

        public Dictionary<RenderAddonLayerDef, RenderAddonColorCombination> renderAddonDictionary = new Dictionary<RenderAddonLayerDef, RenderAddonColorCombination>();

        public void Init()
        {
            if (this.initialized)
                return;

            this.initialized = true;
            PawnKindDefExtension extension = (this.parent as Pawn)?.kindDef.GetModExtension<PawnKindDefExtension>();
            if (extension != null)
            {
                if (!extension.renderAddons.NullOrEmpty())
                    for (int i = 0; i < extension.renderAddons.Count; i++)
                    {
                        RenderAddonData addonData = extension.renderAddons[i];
                        if (Rand.Range(0, 100) < addonData.chance)
                        {
                            HashSet<RenderAddonDef> addons = new HashSet<RenderAddonDef>();
                            for (int index = 0; index < addonData.tags.Count; index++)
                            {
                                HashSet<RenderAddonDef> addonsTag = addonData.layer.renderAddons[addonData.tags[index]];
                                foreach (RenderAddonDef renderAddonDef in addonsTag)
                                    addons.Add(renderAddonDef);
                            }
                            RenderAddonDef addon = addons.RandomElementByWeight(def => def.weight);

                            Color color       = Color.white;
                            Color colorSecond = Color.white;
                            switch (addon.coloring)
                            {
                                case RenderAddonColoringMode.Static:
                                    color       = addon.Graphic.Color;
                                    colorSecond = addon.Graphic.ColorTwo;
                                    break;
                                case RenderAddonColoringMode.Custom:
                                    color       = addonData.color?.NewRandomizedColor() ?? Color.white;
                                    colorSecond = addonData.colorTwo?.NewRandomizedColor() ?? Color.white;
                                    break;
                            }
                            Log.Message(color.ToString());
                            this.AddRenderAddon(addonData.layer, addon, color, colorSecond);
                        }
                    }
            }
        }

        public RenderAddonColorCombination GetRenderAddon(RenderAddonLayerDef layer) =>
            this.renderAddonDictionary.ContainsKey(layer) ? this.renderAddonDictionary[layer] : null;

        public void AddRenderAddon(RenderAddonLayerDef layer, RenderAddonDef addon, Color color, Color colorSecond)
        {
            if(!this.renderAddonDictionary.ContainsKey(layer))
                this.renderAddonDictionary.Add(layer, new RenderAddonColorCombination(addon, color, colorSecond));
            else
                this.renderAddonDictionary[layer] = new RenderAddonColorCombination(addon, color, colorSecond);
        }

        public override void PostExposeData()
        {
            base.PostExposeData();
            Scribe_Collections.Look(ref this.renderAddonDictionary, nameof(this.renderAddonDictionary), LookMode.Def, LookMode.Deep);
        }
    }

    public class RenderAddonColorCombination : IExposable
    {
        private Graphic graphic;
        public Graphic Graphic => 
            this.graphic ?? (this.graphic = this.renderAddonDef.Graphic.GetColoredVersion(this.renderAddonDef.Graphic.Shader, this.color, this.colorSecond));

        public RenderAddonDef renderAddonDef;
        public Color color;
        public Color colorSecond;

        public RenderAddonColorCombination(RenderAddonDef renderAddonDef, Color color, Color colorSecond)
        {
            this.renderAddonDef = renderAddonDef;
            this.color = color;
            this.colorSecond = colorSecond;
        }

        public void ClearGraphic()
        {
            this.graphic = null;
        }

        public void ExposeData()
        {
            Scribe_Defs.Look(ref this.renderAddonDef, nameof(this.renderAddonDef));
            Scribe_Values.Look(ref this.color, nameof(this.color), Color.white);
            Scribe_Values.Look(ref this.colorSecond, nameof(this.colorSecond), Color.white);
        }
    }
}
