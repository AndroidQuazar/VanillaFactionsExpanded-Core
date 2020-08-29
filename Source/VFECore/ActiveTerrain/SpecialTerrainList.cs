using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Verse;

namespace VFECore
{
    public class SpecialTerrainList : MapComponent
    {
    	//.ctor... need we say more
        public SpecialTerrainList(Map map) : base(map) { }
        
        public Dictionary<IntVec3, TerrainInstance> terrains = new Dictionary<IntVec3, TerrainInstance>();

        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Collections.Look(ref terrains, "terrains", LookMode.Value, LookMode.Deep);
        }

        /// <summary>
        /// Updater for terrains
        /// </summary>
        public override void MapComponentUpdate()
        {
            base.MapComponentUpdate();
            foreach (var terr in terrains)
            {
                terr.Value.Update();
            }
        }

        /// <summary>
        /// Ticker for terrains
        /// </summary>
        public override void MapComponentTick()
        {
            base.MapComponentTick();
			foreach (var terr in terrains)
			{
                if (terr.Value.def.tickerType == TickerType.Normal)
                {
                    terr.Value.Tick();
                }
                else if (terr.Value.def.tickerType == TickerType.Rare && Find.TickManager.TicksGame % 250 == 0)
                {
                    terr.Value.TickRare();
                }
                else if (terr.Value.def.tickerType == TickerType.Long && Find.TickManager.TicksGame % 2000 == 0)
                {
                    terr.Value.TickLong();
                }
            }
        }
        
        public override void FinalizeInit()
        {
            base.FinalizeInit();
            RefreshAllCurrentTerrain();
            CallPostLoad();
        }

        public void CallPostLoad()
        {
            foreach (var k in terrains.Keys)
            {
                terrains[k].PostLoad();
            }
        }
		
        /// <summary>
        /// Registers terrain currently present to terrain list, called on init
        /// </summary>
        public void RefreshAllCurrentTerrain()
        {
            foreach (var cell in map) //Map is IEnumerable...
            {
                TerrainDef terrain = map.terrainGrid.TerrainAt(cell);
                if (terrain is ActiveTerrainDef special)
                {
                    RegisterAt(special, cell);
                }
            }
        }

		public void RegisterAt(ActiveTerrainDef special, int i)
		{
			RegisterAt(special, map.cellIndices.IndexToCell(i));
		}

		public void RegisterAt(ActiveTerrainDef special, IntVec3 cell)
		{
            if (!terrains.ContainsKey(cell))
            {
                var newTerr = special.MakeTerrainInstance(map, cell);
                newTerr.Init();
                terrains.Add(cell, newTerr);
            }
        }
        
        public void Notify_RemovedTerrainAt(IntVec3 c)
        {
        	var terr = terrains[c];
        	terrains.Remove(c);
        	terr.PostRemove();
        }
    }
}
