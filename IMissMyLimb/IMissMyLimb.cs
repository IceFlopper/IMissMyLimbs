using HarmonyLib;
using System;
using Verse;

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
