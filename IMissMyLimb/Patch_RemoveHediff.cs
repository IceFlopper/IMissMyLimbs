using HarmonyLib;
using System;
using Verse;

[HarmonyPatch(typeof(Pawn_HealthTracker))]
[HarmonyPatch("RemoveHediff")]
public static class Patch_RemoveHediff
{
    [HarmonyPostfix]
    public static void Postfix(Pawn_HealthTracker __instance, Hediff hediff)
    {
        Log.Message("IMissMyLimb: RemoveHediff Postfix method called.");
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

            Log.Message($"IMissMyLimb: Pawn {pawnName} had Hediff {hediffName} removed from {partName}.");

            if (hediff.Part != null && hediff is Hediff_MissingPart)
            {
                Log.Message($"IMissMyLimb: Removed missing part hediff from {partName}.");
                CommonUtils.RemoveNegativeThought(pawn, hediff.Part);
            }
        }
        catch (Exception ex)
        {
            Log.Error($"IMissMyLimb: Exception in RemoveHediff Postfix: {ex}");
        }
    }
}
