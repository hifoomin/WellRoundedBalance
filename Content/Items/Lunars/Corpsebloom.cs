using Mono.Cecil.Cil;
using MonoMod.Cil;
using System;
using static RoR2.HealthComponent;

namespace WellRoundedBalance.Items.Lunars
{
    public class Corpsebloom : ItemBase<Corpsebloom>
    {
        public override string Name => ":: Items ::::: Lunars :: Corpsebloom";
        public override ItemDef InternalPickup => RoR2Content.Items.RepeatHeal;

        public override string PickupText => "Increase your healing and apply it over time... <color=#FF7F7F>BUT its rate is limited.</color>";
        public override string DescText => "<style=cIsHealing>Heal " + d(healingIncrease) + "</style> <style=cStack>(+" + d(healingIncrease) + " per stack)</style> more. <style=cIsHealing>All healing is applied over time</style>. Can <style=cIsHealing>heal</style> for a <style=cIsHealing>maximum</style> of <style=cIsHealing>" + d(percentHealingCapPerSecond) + "</style> <style=cStack>(-" + whatTheFuck + "% per stack)</style> of your <style=cIsHealing>health per second</style>.";

        [ConfigField("Healing Increase", "Decimal.", 0.5f)]
        public static float healingIncrease;

        [ConfigField("Base Percent Healing Cap Per Second", "Decimal.", 0.07f)]
        public static float percentHealingCapPerSecond;

        [ConfigField("Percent Healing Cap Per Second Reduction Per Stack", "", 25f)]
        public static float whatTheFuck;

        public override void Init()
        {
            base.Init();
        }

        public override void Hooks()
        {
            IL.RoR2.HealthComponent.Heal += HealthComponent_Heal;
            IL.RoR2.HealthComponent.RepeatHealComponent.AddReserve += RepeatHealComponent_AddReserve;
            CharacterBody.onBodyInventoryChangedGlobal += CharacterBody_onBodyInventoryChangedGlobal;
        }

        private void CharacterBody_onBodyInventoryChangedGlobal(CharacterBody body)
        {
            var repeatHealComponent = body.GetComponent<RepeatHealComponent>();
            if (!repeatHealComponent) return;

            var inventory = body.inventory;
            if (!inventory) return;

            var stack = inventory.GetItemCount(RoR2Content.Items.RepeatHeal);
            var bruh = percentHealingCapPerSecond * whatTheFuck;
            repeatHealComponent.healthFractionToRestorePerSecond = Mathf.Max(0.001f, percentHealingCapPerSecond - ((stack - 1) * bruh / 100f));
        }

        private void HealthComponent_Heal(ILContext il)
        {
            ILCursor c = new(il);

            if (c.TryGotoNext(MoveType.Before,
                    x => x.MatchLdarg(1),
                    x => x.MatchLdcR4(2f),
                    x => x.MatchMul()))
            {
                c.Index += 1;
                c.Next.Operand = 1f + healingIncrease;
            }
            else
            {
                Logger.LogError("Failed to apply Corpsebloom Heal Increase hook");
            }

            c.Index = 0;

            if (c.TryGotoNext(MoveType.Before,
                x => x.MatchLdarg(0),
                x => x.MatchLdfld<HealthComponent>("repeatHealComponent"),
                x => x.MatchLdcR4(0.1f)))
            {
                for (int i = 0; i < 9; i++)
                {
                    c.Remove();
                }
                // remove health fraction to restore per second assignment
            }
        }

        private void RepeatHealComponent_AddReserve(ILContext il)
        {
            ILCursor c = new(il);

            if (c.TryGotoNext(MoveType.Before,
                x => x.MatchLdarg(1)))
            {
                c.Index++;
                c.Emit(OpCodes.Ldarg_0);
                c.EmitDelegate<Func<float, RepeatHealComponent, float>>((orig, self) =>
                {
                    var body = self.healthComponent.body;
                    if (body)
                    {
                        var inventory = body.inventory;
                        if (inventory)
                        {
                            var rejuvRacks = inventory.GetItemCount(RoR2Content.Items.IncreaseHealing);
                            if (rejuvRacks > 0)
                            {
                                // Main.WRBLogger.LogError(rejuvRacks + " racks, returned " + orig / rejuvRacks);
                                // Main.WRBLogger.LogError("no racks would've been " + orig);
                                return orig / rejuvRacks;
                            }
                        }
                    }
                    return orig;
                });
            }
        }
    }
}