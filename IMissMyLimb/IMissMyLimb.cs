using System;
using System.Linq;
using Verse;
using RimWorld;
using HarmonyLib;

namespace IMissMyLimb
{
    [StaticConstructorOnStartup]
    internal static class IMissMyLimb
    {
        static IMissMyLimb()
        {
            var harmony = new Harmony("com.Lewkah0.immissmylimb");
            try
            {
                harmony.PatchAll();
                Log.Message("IMissMyLimb: Harmony patches applied.");
            }
            catch (Exception ex)
            {
                Log.Error($"IMissMyLimb: Error applying Harmony patches: {ex}");
            }
        }
    }
}

[HarmonyPatch(typeof(Pawn_HealthTracker))]
[HarmonyPatch("AddHediff")]
[HarmonyPatch(new Type[] { typeof(Hediff), typeof(BodyPartRecord), typeof(DamageInfo?), typeof(DamageWorker.DamageResult) })]
public static class Patch_Pawn_HealthTracker_AddHediff
{
    [HarmonyPostfix]
    public static void Postfix(Pawn_HealthTracker __instance, Hediff hediff)
    {
        Log.Message("IMissMyLimb: Postfix method called.");
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

            string pawnName = pawn.Name?.ToStringFull ?? "unknown";
            string hediffName = hediff?.def?.defName ?? "unknown hediff";
            string partName = hediff?.Part?.def?.defName ?? "no body part";

            Log.Message($"IMissMyLimb: Pawn {pawnName} received Hediff {hediffName} on {partName}.");

            if (hediff is Hediff_MissingPart missingPart)
            {
                Log.Message($"IMissMyLimb: Hediff is a missing part: {missingPart.Part?.def?.defName ?? "unknown part"}");
                if (missingPart.Part == null)
                {
                    Log.Error("IMissMyLimb: missingPart.Part is null.");
                    return;
                }

                if (IsFingerOrToe(missingPart.Part))
                {
                    Log.Message("IMissMyLimb: Missing part is a finger or toe.");
                    AssignThought(pawn, ThoughtDef.Named("ColonistLostFingerToe"), missingPart.Part);
                }
                else if (missingPart.Part.def == BodyPartDefOf.Arm)
                {
                    Log.Message("IMissMyLimb: Missing part is an arm.");
                    AssignThought(pawn, ThoughtDef.Named("ColonistLostArm"), missingPart.Part);
                }
                else if (missingPart.Part.def == BodyPartDefOf.Leg)
                {
                    Log.Message("IMissMyLimb: Missing part is a leg.");
                    AssignThought(pawn, ThoughtDef.Named("ColonistLostLeg"), missingPart.Part);
                }
            }
            else if (hediff.Part != null && IsProsthetic(hediff.def))
            {
                Log.Message($"IMissMyLimb: Hediff is a prosthetic: {hediff.def.defName}");
                RemoveNegativeThought(pawn, hediff.Part);
                if (IsFingerOrToe(hediff.Part))
                {
                    Log.Message("IMissMyLimb: Prosthetic part is a finger or toe.");
                    AssignThought(pawn, ThoughtDef.Named("ColonistGotProstheticFingerToe"), hediff.Part);
                }
                else if (hediff.Part.def == BodyPartDefOf.Arm)
                {
                    Log.Message("IMissMyLimb: Prosthetic part is an arm.");
                    AssignThought(pawn, ThoughtDef.Named("ColonistGotProstheticArm"), hediff.Part);
                }
                else if (hediff.Part.def == BodyPartDefOf.Leg)
                {
                    Log.Message($"IMissMyLimb: Prosthetic part is a leg: {hediff.def.defName}");
                    AssignThought(pawn, ThoughtDef.Named("ColonistGotProstheticLeg"), hediff.Part);
                }
            }
            else
            {
                Log.Message("IMissMyLimb: Hediff does not target a specific body part or is not recognized as a prosthetic.");
            }
        }
        catch (Exception ex)
        {
            Log.Error($"IMissMyLimb: Exception in AddHediff Postfix: {ex}");
        }
    }

    private static bool IsFingerOrToe(BodyPartRecord part)
    {
        if (part == null)
        {
            Log.Error("IMissMyLimb: BodyPartRecord is null.");
            return false;
        }
        bool result = part.def.defName.Contains("Finger") || part.def.defName.Contains("Toe");
        Log.Message($"IMissMyLimb: IsFingerOrToe check for {part.def.defName}: {result}");
        return result;
    }

    private static void AssignThought(Pawn pawn, ThoughtDef thoughtDef, BodyPartRecord part)
    {
        if (thoughtDef == null)
        {
            Log.Error($"IMissMyLimb: ThoughtDef is null.");
            return;
        }

        if (part == null)
        {
            Log.Error("IMissMyLimb: BodyPartRecord is null in AssignThought.");
            return;
        }

        int stageIndex = 0;
        if (pawn.health.hediffSet.GetMissingPartsCommonAncestors().Count(x => x.Part.def == part.def) > 1)
        {
            stageIndex = 1; // Use second stage for multiple lost parts of the same type
        }

        Log.Message($"IMissMyLimb: Assigning thought {thoughtDef.defName} at stage {stageIndex} to pawn {pawn.Name.ToStringFull} for part {part.def.defName}");

        var thought = ThoughtMaker.MakeThought(thoughtDef, stageIndex);
        pawn.needs?.mood?.thoughts?.memories?.TryGainMemory(thought);
    }

    private static bool IsProsthetic(HediffDef hediffDef)
    {
        if (hediffDef == null)
        {
            Log.Error("IMissMyLimb: HediffDef is null.");
            return false;
        }

        if (hediffDef.comps != null)
        {
            foreach (var comp in hediffDef.comps)
            {
                if (comp is HediffCompProperties_VerbGiver || comp is HediffCompProperties_Disappears)
                {
                    Log.Message($"IMissMyLimb: {hediffDef.defName} is recognized as a prosthetic.");
                    return true;
                }

                // Additional checks for common prosthetic properties
                if (comp is HediffCompProperties_SeverityPerDay || comp is HediffCompProperties_Immunizable || comp is HediffCompProperties_TendDuration)
                {
                    Log.Message($"IMissMyLimb: {hediffDef.defName} has a common prosthetic component.");
                    return true;
                }
            }
        }

        // Additional checks for known prosthetic keywords
        if (hediffDef.defName.Contains("Prosthetic") || hediffDef.defName.Contains("Bionic") || hediffDef.defName.Contains("SimpleProsthetic"))
        {
            Log.Message($"IMissMyLimb: {hediffDef.defName} is recognized by keyword as a prosthetic.");
            return true;
        }

        Log.Message($"IMissMyLimb: {hediffDef.defName} is not recognized as a prosthetic.");
        return false;
    }

    private static bool IsPartMissing(Pawn pawn, BodyPartRecord part)
    {
        if (pawn == null || part == null)
        {
            return false;
        }
        return pawn.health.hediffSet.PartIsMissing(part);
    }

    private static void RemoveNegativeThought(Pawn pawn, BodyPartRecord part)
    {
        if (part == null || pawn.needs?.mood?.thoughts?.memories == null)
        {
            return;
        }

        if (IsFingerOrToe(part))
        {
            pawn.needs.mood.thoughts.memories.RemoveMemoriesOfDef(ThoughtDef.Named("ColonistLostFingerToe"));
        }
        else if (part.def == BodyPartDefOf.Arm)
        {
            pawn.needs.mood.thoughts.memories.RemoveMemoriesOfDef(ThoughtDef.Named("ColonistLostArm"));
        }
        else if (part.def == BodyPartDefOf.Leg)
        {
            pawn.needs.mood.thoughts.memories.RemoveMemoriesOfDef(ThoughtDef.Named("ColonistLostLeg"));
        }
    }
}

