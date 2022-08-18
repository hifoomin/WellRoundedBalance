using Mono.Cecil.Cil;
using MonoMod.Cil;
using R2API;
using RoR2;
using System;

namespace UltimateCustomRun.Items.Greens
{
    public class OldWarStealthkit : ItemBase
    {
        public static float BuffArmor;
        public static bool StackBuffArmor;
        public static float Armor;
        public static bool StackArmor;
        public static float BuffDuration;
        public static float RechargeTime;
        public static float StackRechargeTime;
        public static float Threshold;

        public override string Name => ":: Items :: Greens :: Old War Stealthkit";
        public override string InternalPickupToken => "phasing";
        public override bool NewPickup => false;
        public override string PickupText => "";

        public override string DescText => (Armor != 0f ? "<style=cIsHealing>Increase armor</style> by <style=cIsHealing>" + Armor + "</style> <style=cStack>(+" + Armor + " per stack)</style>. " : "") +
                                           "Falling below <style=cIsHealth>" + d(Threshold) + " health</style> causes you to gain <style=cIsUtility>40% movement speed</style>" +
                                           (BuffArmor != 0f ? ", <style=cIsHealing>" + BuffArmor + " armor</style>" +
                                           (StackBuffArmor ? " <style=cStack>(+" + BuffArmor + " per stack)</style>" : "") : "") +
                                           " and <style=cIsUtility>invisibility</style> for <style=cIsUtility>" + BuffDuration + "s</style>. Recharges every <style=cIsUtility>" + RechargeTime + " seconds</style> <style=cStack>(-" + d(RechargeTime) + " per stack)</style>.";

        public override void Init()
        {
            BuffArmor = ConfigOption(0f, "Buff Armor", "With Buff. Vanilla is 0");
            StackBuffArmor = ConfigOption(false, "Stack Buff Armor?", "Vanilla is false");
            Armor = ConfigOption(0f, "Armor", "Vanilla is 0");
            StackArmor = ConfigOption(false, "Stack Armor?", "Vanilla is false");
            BuffDuration = ConfigOption(5f, "Buff Duration", "Vanilla is 5");
            RechargeTime = ConfigOption(30f, "Recharge Time", "Vanilla is 30");
            StackRechargeTime = ConfigOption(0.5f, "Stack Recharge Time Reduction", "Decimal. Vanilla is 0.5");
            Threshold = ConfigOption(0.25f, "Health Threshold", "Decimal. Vanilla is 0.25");
            base.Init();
        }

        public override void Hooks()
        {
            RecalculateStatsAPI.GetStatCoefficients += AddBehavior;
            On.RoR2.Items.PhasingBodyBehavior.Start += Changes;
            IL.RoR2.Items.PhasingBodyBehavior.FixedUpdate += ChangeThreshold;
        }

        private void ChangeThreshold(ILContext il)
        {
            ILCursor c = new(il);

            if (c.TryGotoNext(MoveType.Before,
                x => x.MatchCallOrCallvirt<HealthComponent>("get_isHealthLow")
            ))
            {
                c.Index += 1;
                c.Emit(OpCodes.Ldarg_0);
                c.EmitDelegate<Func<bool, HealthComponent, bool>>((Check, self) =>
                {
                    if ((self.health + self.shield) / self.fullCombinedHealth < Threshold)
                    {
                        Check = true;
                        return Check;
                    }
                    else
                    {
                        Check = false;
                        return Check;
                    }
                });
            }
            else
            {
                Main.UCRLogger.LogError("Failed to apply Old War Stealthkit Threshold hook");
            }

            // this NREs
        }

        private void Changes(On.RoR2.Items.PhasingBodyBehavior.orig_Start orig, RoR2.Items.PhasingBodyBehavior self)
        {
            self.baseRechargeSeconds = RechargeTime;
            self.rechargeReductionMultiplierPerStack = StackRechargeTime;
            self.buffDuration = BuffDuration;
            orig(self);
        }

        public static void AddBehavior(CharacterBody sender, RecalculateStatsAPI.StatHookEventArgs args)
        {
            if (sender.inventory)
            {
                var stack = sender.inventory.GetItemCount(RoR2Content.Items.Phasing);
                var buffp1 = sender.HasBuff(RoR2.RoR2Content.Buffs.Cloak);
                var buffp2 = sender.HasBuff(RoR2.RoR2Content.Buffs.CloakSpeed);
                // periphery ZASED
                if (stack > 0 && buffp1 && buffp2)
                {
                    args.armorAdd += StackBuffArmor ? BuffArmor * stack : BuffArmor;
                }
                if (stack > 0)
                {
                    args.armorAdd += StackArmor ? Armor * stack : Armor;
                }
            }
        }
    }
}