using System.Collections.Generic;

namespace VFECore.Defs
{
    using System.IO;
    using RimWorld;
    using Verse;

    public class RenderAddonDef : Def
    {
        public List<RenderAddonLayerDef> layers = new List<RenderAddonLayerDef>();

        public List<string> tags = new List<string>();

        public float weight = 1f;

        public float workToChange = 150f;

        private GraphicData graphicData;

        public RenderAddonColoringMode coloring = RenderAddonColoringMode.Custom;
        public RenderAddonDependency dependency = RenderAddonDependency.Head;

        public Graphic Graphic =>
            this.graphicData?.Graphic;

        public override void ResolveReferences()
        {
            base.ResolveReferences();
            for (int i = 0; i < this.layers.Count; i++)
            {
                RenderAddonLayerDef layer = this.layers[i];
                for (int i1 = 0; i1 < this.tags.Count; i1++)
                {
                    string tag = this.tags[i1];
                    if(!layer.renderAddons.ContainsKey(tag))
                        layer.renderAddons.Add(tag, new HashSet<RenderAddonDef>());
                    layer.renderAddons[tag].Add(this);
                }
            }
        }
    }

    public class RenderAddonLayerDef : Def
    {
        public Dictionary<string, HashSet<RenderAddonDef>> renderAddons = new Dictionary<string, HashSet<RenderAddonDef>>();

        public float offset = 0f;
    }

    public enum RenderAddonColoringMode
    {
        Static,
        Custom,
        Hair
    }
    public enum RenderAddonDependency
    {
        Body,
        Head,
        None
    }
}
