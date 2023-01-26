using Mono.Cecil.Cil;
using MonoMod.Cil;
using RoR2;
using System;

namespace WellRoundedBalance.Items.Greens
{
    public class DeathMark : ItemBase
    {
        public override string Name => ":: Items :: Greens :: Death Mark";
        public override string InternalPickupToken => "deathMark";

        public override string PickupText => "Enemies with 2 or more debuffs are marked for death, taking bonus damage.";

        public override string DescText => "Enemies with <style=cIsDamage>2</style> or more debuffs are <style=cIsDamage>marked for death</style>, increasing damage taken by <style=cIsDamage>7%</style> <style=cStack>(+2% per stack)</style> per debuff from all sources for <style=cIsUtility>7</style> <style=cStack>(+7 per stack)</style> seconds.";

        public override void Init()
        {
            base.Init();
        }

        public override void Hooks()
        {
            IL.RoR2.GlobalEventManager.OnHitEnemy += ChangeDebuffsReq;
            IL.RoR2.HealthComponent.TakeDamage += Changes;
        }

        public static void Changes(ILContext il)
        {
            ILCursor c = new(il);
            if (c.TryGotoNext(
                    x => x.MatchBrfalse(out _),
                    x => x.MatchLdloc(6),
                    x => x.MatchLdcR4(1.5f),
                    x => x.MatchMul(),
                    x => x.MatchStloc(6),
                    x => x.MatchLdarg(1),
                    x => x.MatchLdcI4(7),
                    x => x.MatchStfld<DamageInfo>("damageColorIndex")
                ))
            {
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
                        float damageBonus = debuffCount * 0.07f;
                        if (DeathMarkCount > 0)
                        {
                            return 1f + damageBonus + (0.02f * damageBonus * ((float)DeathMarkCount - 1f));
                        }
                        return 1f + damageBonus;
                    }
                    return 1.5f;
                });
            }
            else
            {
                Main.WRBLogger.LogError("Failed to apply Death Mark hook");
            }
        }

        public static void ChangeDebuffsReq(ILContext il)
        {
            ILCursor c = new(il);
            if (c.TryGotoNext(
                    x => x.MatchLdloc(16),
                    x => x.MatchLdcI4(4),
                    x => x.MatchBlt(out ILLabel IL_1180)))
            {
                c.Index += 2;
                c.Emit(OpCodes.Pop);
                c.Emit(OpCodes.Ldc_I4, 2);
            }
            else
            {
                Main.WRBLogger.LogError("Failed to apply Death Mark Minimum Debuffs hook");
            }
        }
    }
}