using System;
using Mono.Cecil.Cil;
using MonoMod.Cil;
using RoR2;

namespace UltimateCustomRun
{
    public static class DeathMark
    {
        // ////////////
        // 
        // Thanks to Skell
        //
        // ///////////////
        public static void Changes(ILContext il)
        {
            ILCursor c = new ILCursor(il);
            c.GotoNext(
                x => x.MatchBrfalse(out _),
                x => x.MatchLdloc(6),
                x => x.MatchLdcR4(1.5f),
                x => x.MatchMul(),
                x => x.MatchStloc(6),
                x => x.MatchLdarg(1),
                x => x.MatchLdcI4(7),
                x => x.MatchStfld<DamageInfo>("damageColorIndex")
                );
            c.Index += 3;
            c.Emit(OpCodes.Pop);
            c.Emit(OpCodes.Ldarg_0);
            c.Emit(OpCodes.Ldloc_1);
            c.EmitDelegate<Func<HealthComponent, CharacterBody, float>>((self, attacker) =>
            {
                if (attacker.master.inventory)
                {
                    int DeathMarkCount = Util.GetItemCountForTeam(attacker.master.teamIndex, RoR2Content.Items.DeathMark.itemIndex, false, true);
                    int debuffCount = 0;
                    foreach (BuffIndex buffType in BuffCatalog.debuffBuffIndices)
                    {
                        if (self.body.HasBuff(buffType))
                        {
                            debuffCount++;
                        }
                    }
                    DotController dotController = DotController.FindDotController(self.gameObject);
                    if (dotController)
                    {
                        for (DotController.DotIndex dotIndex = DotController.DotIndex.Bleed; dotIndex < DotController.DotIndex.Count; dotIndex++)
                        {
                            if (dotController.HasDotActive(dotIndex))
                            {
                                debuffCount++;
                            }
                        }
                    }
                    float damageBonus = debuffCount * Main.DeathMarkDmgIncreasePerDebuff.Value;
                    if (DeathMarkCount > 0)
                    {
                        return 1f + damageBonus + (Main.DeathMarkStackBonus.Value * damageBonus * ((float)DeathMarkCount - 1f));
                    }
                    return 1f + damageBonus;
                }
                return 1.5f;
            });
        }

        public static void ChangeDebuffsReq(ILContext il)
        {
            ILCursor c = new ILCursor(il);
            c.GotoNext(
                x => x.MatchLdloc(16),
                x => x.MatchLdcI4(4),
                x => x.MatchBlt(out ILLabel IL_0BD7)
                );
            c.Index += 2;
            c.Emit(OpCodes.Pop);
            c.Emit(OpCodes.Ldc_I4, Main.DeathMarkMinimumDebuffsRequired.Value);
        }
    }
}
