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
        return part.def.defName.Contains("Finger") || part.def.defName.Contains("Toe");
    }

    public static bool IsProsthetic(HediffDef hediffDef)
    {
        if (hediffDef == null)
        {
            Log.Error("IMissMyLimb: HediffDef is null.");
            return false;
        }

        bool isProsthetic = hediffDef.hediffClass == typeof(Hediff_Implant) || hediffDef.hediffClass == typeof(Hediff_AddedPart);
        return isProsthetic && (hediffDef.defName.Contains("Prosthetic") || hediffDef.defName.Contains("Bionic") || hediffDef.defName.Contains("SimpleProsthetic") || hediffDef.defName.Contains("Archotech"));
    }

    public static bool IsMissingBodyPart(Hediff hediff)
    {
        return hediff is Hediff_MissingPart;
    }

    public static bool IsArchotech(HediffDef hediffDef)
    {
        return hediffDef.defName.Contains("Archotech");
    }
}
