using System.Collections.Generic;

namespace VFECore.Defs
{
    using RimWorld;
    using Verse;

    public class RenderAddonDef : Def
    {
        public List<RenderAddonLayerDef> layers = new List<RenderAddonLayerDef>();

        public List<string> tags = new List<string>();

        private GraphicData graphicData;

        public RenderAddonColoringMode coloring = RenderAddonColoringMode.Custom;
        public RenderAddonDependency dependency = RenderAddonDependency.Head;

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
    public enum RenderAddonDependency
    {
        Body,
        Head,
        None
    }
}