public class ThoughtWorker_LostLimb : ThoughtWorker
{
    protected override ThoughtState CurrentStateInternal(Pawn p)
    {
        Log.Message($"IMissMyLimb: Checking thoughts for Pawn {p.Name.ToStringFull}");
        try
        {
            int lostFingerToeCount = p.health.hediffSet.GetMissingPartsCommonAncestors().Count(part => IsFingerOrToe(part.Part));
            int lostArmCount = p.health.hediffSet.GetMissingPartsCommonAncestors().Count(part => part.Part.def == BodyPartDefOf.Arm);
            int lostLegCount = p.health.hediffSet.GetMissingPartsCommonAncestors().Count(part => part.Part.def == BodyPartDefOf.Leg);

            if (lostFingerToeCount > 0)
            {
                Log.Message("IMissMyLimb: Pawn has lost a finger or toe.");
                return ThoughtState.ActiveAtStage(lostFingerToeCount > 1 ? 1 : 0, ThoughtDef.Named("ColonistLostFingerToe").defName);
            }
            if (lostArmCount > 0)
            {
                Log.Message("IMissMyLimb: Pawn has lost an arm.");
                return ThoughtState.ActiveAtStage(lostArmCount > 1 ? 1 : 0, ThoughtDef.Named("ColonistLostArm").defName);
            }
            if (lostLegCount > 0)
            {
                Log.Message("IMissMyLimb: Pawn has lost a leg.");
                return ThoughtState.ActiveAtStage(lostLegCount > 1 ? 1 : 0, ThoughtDef.Named("ColonistLostLeg").defName);
            }

            Log.Message("IMissMyLimb: No relevant injuries found.");
            return ThoughtState.Inactive;
        }
        catch (Exception ex)
        {
            Log.Error($"IMissMyLimb: Exception in ThoughtWorker_LostLimb: {ex}");
            return ThoughtState.Inactive;
        }
    }

    private static bool IsFingerOrToe(BodyPartRecord part)
    {
        if (part == null)
        {
            Log.Error("IMissMyLimb: BodyPartRecord is null.");
            return false;
        }
        bool result = part.def.defName.Contains("Finger") || part.def.defName.Contains("Toe");
        Log.Message($"IMissMyLimb: IsFingerOrToe check for {part.def.defName}: {result}");
        return result;
    }
}
