using HarmonyLib;
using RimWorld;
using System;
using System.Linq;
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

            Log.Message($"IMissMyLimb: Adding hediff '{hediff.def.defName}' to pawn '{pawn.Label}'.");

            if (CommonUtils.IsMissingBodyPart(hediff))
            {
                ThoughtDef thoughtDef = null;

                if (CommonUtils.IsFingerOrToe(hediff.Part))
                {
                    thoughtDef = ThoughtDef.Named("IMissMyLimb_ColonistLostFingerToe");
                }
                else if (hediff.Part.def == BodyPartDefOf.Arm)
                {
                    thoughtDef = ThoughtDef.Named("IMissMyLimb_ColonistLostArm");
                }
                else if (hediff.Part.def == BodyPartDefOf.Leg)
                {
                    thoughtDef = ThoughtDef.Named("IMissMyLimb_ColonistLostLeg");
                }

                if (thoughtDef != null)
                {
                    ThoughtUtils.AssignThought(pawn, thoughtDef, hediff.Part);
                }
            }
            else if (CommonUtils.IsProsthetic(hediff.def) && hediff.Part != null)
            {
                ThoughtUtils.RemoveProstheticThought(pawn, hediff.Part);

                ThoughtDef thoughtDef = null;

                int prostheticArmCount = pawn.health.hediffSet.hediffs.Count(h => h.Part?.def == BodyPartDefOf.Arm && CommonUtils.IsProsthetic(h.def));
                int prostheticLegCount = pawn.health.hediffSet.hediffs.Count(h => h.Part?.def == BodyPartDefOf.Leg && CommonUtils.IsProsthetic(h.def));

                if (CommonUtils.IsArchotech(hediff.def))
                {
                    if (CommonUtils.IsFingerOrToe(hediff.Part))
                    {
                        thoughtDef = ThoughtDef.Named("IMissMyLimb_ColonistGotArchotechFingerToe");
                    }
                    else if (hediff.Part.def == BodyPartDefOf.Arm)
                    {
                        thoughtDef = prostheticArmCount > 1 ? ThoughtDef.Named("IMissMyLimb_ColonistGotBothArchotechArms") : ThoughtDef.Named("IMissMyLimb_ColonistGotArchotechArm");
                    }
                    else if (hediff.Part.def == BodyPartDefOf.Leg)
                    {
                        thoughtDef = prostheticLegCount > 1 ? ThoughtDef.Named("IMissMyLimb_ColonistGotBothArchotechLegs") : ThoughtDef.Named("IMissMyLimb_ColonistGotArchotechLeg");
                    }
                }
                else if (hediff.def.defName.Contains("Bionic"))
                {
                    if (CommonUtils.IsFingerOrToe(hediff.Part))
                    {
                        thoughtDef = ThoughtDef.Named("IMissMyLimb_ColonistGotProstheticFingerToe");
                    }
                    else if (hediff.Part.def == BodyPartDefOf.Arm)
                    {
                        thoughtDef = prostheticArmCount > 1 ? ThoughtDef.Named("IMissMyLimb_ColonistGotBothBionicArms") : ThoughtDef.Named("IMissMyLimb_ColonistGotBionicArm");
                    }
                    else if (hediff.Part.def == BodyPartDefOf.Leg)
                    {
                        thoughtDef = prostheticLegCount > 1 ? ThoughtDef.Named("IMissMyLimb_ColonistGotBothBionicLegs") : ThoughtDef.Named("IMissMyLimb_ColonistGotBionicLeg");
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
                        thoughtDef = prostheticArmCount > 1 ? ThoughtDef.Named("IMissMyLimb_ColonistGotBothProstheticArms") : ThoughtDef.Named("IMissMyLimb_ColonistGotProstheticArm");
                    }
                    else if (hediff.Part.def == BodyPartDefOf.Leg)
                    {
                        thoughtDef = prostheticLegCount > 1 ? ThoughtDef.Named("IMissMyLimb_ColonistGotBothProstheticLegs") : ThoughtDef.Named("IMissMyLimb_ColonistGotProstheticLeg");
                    }
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
            Log.Error($"IMissMyLimb: Exception in AddHediff Postfix: {ex}");
        }
    }
}
