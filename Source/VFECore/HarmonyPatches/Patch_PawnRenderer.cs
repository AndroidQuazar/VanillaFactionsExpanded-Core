using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VFECore
{
    using System.Reflection;
    using System.Reflection.Emit;
    using Comps;
    using Defs;
    using HarmonyLib;
    using RimWorld;
    using UnityEngine;
    using Verse;

    public static class Patch_PawnRenderer
    {
        [HarmonyPatch(typeof(PawnRenderer), "RenderPawnInternal", typeof(Vector3), typeof(float), typeof(bool), typeof(Rot4), typeof(Rot4), typeof(RotDrawMode), typeof(bool), typeof(bool), typeof(bool))]
        public static class Patch_RenderPawnInternal
        {
            public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
            {
                List<CodeInstruction> instructionList = instructions.ToList();

                MethodInfo animalInfo = AccessTools.PropertyGetter(typeof(RaceProperties), nameof(RaceProperties.Animal));


                for (int i = 0; i < instructionList.Count; i++)
                {
                    CodeInstruction instruction = instructionList[i];

                    if (instruction.opcode == OpCodes.Ldarg_S && instructionList[i+1].opcode == OpCodes.Brtrue_S && instructionList[i+5].OperandIs(animalInfo))
                    {
                        yield return instruction; // portrait
                        yield return new CodeInstruction(opcode: OpCodes.Ldarg_1);
                        yield return new CodeInstruction(opcode: OpCodes.Ldarg_0);
                        yield return new CodeInstruction(opcode: OpCodes.Ldfld, operand: AccessTools.Field(type: typeof(PawnRenderer), name: "pawn"));
                        yield return new CodeInstruction(opcode: OpCodes.Ldloc_0);             // quat
                        yield return new CodeInstruction(opcode: OpCodes.Ldarg_S, operand: 4); // bodyfacing
                        yield return new CodeInstruction(opcode: OpCodes.Ldarg_S, operand: 9); //invisible
                        yield return new CodeInstruction(opcode: OpCodes.Ldarg_S, operand: 3); //renderbody
                        yield return new CodeInstruction(opcode: OpCodes.Ldarg_S, operand: 8); //headStump
                        yield return new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(Patch_RenderPawnInternal), nameof(DrawCall)));
                        instruction = new CodeInstruction(instruction);
                        instruction.labels.Clear();
                    }

                    yield return instruction;
                }
            }

            public static void DrawCall(bool portrait, Vector3 vector, Pawn pawn, Quaternion quat, Rot4 rotation, bool invisible, bool renderBody, bool headStump)
            {
                PawnComp pawnComp = pawn.GetComp<PawnComp>();

                if (invisible || pawnComp == null)
                    return;


                foreach (RenderAddonLayerDef layerDef in DefDatabase<RenderAddonLayerDef>.AllDefsListForReading)
                {
                    RenderAddonColorCombination addon = pawnComp.GetRenderAddon(layerDef);
                    if (addon != null)
                    {
                        switch (addon.renderAddonDef.dependency)
                        {
                            case RenderAddonDependency.Body:
                                if (!renderBody)
                                    continue;
                                break;
                            case RenderAddonDependency.Head:
                                if(headStump)
                                    continue;
                                break;
                            default:
                                break;
                        }

                        Vector3 layerOffset = new Vector3(0f, layerDef.offset, 0f);

                        //                                                                                        Angle calculation to not pick the shortest, taken from Quaternion.Angle and modified
                        GenDraw.DrawMeshNowOrLater(mesh: addon.Graphic.MeshAt(rot: rotation), loc: vector + (layerOffset + addon.renderAddonDef.Graphic.DrawOffset(rotation)).RotatedBy(angle: Mathf.Acos(f: Quaternion.Dot(a: Quaternion.identity, b: quat)) * 2f * 57.29578f),
                                                   quat: Quaternion.AngleAxis(angle: 0f, axis: Vector3.up) * quat, mat: addon.Graphic.MatAt(rot: rotation), drawNow: portrait);
                    }
                }
            }
        }
    }
}
