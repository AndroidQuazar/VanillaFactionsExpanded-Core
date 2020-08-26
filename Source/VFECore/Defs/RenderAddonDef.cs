using System.Collections.Generic;

namespace VFECore.Defs
{
    using RimWorld;
    using Verse;

    public class RenderAddonDef : Def
    {
        public List<RenderAddonLayerDef> layers;

        private GraphicData graphicData;

        public RenderAddonColoringMode coloring = RenderAddonColoringMode.Custom;

        public Graphic Graphic =>
            this.graphicData?.Graphic;
    }

    public class RenderAddonLayerDef : Def
    {
        public float offset = 0f;
    }

    public enum RenderAddonColoringMode
    {
        Static,
        Custom,
        Hair
    }
}
