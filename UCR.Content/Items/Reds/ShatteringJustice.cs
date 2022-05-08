using MonoMod.Cil;
using R2API;
using RoR2;
using UnityEngine;

namespace UltimateCustomRun.Items.Reds
{
    public class ShatteringJustice : ItemBase
    {
        public static int Hits;
        public static float ArmorReduction;
        public static float PercentHealthReduction;
        public static float Duration;
        public static BuffDef Counter;
        public override string Name => ":: Items ::: Reds :: Shattering Justice";
        public override string InternalPickupToken => "armorReductionOnHit";
        public override bool NewPickup => true;

        public override string PickupText => "Reduce the" +
                                             (ArmorReduction > 0 && PercentHealthReduction > 0 ? " armor and health" : (ArmorReduction > 0 ? " armor" : (PercentHealthReduction > 0 ? " health" : ""))) +
                                             " of enemies after repeatedly striking them.";

        public override string DescText => "After hitting an enemy <style=cIsDamage>" + Hits + "</style> times, reduce their" +
                                           (ArmorReduction > 0 && PercentHealthReduction > 0 ? " <style=cIsDamage>armor</style> by <style=cIsDamage>" + ArmorReduction + "</style> once <style=cIsDamage> and <style=cIsDamage>health</style> by <style=cIsDamage>" + d(PercentHealthReduction) + "</style> every hit" :
                                           (ArmorReduction > 0 ? " <style=cIsDamage>armor</style> by <style=cIsDamage>" + ArmorReduction + "</style> " :
                                           (PercentHealthReduction > 0 ? " <style=cIsDamage>health</style> by <style=cIsDamage>" + d(PercentHealthReduction) + "</style> each hit" : ""))) +
                                           " for <style=cIsDamage>" + Duration + "</style><style=cStack> (+" + Duration + " per stack)</style> seconds.";

        public override void Init()
        {
            Hits = ConfigOption(5, "Hits", "Vanilla is 5");
            ArmorReduction = ConfigOption(60f, "Armor Reduction", "Vanilla is 60");
            PercentHealthReduction = ConfigOption(0f, "Percent Health Reduction", "Decimal. Vanilla is 0");
            Duration = ConfigOption(8f, "Duration", "Per Stack. Vanilla is 8");
            base.Init();
        }

        public override void Hooks()
        {
            IL.RoR2.HealthComponent.TakeDamage += ChangeHitCount;
            IL.RoR2.HealthComponent.TakeDamage += ChangeDuration;
            IL.RoR2.CharacterBody.RecalculateStats += ChangeArmorReduction;
            AddCounter();
            GlobalEventManager.onServerDamageDealt += AddBehavior;
            On.RoR2.CharacterBody.RecalculateStats += LowerMaxHp;
        }

        public static void AddBehavior(DamageReport report)
        {
            var AB = report.attackerBody;
            var VB = report.victimBody;
            if (report != null && AB != null)
            {
                if (AB.inventory)
                {
                    var Stack = AB.inventory.GetItemCount(RoR2Content.Items.ArmorReductionOnHit);
                    if (Stack > 0 && VB.HasBuff(RoR2Content.Buffs.Pulverized))
                    {
                        VB.AddTimedBuffAuthority(Counter.buffIndex, Duration);
                    }
                }
            }
        }

        public static void LowerMaxHp(On.RoR2.CharacterBody.orig_RecalculateStats orig, CharacterBody self)
        {
            var count = self.GetBuffCount(Counter);
            if (count > 0)
            {
                Main.UCRLogger.LogInfo("max hp before reduction is " + self.maxHealth);
                self.maxHealth -= self.maxHealth * PercentHealthReduction * count;
                self.healthComponent.health -= self.healthComponent.health * PercentHealthReduction * count;
                Main.UCRLogger.LogInfo("max hp after reduction is " + self.maxHealth);
            }
            orig(self);
        }

        public static void AddCounter()
        {
            Counter = ScriptableObject.CreateInstance<BuffDef>();

            Counter.buffColor = Color.black;
            Counter.canStack = true;
            Counter.isDebuff = false;
            Counter.isHidden = true;
            Counter.name = "ShatteringJusticeCounter";
            Counter.iconSprite = LegacyResourcesAPI.Load<Sprite>("textures/bufficons/texBuffEntangleIcon");
            R2API.ContentAddition.AddBuffDef(Counter);
        }

        public static void ChangeArmorReduction(ILContext il)
        {
            ILCursor c = new ILCursor(il);

            c.GotoNext(MoveType.Before,
                x => x.MatchLdcR4(60f)
            );
            c.Next.Operand = ArmorReduction;
        }

        public static void ChangeHitCount(ILContext il)
        {
            ILCursor c = new(il);

            c.GotoNext(MoveType.Before,
                x => x.MatchLdsfld("RoR2.RoR2Content/Buffs", "PulverizeBuildup"),
                x => x.MatchCallOrCallvirt<CharacterBody>("GetBuffCount"),
                x => x.MatchLdcI4(5)
            );
            c.Index += 2;
            c.Next.Operand = Hits;
        }

        public static void ChangeDuration(ILContext il)
        {
            ILCursor c = new(il);

            c.GotoNext(MoveType.Before,
                x => x.MatchLdsfld("RoR2.RoR2Content/Buffs", "Pulverized"),
                x => x.MatchLdcR4(8f)
            );
            c.Index += 1;
            c.Next.Operand = Duration;
        }
    }
}