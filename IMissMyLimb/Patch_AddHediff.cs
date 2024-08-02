using HarmonyLib;
using RimWorld;
using System;
using Verse;

[HarmonyPatch(typeof(Pawn_HealthTracker))]
[HarmonyPatch("AddHediff")]
[HarmonyPatch(new Type[] { typeof(Hediff), typeof(BodyPartRecord), typeof(DamageInfo?), typeof(DamageWorker.DamageResult) })]
public static class Patch_AddHediff
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

            if (hediff is Hediff_MissingPart missingPart)
            {
                if (missingPart.Part == null)
                {
                    Log.Error("IMissMyLimb: missingPart.Part is null.");
                    return;
                }

                if (CommonUtils.IsFingerOrToe(missingPart.Part))
                {
                    CommonUtils.AssignThought(pawn, ThoughtDef.Named("IMissMyLimb_ColonistLostFingerToe"), missingPart.Part);
                }
                else if (missingPart.Part.def == BodyPartDefOf.Arm)
                {
                    CommonUtils.AssignThought(pawn, ThoughtDef.Named("IMissMyLimb_ColonistLostArm"), missingPart.Part);
                }
                else if (missingPart.Part.def == BodyPartDefOf.Leg)
                {
                    CommonUtils.AssignThought(pawn, ThoughtDef.Named("IMissMyLimb_ColonistLostLeg"), missingPart.Part);
                }
            }
            else if (hediff.Part != null && CommonUtils.IsProsthetic(hediff.def))
            {
                CommonUtils.RemoveNegativeThought(pawn, hediff.Part);
                ThoughtDef thoughtDef = null;
                int stageIndex = 0;

                if (CommonUtils.IsArchotech(hediff.def))
                {
                    if (CommonUtils.IsFingerOrToe(hediff.Part))
                    {
                        thoughtDef = ThoughtDef.Named("IMissMyLimb_ColonistGotProstheticFingerToe");
                        stageIndex = 2;
                    }
                    else if (hediff.Part.def == BodyPartDefOf.Arm)
                    {
                        thoughtDef = ThoughtDef.Named("IMissMyLimb_ColonistGotProstheticArm");
                        stageIndex = 2;
                    }
                    else if (hediff.Part.def == BodyPartDefOf.Leg)
                    {
                        thoughtDef = ThoughtDef.Named("IMissMyLimb_ColonistGotProstheticLeg");
                        stageIndex = 2;
                    }
                }
                else
                {
                    if (CommonUtils.IsFingerOrToe(hediff.Part))
                    {
                        thoughtDef = ThoughtDef.Named("IMissMyLimb_ColonistGotProstheticFingerToe");
                    }
                    else if (hediff.Part.def == BodyPartDefOf.Arm)
                    {
                        thoughtDef = ThoughtDef.Named("IMissMyLimb_ColonistGotProstheticArm");
                    }
                    else if (hediff.Part.def == BodyPartDefOf.Leg)
                    {
                        thoughtDef = ThoughtDef.Named("IMissMyLimb_ColonistGotProstheticLeg");
                    }
                }

                if (thoughtDef != null)
                {
                    var thought = ThoughtMaker.MakeThought(thoughtDef, stageIndex);
                    CommonUtils.ApplyIdeologyFactor(pawn, thought);
                    pawn.needs.mood.thoughts.memories.TryGainMemory(thought);
                }
            }
        }
        catch (Exception ex)
        {
            Log.Error($"IMissMyLimb: Exception in AddHediff Postfix: {ex}");
        }
    }
}
