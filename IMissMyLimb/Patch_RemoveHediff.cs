using HarmonyLib;
using RimWorld;
using System;
using Verse;

[HarmonyPatch(typeof(Pawn_HealthTracker))]
[HarmonyPatch("RemoveHediff")]
public static class Patch_RemoveHediff
{
    [HarmonyPostfix]
    public static void Postfix(Pawn_HealthTracker __instance, Hediff hediff)
    {
        try
        {
            if (__instance == null)
            {
                Log.Error("IMissMyLimb: __instance is null.");
                return;
            }

            Pawn pawn = Traverse.Create(__instance).Field("pawn").GetValue<Pawn>();
            if (pawn == null)
            {
                Log.Error("IMissMyLimb: Pawn is null.");
                return;
            }

            if (hediff == null)
            {
                Log.Error("IMissMyLimb: Hediff is null.");
                return;
            }

            if (hediff.Part != null && hediff is Hediff_MissingPart)
            {
                ThoughtUtils.RemoveNegativeThought(pawn, hediff.Part);
            }
            else if (hediff.Part != null && CommonUtils.IsProsthetic(hediff.def))
            {
                ThoughtUtils.RemoveNegativeThought(pawn, hediff.Part);

                ThoughtDef thoughtDef = null;

                if (CommonUtils.IsFingerOrToe(hediff.Part))
                {
                    thoughtDef = ThoughtDef.Named("IMissMyLimb_LimbGrewBackFingerToe");
                }
                else if (hediff.Part.def == BodyPartDefOf.Arm)
                {
                    thoughtDef = ThoughtDef.Named("IMissMyLimb_LimbGrewBackArm");
                }
                else if (hediff.Part.def == BodyPartDefOf.Leg)
                {
                    thoughtDef = ThoughtDef.Named("IMissMyLimb_LimbGrewBackLeg");
                }

                if (thoughtDef != null)
                {
                    var thought = ThoughtMaker.MakeThought(thoughtDef) as Thought_Memory;
                    if (thought != null)
                    {
                        ThoughtUtils.ApplyIdeologyFactor(pawn, thought);
                        ThoughtUtils.ApplyTraitModifiers(pawn, thought);
                        pawn.needs.mood.thoughts.memories.TryGainMemory(thought);
                    }
                }
            }
        }
        catch (Exception ex)
        {
            Log.Error($"IMissMyLimb: Exception in RemoveHediff Postfix: {ex}");
        }
    }
}
