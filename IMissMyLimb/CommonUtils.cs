using HarmonyLib;
using RimWorld;
using System;
using System.Linq;
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
        bool isFingerOrToe = part.def.defName.Contains("Finger") || part.def.defName.Contains("Toe");
        //Log.Message($"IMissMyLimb: IsFingerOrToe - {part.Label}: {isFingerOrToe}"); // Log message to indicate if the part is a finger or toe
        return isFingerOrToe;
    }

    public static void AssignThought(Pawn pawn, ThoughtDef thoughtDef, BodyPartRecord part)
    {
        //Log.Message($"IMissMyLimb: Assigning thought '{thoughtDef?.defName}' for part '{part?.Label}' to pawn '{pawn?.Label}'."); // Log message to indicate the thought being assigned

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
            pawn.needs.mood.thoughts.memories.TryGainMemory(thought);
            //Log.Message($"IMissMyLimb: Thought '{thoughtDef.defName}' successfully assigned to pawn '{pawn.Label}'."); // Log message to indicate successful thought assignment
        }
        else
        {
            Log.Error($"IMissMyLimb: Failed to make Thought_Memory for '{thoughtDef.defName}'.");
        }
    }

    public static bool IsProsthetic(HediffDef hediffDef)
    {
        if (hediffDef == null)
        {
            Log.Error("IMissMyLimb: HediffDef is null.");
            return false;
        }

        bool isProsthetic = hediffDef.hediffClass == typeof(Hediff_Implant) || hediffDef.hediffClass == typeof(Hediff_AddedPart);
        isProsthetic = isProsthetic && (hediffDef.defName.Contains("Prosthetic") || hediffDef.defName.Contains("Bionic") || hediffDef.defName.Contains("SimpleProsthetic") || hediffDef.defName.Contains("Archotech"));

        //Log.Message($"IMissMyLimb: IsProsthetic - {hediffDef.defName}: {isProsthetic}"); // Log message to indicate if the hediff is a prosthetic
        return isProsthetic;
    }

    public static bool IsMissingBodyPart(Hediff hediff)
    {
        bool isMissingBodyPart = hediff is Hediff_MissingPart;
        //Log.Message($"IMissMyLimb: IsMissingBodyPart - {hediff.def.defName}: {isMissingBodyPart}"); // Log message to indicate if the hediff is a missing body part
        return isMissingBodyPart;
    }

    public static bool IsArchotech(HediffDef hediffDef)
    {
        bool isArchotech = hediffDef.defName.Contains("Archotech");
        //Log.Message($"IMissMyLimb: IsArchotech - {hediffDef.defName}: {isArchotech}"); // Log message to indicate if the hediff is an Archotech
        return isArchotech;
    }

    public static void RemoveNegativeThought(Pawn pawn, BodyPartRecord part)
    {
        //Log.Message($"IMissMyLimb: Removing negative thoughts for part '{part?.Label}' from pawn '{pawn?.Label}'."); // Log message to indicate the removal of negative thoughts

        if (part == null || pawn.needs?.mood?.thoughts?.memories == null)
        {
            return;
        }

        if (IsFingerOrToe(part))
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
                        //Log.Message($"IMissMyLimb: Applying ideology factor - Approved. Mood power factor: {thought.moodPowerFactor}");
                    }
                    else if (precept.def.defName == "BodyModification_Disapproved")
                    {
                        thought.moodPowerFactor -= 0.5f;
                        //Log.Message($"IMissMyLimb: Applying ideology factor - Disapproved. Mood power factor: {thought.moodPowerFactor}");
                    }
                }
            }
        }
    }

    public static void RemoveProstheticThought(Pawn pawn, BodyPartRecord part)
    {
        //Log.Message($"IMissMyLimb: Removing prosthetic thoughts for part '{part?.Label}' from pawn '{pawn?.Label}'.");

        if (part == null || pawn.needs?.mood?.thoughts?.memories == null)
        {
            return;
        }

        if (IsFingerOrToe(part))
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
