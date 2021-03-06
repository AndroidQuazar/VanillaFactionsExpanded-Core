﻿using RimWorld;
using System;
using System.Collections.Generic;
using Verse;

namespace KCSG
{
    internal class GenStep_CustomStructureGen : GenStep
    {
        public override int SeedPart
        {
            get
            {
                return 916595355;
            }
        }

        public override void Generate(Map map, GenStepParams parms)
        {
            StructureLayoutDef structureLayoutDef = structureLayoutDefs.RandomElement();

            KCSG_Utilities.HeightWidthFromLayout(structureLayoutDef, out int h, out int w);
            CellRect cellRect = CellRect.CenteredOn(map.Center, w, h);

            foreach (List<String> item in structureLayoutDef.layouts)
            {
                KCSG_Utilities.GenerateRoomFromLayout(item, cellRect, map, structureLayoutDef);
            }

            // Flood refog
            this.SetAllFogged(map);
            foreach (IntVec3 loc in map.AllCells)
            {
                map.mapDrawer.MapMeshDirty(loc, MapMeshFlag.FogOfWar);
            }

            // Remove power cable not connected to a powered grid
            map.listerBuildings.allBuildingsNonColonist.RemoveAll(b => b.TryGetComp<CompPowerTransmitter>() is CompPowerTransmitter cp && cp != null && cp.Props.transmitsPower == true && !cp.PowerNet.HasActivePowerSource);
        }

        internal void SetAllFogged(Map map)
        {
            CellIndices cellIndices = map.cellIndices;
            if (map.fogGrid?.fogGrid != null)
            {
                foreach (IntVec3 c in map.AllCells)
                {
                    map.fogGrid.fogGrid[cellIndices.CellToIndex(c)] = true;
                }
                if (Current.ProgramState == ProgramState.Playing)
                {
                    map.roofGrid.Drawer.SetDirty();
                }
            }
        }

        public List<StructureLayoutDef> structureLayoutDefs = new List<StructureLayoutDef>();
    }
}