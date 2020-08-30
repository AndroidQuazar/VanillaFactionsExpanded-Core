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
        public override void PostSpawnSetup(bool respawningAfterLoad)
        {
            base.PostSpawnSetup(respawningAfterLoad);
            AddRenderAddon(DefDatabase<RenderAddonLayerDef>.GetNamed("facepaint"), DefDatabase<RenderAddonDef>.GetNamed("VFEV_Facepaint_Barbarian"), Color.white, Color.white);
        }

        private Dictionary<RenderAddonLayerDef, RenderAddonColorCombination> renderAddonDictionary = new Dictionary<RenderAddonLayerDef, RenderAddonColorCombination>();

        public RenderAddonColorCombination GetRenderAddon(RenderAddonLayerDef layer) =>
            this.renderAddonDictionary.ContainsKey(layer) ? this.renderAddonDictionary[layer] : null;

        public void AddRenderAddon(RenderAddonLayerDef layer, RenderAddonDef addon, Color color, Color colorSecond)
        {
            Log.Message(layer?.defName + " " + addon?.defName);

            if(!this.renderAddonDictionary.ContainsKey(layer))
                this.renderAddonDictionary.Add(layer, new RenderAddonColorCombination(addon, color, colorSecond));
            else
                this.renderAddonDictionary[layer] = new RenderAddonColorCombination(addon, color, colorSecond);

            Log.Message(this.renderAddonDictionary[layer]?.renderAddonDef?.defName);
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

        public void ExposeData()
        {
            Scribe_Defs.Look(ref this.renderAddonDef, nameof(this.renderAddonDef));
            Scribe_Values.Look(ref this.color, nameof(this.color), Color.white);
            Scribe_Values.Look(ref this.colorSecond, nameof(this.colorSecond), Color.white);
        }
    }
}
