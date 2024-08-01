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
        // Log.Message("IMissMyLimb: AddHediff Postfix method called.");
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

            string pawnName = pawn.Name?.ToStringFull ?? "unknown";
            string hediffName = hediff.def?.defName ?? "unknown hediff";
            string partName = hediff.Part?.def?.defName ?? "no body part";

            // Log.Message($"IMissMyLimb: Pawn {pawnName} received Hediff {hediffName} on {partName}.");

            if (hediff is Hediff_MissingPart missingPart)
            {
                // Log.Message($"IMissMyLimb: Hediff is a missing part: {missingPart.Part?.def?.defName ?? "unknown part"}");
                if (missingPart.Part == null)
                {
                    Log.Error("IMissMyLimb: missingPart.Part is null.");
                    return;
                }

                if (CommonUtils.IsFingerOrToe(missingPart.Part))
                {
                    // Log.Message("IMissMyLimb: Missing part is a finger or toe.");
                    CommonUtils.AssignThought(pawn, ThoughtDef.Named("ColonistLostFingerToe"), missingPart.Part);
                }
                else if (missingPart.Part.def == BodyPartDefOf.Arm)
                {
                    // Log.Message("IMissMyLimb: Missing part is an arm.");
                    CommonUtils.AssignThought(pawn, ThoughtDef.Named("ColonistLostArm"), missingPart.Part);
                }
                else if (missingPart.Part.def == BodyPartDefOf.Leg)
                {
                    // Log.Message("IMissMyLimb: Missing part is a leg.");
                    CommonUtils.AssignThought(pawn, ThoughtDef.Named("ColonistLostLeg"), missingPart.Part);
                }
            }
            else if (hediff.Part != null && CommonUtils.IsProsthetic(hediff.def))
            {
                // Log.Message($"IMissMyLimb: Hediff is a prosthetic: {hediff.def.defName}");
                CommonUtils.RemoveNegativeThought(pawn, hediff.Part);
                if (CommonUtils.IsFingerOrToe(hediff.Part))
                {
                    // Log.Message("IMissMyLimb: Prosthetic part is a finger or toe.");
                    CommonUtils.AssignThought(pawn, ThoughtDef.Named("ColonistGotProstheticFingerToe"), hediff.Part);
                }
                else if (hediff.Part.def == BodyPartDefOf.Arm)
                {
                    // Log.Message("IMissMyLimb: Prosthetic part is an arm.");
                    CommonUtils.AssignThought(pawn, ThoughtDef.Named("ColonistGotProstheticArm"), hediff.Part);
                }
                else if (hediff.Part.def == BodyPartDefOf.Leg)
                {
                    // Log.Message($"IMissMyLimb: Prosthetic part is a leg: {hediff.def.defName}");
                    CommonUtils.AssignThought(pawn, ThoughtDef.Named("ColonistGotProstheticLeg"), hediff.Part);
                }
            }
            else
            {
                // Log.Message("IMissMyLimb: Hediff does not target a specific body part or is not recognized as a prosthetic.");
            }
        }
        catch (Exception ex)
        {
            Log.Error($"IMissMyLimb: Exception in AddHediff Postfix: {ex}");
        }
    }
}
