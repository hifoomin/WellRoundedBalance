using MonoMod.Cil;
using RoR2;
using UnityEngine;

namespace UltimateCustomRun.Items.Whites
{
    public class Gasoline : ItemBase
    {
        public static float ExplosionDamage;
        public static int BurnDamage;
        public static float BurnDamageMultiplier;
        public static float Radius;
        public static float StackRadius;
        public static float BurnChance;
        public static bool StackBurnChance;
        public static float BurnChancePerStack;

        public override string Name => ":: Items : Whites :: Gasoline";
        public override string InternalPickupToken => "igniteOnKill";
        public override bool NewPickup => true;

        public override string PickupText => (BurnChance > 0 ? "Gain " + BurnChance + "% chance to ignite enemies on hit." : "") +
                                             "Killing an enemy ignites other nearby enemies.";

        public override string DescText => (BurnChance > 0 ? "Gain " + BurnChance + "%" + (StackBurnChance ? " <style=cStack>(+" + BurnChancePerStack + "% per stack)</style>" : "") + " chance to <style=cIsDamage>ignite</style> enemies on hit." : "") +
                                           "Killing an enemy <style=cIsDamage>ignites</style> all enemies within <style=cIsDamage>" + Radius + "m</style> <style=cStack>(+" + StackRadius + "m per stack)</style> for <style=cIsDamage>" + d(ExplosionDamage) + "</style> base damage. Additionally, enemies <style=cIsDamage>burn</style> for <style=cIsDamage>" + d(BurnDamage * BurnDamageMultiplier) + "</style> <style=cStack>(+" + d(BurnDamage * BurnDamageMultiplier / 2f) + " per stack)</style> base damage.";

        public override void Init()
        {
            ExplosionDamage = ConfigOption(1.5f, "Explosion Damage", "Decimal. Vanilla is 1.5");
            ROSOption("Whites", 0f, 5f, 0.01f, "1");
            BurnDamage = ConfigOption(2, "Burn Damage", "Per Stack. Vanilla is 2");
            ROSOption("Whites", 0f, 4f, 0.1f, "1");
            BurnDamageMultiplier = ConfigOption(0.75f, "Burn Damage Multiplier", "Decimal. Vanilla is 0.75");
            ROSOption("Whites", 0f, 2f, 0.05f, "1");
            Radius = ConfigOption(12f, "Base Range", "Vanilla is 12");
            ROSOption("Whites", 0f, 30f, 0.5f, "1");
            StackRadius = ConfigOption(4f, "Stack Range", "Per Stack. Vanilla is 4");
            ROSOption("Whites", 0f, 30f, 0.5f, "1");
            BurnChance = ConfigOption(0f, "Base Burn Chance", "Vanilla is 0");
            ROSOption("Whites", 0f, 100f, 1f, "1");
            BurnChancePerStack = ConfigOption(0f, "Stack Burn Chance", "Per Stack. Vanilla is 0");
            ROSOption("Whites", 0f, 100f, 1f, "1");
            StackBurnChance = ConfigOption(false, "Make Burn Chance Stack?", "Vanilla is false");
            ROSOption("Whites", 0f, 5f, 0.01f, "1");
            base.Init();
        }

        public override void Hooks()
        {
            IL.RoR2.GlobalEventManager.ProcIgniteOnKill += ChangeBurnDamage;
            IL.RoR2.GlobalEventManager.ProcIgniteOnKill += ChangeExplosionDamage;
            IL.RoR2.GlobalEventManager.ProcIgniteOnKill += ChangeRadius;
            On.RoR2.GlobalEventManager.OnHitEnemy += AddBurn;
        }

        public void AddBurn(On.RoR2.GlobalEventManager.orig_OnHitEnemy orig, GlobalEventManager self, DamageInfo damageInfo, GameObject victim)
        {
            if (damageInfo.attacker && damageInfo.attacker.GetComponent<CharacterBody>().inventory)
            {
                var inv = damageInfo.attacker.GetComponent<CharacterBody>().inventory;
                var stack = inv.GetItemCount(RoR2Content.Items.IgniteOnKill);
                if (stack > 0)
                {
                    switch (StackBurnChance)
                    {
                        default:
                            if (Util.CheckRoll(BurnChance + BurnChance * (stack - 1), damageInfo.attacker.GetComponent<CharacterBody>().master.luck))
                            {
                                InflictDotInfo galsone = new()
                                {
                                    attackerObject = damageInfo.attacker,
                                    victimObject = victim,
                                    dotIndex = DotController.DotIndex.Burn,
                                    damageMultiplier = 1f
                                };
                                DotController.InflictDot(ref galsone);
                            }
                            break;

                        case false:
                            if (Util.CheckRoll(BurnChance, damageInfo.attacker.GetComponent<CharacterBody>().master.luck))
                            {
                                InflictDotInfo galsone = new()
                                {
                                    attackerObject = damageInfo.attacker,
                                    victimObject = victim,
                                    dotIndex = DotController.DotIndex.Burn,
                                    damageMultiplier = 1f
                                };
                                DotController.InflictDot(ref galsone);
                            }
                            break;
                    }
                }
            }
            orig(self, damageInfo, victim);
        }

        public static void ChangeExplosionDamage(ILContext il)
        {
            ILCursor c = new(il);

            c.GotoNext(MoveType.Before,
                x => x.MatchAdd(),
                x => x.MatchStloc(1),
                x => x.MatchLdcR4(1.5f)
            );
            c.Index += 2;
            c.Next.Operand = ExplosionDamage;
        }

        public static void ChangeBurnDamage(ILContext il)
        {
            ILCursor c = new(il);

            c.GotoNext(MoveType.Before,
                x => x.MatchLdcI4(1),
                x => x.MatchLdarg(1),
                x => x.MatchAdd(),
                x => x.MatchConvR4(),
                x => x.MatchLdcR4(0.75f)
            );
            c.Next.Operand = BurnDamage;
            c.Index += 4;
            c.Next.Operand = BurnDamageMultiplier;
        }

        public static void ChangeRadius(ILContext il)
        {
            ILCursor c = new(il);

            c.GotoNext(MoveType.Before,
                x => x.MatchLdcR4(8f),
                x => x.MatchLdcR4(4f)
            );
            c.Next.Operand = Radius - StackRadius;
            c.Index += 1;
            c.Next.Operand = StackRadius;
        }
    }
}