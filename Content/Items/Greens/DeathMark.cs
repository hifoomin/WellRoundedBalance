using Mono.Cecil.Cil;
using MonoMod.Cil;
using System;

namespace WellRoundedBalance.Items.Greens
{
    public class DeathMark : ItemBase<DeathMark>
    {
        public override string Name => ":: Items :: Greens :: Death Mark";
        public override ItemDef InternalPickup => RoR2Content.Items.DeathMark;

        public override string PickupText => "Enemies with " + minimumDebuffs + " or more debuffs are marked for death, taking bonus damage.";

        public override string DescText => "Enemies with <style=cIsDamage>" + minimumDebuffs + "</style> or more debuffs are <style=cIsDamage>marked for death</style>, increasing damage taken by <style=cIsDamage>" + d(baseDamageIncreasePerDebuff) + "</style> <style=cStack>(+" + d(damageIncreasePerDebuffPerStack) + " per stack)</style> per debuff from all sources for <style=cIsUtility>7</style> seconds.";

        [ConfigField("Minimum Debuffs", 2)]
        public static int minimumDebuffs;

        [ConfigField("Base Damage Increase Per Debuff", "Decimal.", 0.05f)]
        public static float baseDamageIncreasePerDebuff;

        [ConfigField("Damage Increase Per Debuff Per Stack", "Decimal.", 0.05f)]
        public static float damageIncreasePerDebuffPerStack;

        public override void Init()
        {
            base.Init();
        }

        public override void Hooks()
        {
            IL.RoR2.GlobalEventManager.ProcDeathMark += Changes;
            IL.RoR2.HealthComponent.TakeDamageProcess += Rework;
        }

        private void Rework(ILContext il)
        {
            ILCursor c = new(il);

            if (c.TryGotoNext(MoveType.After, x => x.MatchLdsfld(typeof(RoR2Content.Buffs), nameof(RoR2Content.Buffs.DeathMark)))) {
                if (c.TryGotoNext(MoveType.After, x => x.MatchLdcR4(1.5f))) {
                    c.Emit(OpCodes.Ldarg_0);
                    c.Emit(OpCodes.Ldarg_1);
                    c.EmitDelegate<Func<float, HealthComponent, DamageInfo, float>>((useless, hc, info) => {
                        if (!info.attacker) return 1.5f;

                        CharacterBody attacker = info.attacker.GetComponent<CharacterBody>();
                        if (hc.body && attacker.master && attacker.master.inventory)
                        {
                            int DeathMarkCount = Util.GetItemCountForTeam(attacker.master.teamIndex, RoR2Content.Items.DeathMark.itemIndex, false);
                            int debuffCount = 0;
                            foreach (BuffIndex buffType in BuffCatalog.debuffBuffIndices)
                            {
                                if (hc.body.HasBuff(buffType))
                                {
                                    debuffCount++;
                                }
                            }
                            DotController dotController = DotController.FindDotController(hc.gameObject);
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
            }
            else {
                Logger.LogError("Failed to apply Death Mark rework hook");
            }
        }

        private void Changes(ILContext il)
        {
            ILCursor c = new(il);
            if (c.TryGotoNext(MoveType.Before, x => x.MatchLdcI4(4)))
            {
                c.Next.Operand = minimumDebuffs;
            }
            else
            {
                Logger.LogError("Failed to apply Death Mark debuff count hook");
            }

            if (c.TryGotoNext(MoveType.Before, x => x.MatchLdcR4(7f)))
            {
                c.Remove();
                c.Emit(OpCodes.Ldc_R4, 1f);
            }
            else
            {
                Logger.LogError("Failed to apply Death Mark debuff duration hook");
            }
        }
    }
}