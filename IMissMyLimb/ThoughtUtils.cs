using RimWorld;
using Verse;

public static class ThoughtUtils
{
    public static void AssignThought(Pawn pawn, ThoughtDef thoughtDef, BodyPartRecord part)
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

        var thought = ThoughtMaker.MakeThought(thoughtDef) as Thought_Memory;
        if (thought != null)
        {
            ApplyTraitModifiers(pawn, thought);
            pawn.needs.mood.thoughts.memories.TryGainMemory(thought);
        }
        else
        {
            Log.Error($"IMissMyLimb: Failed to make Thought_Memory for '{thoughtDef.defName}'.");
        }
    }

    public static void RemoveNegativeThought(Pawn pawn, BodyPartRecord part)
    {
        if (part == null || pawn.needs?.mood?.thoughts?.memories == null)
        {
            return;
        }

        if (CommonUtils.IsFingerOrToe(part))
        {
            pawn.needs.mood.thoughts.memories.RemoveMemoriesOfDef(ThoughtDef.Named("IMissMyLimb_ColonistLostFingerToe"));
            pawn.needs.mood.thoughts.memories.RemoveMemoriesOfDef(ThoughtDef.Named("IMissMyLimb_ColonistLostMultipleFingersToes"));
            pawn.needs.mood.thoughts.memories.RemoveMemoriesOfDef(ThoughtDef.Named("IMissMyLimb_ColonistGotProstheticFingerToe"));
            pawn.needs.mood.thoughts.memories.RemoveMemoriesOfDef(ThoughtDef.Named("IMissMyLimb_ColonistGotMultipleProstheticFingersToes"));
            pawn.needs.mood.thoughts.memories.RemoveMemoriesOfDef(ThoughtDef.Named("IMissMyLimb_ColonistGotArchotechFingerToe"));
        }
        else if (part.def == BodyPartDefOf.Arm)
        {
            pawn.needs.mood.thoughts.memories.RemoveMemoriesOfDef(ThoughtDef.Named("IMissMyLimb_ColonistLostArm"));
            pawn.needs.mood.thoughts.memories.RemoveMemoriesOfDef(ThoughtDef.Named("IMissMyLimb_ColonistLostBothArms"));
            pawn.needs.mood.thoughts.memories.RemoveMemoriesOfDef(ThoughtDef.Named("IMissMyLimb_ColonistGotProstheticArm"));
            pawn.needs.mood.thoughts.memories.RemoveMemoriesOfDef(ThoughtDef.Named("IMissMyLimb_ColonistGotBothProstheticArms"));
            pawn.needs.mood.thoughts.memories.RemoveMemoriesOfDef(ThoughtDef.Named("IMissMyLimb_ColonistGotBionicArm"));
            pawn.needs.mood.thoughts.memories.RemoveMemoriesOfDef(ThoughtDef.Named("IMissMyLimb_ColonistGotBothBionicArms"));
            pawn.needs.mood.thoughts.memories.RemoveMemoriesOfDef(ThoughtDef.Named("IMissMyLimb_ColonistGotArchotechArm"));
            pawn.needs.mood.thoughts.memories.RemoveMemoriesOfDef(ThoughtDef.Named("IMissMyLimb_ColonistGotBothArchotechArms"));
        }
        else if (part.def == BodyPartDefOf.Leg)
        {
            pawn.needs.mood.thoughts.memories.RemoveMemoriesOfDef(ThoughtDef.Named("IMissMyLimb_ColonistLostLeg"));
            pawn.needs.mood.thoughts.memories.RemoveMemoriesOfDef(ThoughtDef.Named("IMissMyLimb_ColonistLostBothLegs"));
            pawn.needs.mood.thoughts.memories.RemoveMemoriesOfDef(ThoughtDef.Named("IMissMyLimb_ColonistGotProstheticLeg"));
            pawn.needs.mood.thoughts.memories.RemoveMemoriesOfDef(ThoughtDef.Named("IMissMyLimb_ColonistGotBothProstheticLegs"));
            pawn.needs.mood.thoughts.memories.RemoveMemoriesOfDef(ThoughtDef.Named("IMissMyLimb_ColonistGotBionicLeg"));
            pawn.needs.mood.thoughts.memories.RemoveMemoriesOfDef(ThoughtDef.Named("IMissMyLimb_ColonistGotBothBionicLegs"));
            pawn.needs.mood.thoughts.memories.RemoveMemoriesOfDef(ThoughtDef.Named("IMissMyLimb_ColonistGotArchotechLeg"));
            pawn.needs.mood.thoughts.memories.RemoveMemoriesOfDef(ThoughtDef.Named("IMissMyLimb_ColonistGotBothArchotechLegs"));
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
                        thought.moodPowerFactor += 0.5f;
                    }
                    else if (precept.def.defName == "BodyModification_Disapproved")
                    {
                        thought.moodPowerFactor -= 0.5f;
                    }
                }
            }
        }
    }

    public static void ApplyTraitModifiers(Pawn pawn, Thought_Memory thought)
    {
        if (pawn.story?.traits == null)
        {
            return;
        }

        if (pawn.story.traits.HasTrait(TraitDef.Named("BodyPurist")))
        {
            thought.moodPowerFactor -= 1.0f;  // Applying a stronger negative effect for Body Purist
        }
        else if (pawn.story.traits.HasTrait(TraitDef.Named("BodyModder")))
        {
            thought.moodPowerFactor += 0.5f;
        }
    }

    public static void RemoveProstheticThought(Pawn pawn, BodyPartRecord part)
    {
        if (part == null || pawn.needs?.mood?.thoughts?.memories == null)
        {
            return;
        }

        if (CommonUtils.IsFingerOrToe(part))
        {
            pawn.needs.mood.thoughts.memories.RemoveMemoriesOfDef(ThoughtDef.Named("IMissMyLimb_ColonistGotProstheticFingerToe"));
            pawn.needs.mood.thoughts.memories.RemoveMemoriesOfDef(ThoughtDef.Named("IMissMyLimb_ColonistGotMultipleProstheticFingersToes"));
            pawn.needs.mood.thoughts.memories.RemoveMemoriesOfDef(ThoughtDef.Named("IMissMyLimb_ColonistGotArchotechFingerToe"));
        }
        else if (part.def == BodyPartDefOf.Arm)
        {
            pawn.needs.mood.thoughts.memories.RemoveMemoriesOfDef(ThoughtDef.Named("IMissMyLimb_ColonistGotProstheticArm"));
            pawn.needs.mood.thoughts.memories.RemoveMemoriesOfDef(ThoughtDef.Named("IMissMyLimb_ColonistGotBothProstheticArms"));
            pawn.needs.mood.thoughts.memories.RemoveMemoriesOfDef(ThoughtDef.Named("IMissMyLimb_ColonistGotBionicArm"));
            pawn.needs.mood.thoughts.memories.RemoveMemoriesOfDef(ThoughtDef.Named("IMissMyLimb_ColonistGotBothBionicArms"));
            pawn.needs.mood.thoughts.memories.RemoveMemoriesOfDef(ThoughtDef.Named("IMissMyLimb_ColonistGotArchotechArm"));
            pawn.needs.mood.thoughts.memories.RemoveMemoriesOfDef(ThoughtDef.Named("IMissMyLimb_ColonistGotBothArchotechArms"));
        }
        else if (part.def == BodyPartDefOf.Leg)
        {
            pawn.needs.mood.thoughts.memories.RemoveMemoriesOfDef(ThoughtDef.Named("IMissMyLimb_ColonistGotProstheticLeg"));
            pawn.needs.mood.thoughts.memories.RemoveMemoriesOfDef(ThoughtDef.Named("IMissMyLimb_ColonistGotBothProstheticLegs"));
            pawn.needs.mood.thoughts.memories.RemoveMemoriesOfDef(ThoughtDef.Named("IMissMyLimb_ColonistGotBionicLeg"));
            pawn.needs.mood.thoughts.memories.RemoveMemoriesOfDef(ThoughtDef.Named("IMissMyLimb_ColonistGotBothBionicLegs"));
            pawn.needs.mood.thoughts.memories.RemoveMemoriesOfDef(ThoughtDef.Named("IMissMyLimb_ColonistGotArchotechLeg"));
            pawn.needs.mood.thoughts.memories.RemoveMemoriesOfDef(ThoughtDef.Named("IMissMyLimb_ColonistGotBothArchotechLegs"));
        }
    }
}
