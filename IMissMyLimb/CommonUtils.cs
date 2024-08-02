using RimWorld;
using Verse;
using System.Linq;

public static class CommonUtils
{
    public static bool IsFingerOrToe(BodyPartRecord part)
    {
        if (part == null)
        {
            Log.Error("IMissMyLimb: BodyPartRecord is null.");
            return false;
        }
        return part.def.defName.Contains("Finger") || part.def.defName.Contains("Toe");
    }

    public static void AssignThought(Pawn pawn, ThoughtDef thoughtDef, BodyPartRecord part, int stageIndex = 0)
    {
        if (thoughtDef == null)
        {
            Log.Error("IMissMyLimb: ThoughtDef is null.");
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
                    return true;
                }

                if (comp is HediffCompProperties_SeverityPerDay || comp is HediffCompProperties_Immunizable || comp is HediffCompProperties_TendDuration)
                {
                    return true;
                }
            }
        }

        if (hediffDef.defName.Contains("Prosthetic") || hediffDef.defName.Contains("Bionic") || hediffDef.defName.Contains("SimpleProsthetic"))
        {
            return true;
        }

        return false;
    }

    public static bool IsArchotech(HediffDef hediffDef)
    {
        return hediffDef.defName.Contains("Archotech");
    }

    public static void RemoveNegativeThought(Pawn pawn, BodyPartRecord part)
    {
        if (part == null || pawn.needs?.mood?.thoughts?.memories == null)
        {
            return;
        }

        if (IsFingerOrToe(part))
        {
            pawn.needs.mood.thoughts.memories.RemoveMemoriesOfDef(ThoughtDef.Named("IMissMyLimb_ColonistLostFingerToe"));
        }
        else if (part.def == BodyPartDefOf.Arm)
        {
            pawn.needs.mood.thoughts.memories.RemoveMemoriesOfDef(ThoughtDef.Named("IMissMyLimb_ColonistLostArm"));
        }
        else if (part.def == BodyPartDefOf.Leg)
        {
            pawn.needs.mood.thoughts.memories.RemoveMemoriesOfDef(ThoughtDef.Named("IMissMyLimb_ColonistLostLeg"));
        }
    }

    public static void ApplyIdeologyFactor(Pawn pawn, Thought_Memory thought)
    {
        if (ModsConfig.IdeologyActive)
        {
            var precepts = pawn.Ideo?.PreceptsListForReading;
            if (precepts != null)
            {
                foreach (var precept in precepts)
                {
                    if (precept.def.defName == "BodyModification_Approved")
                    {
                        thought.moodPowerFactor += 0.5f; // Adjust this value as needed
                    }
                    else if (precept.def.defName == "BodyModification_Disapproved")
                    {
                        thought.moodPowerFactor -= 0.5f; // Adjust this value as needed
                    }
                }
            }
        }
    }
}
