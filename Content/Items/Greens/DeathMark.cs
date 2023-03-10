using Mono.Cecil.Cil;
using MonoMod.Cil;
using System;

namespace WellRoundedBalance.Items.Greens
{
    public class DeathMark : ItemBase
    {
        public override string Name => ":: Items :: Greens :: Death Mark";
        public override ItemDef InternalPickup => RoR2Content.Items.DeathMark;

        public override string PickupText => "Enemies with " + minimumDebuffs + " or more debuffs are marked for death, taking bonus damage.";

        public override string DescText => "Enemies with <style=cIsDamage>" + minimumDebuffs + "</style> or more debuffs are <style=cIsDamage>marked for death</style>, increasing damage taken by <style=cIsDamage>" + d(baseDamageIncreasePerDebuff) + "</style> <style=cStack>(+" + d(damageIncreasePerDebuffPerStack) + " per stack)</style> per debuff from all sources for <style=cIsUtility>7</style> seconds.";

        [ConfigField("Minimum Debuffs", 2)]
        public static int minimumDebuffs;

        [ConfigField("Base Damage Increase Per Debuff", "Decimal.", 0.07f)]
        public static float baseDamageIncreasePerDebuff;

        [ConfigField("Damage Increase Per Debuff Per Stack", "Decimal.", 0.03f)]
        public static float damageIncreasePerDebuffPerStack;

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
                    if (self.body && attacker.master && attacker.master.inventory)
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
                        float damageBonus = debuffCount * baseDamageIncreasePerDebuff;
                        if (DeathMarkCount > 0)
                        {
                            return 1f + damageBonus + (damageIncreasePerDebuffPerStack * damageBonus * ((float)DeathMarkCount - 1f));
                        }
                        return 1f + damageBonus;
                    }
                    return 1.5f;
                });
            }
            else
            {
                Logger.LogError("Failed to apply Death Mark Rework hook");
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
                c.Emit(OpCodes.Ldc_I4, minimumDebuffs);
            }
            else
            {
                Logger.LogError("Failed to apply Death Mark Minimum Debuffs hook");
            }

            c.Index = 0;

            if (c.TryGotoNext(MoveType.Before,
                x => x.MatchLdcR4(7f),
                x => x.MatchLdloc(out _),
                x => x.MatchConvR4()))
            {
                c.Index += 2;
                c.EmitDelegate<Func<int, int>>((useless) =>
                {
                    return 1;
                });
            }
            else
            {
                Logger.LogError("Failed to apply Death Mark Debuff Length hook");
            }
        }
    }
}