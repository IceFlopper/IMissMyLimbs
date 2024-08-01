using RimWorld;
using Verse;

public static class CommonUtils
{
    public static bool IsFingerOrToe(BodyPartRecord part)
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

    public static void AssignThought(Pawn pawn, ThoughtDef thoughtDef, BodyPartRecord part)
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

        if (pawn.needs == null || pawn.needs.mood == null || pawn.needs.mood.thoughts == null || pawn.needs.mood.thoughts.memories == null)
        {
            Log.Error("IMissMyLimb: Pawn's needs, mood, thoughts, or memories are null.");
            return;
        }

        int stageIndex = 0;
        if (pawn.health.hediffSet.GetMissingPartsCommonAncestors().Count(x => x.Part?.def == part.def) > 1)
        {
            stageIndex = 1; // Use second stage for multiple lost parts of the same type
        }

        Log.Message($"IMissMyLimb: Assigning thought {thoughtDef.defName} at stage {stageIndex} to pawn {pawn.Name.ToStringFull} for part {part.def.defName}");

        var thought = ThoughtMaker.MakeThought(thoughtDef, stageIndex);
        pawn.needs.mood.thoughts.memories.TryGainMemory(thought);
    }

    public static bool IsProsthetic(HediffDef hediffDef)
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

    public static void RemoveNegativeThought(Pawn pawn, BodyPartRecord part)
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
