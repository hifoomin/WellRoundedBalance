using Mono.Cecil.Cil;
using MonoMod.Cil;
using R2API;
using R2API.Utils;
using RoR2;
using System;

namespace UltimateCustomRun.Items.Greens
{
    public class RoseBuckler : ItemBase
    {
        public static int ConditionalArmor;
        public static float Armor;
        public static bool StackArmor;
        public static bool ChangeCondition;
        public static float Threshold;

        public override string Name => ":: Items :: Greens :: Rose Buckler";
        public override string InternalPickupToken => "sprintArmor";
        public override bool NewPickup => false;
        public override string PickupText => "";

        public override string DescText => (Armor != 0f ? "<style=cIsHealing>Increase armor</style> by <style=cIsHealing>" + Armor + "</style>" +
                                           (StackArmor ? " <style=cStack>(+" + Armor + " per stack)</style>" : "") +
                                           " and <style=cIsHealing>" + ConditionalArmor + "</style> <style=cStack>(+" + ConditionalArmor + " per stack)</style> " : "<style=cIsHealing>Increase armor</style> by <style=cIsHealing>" + ConditionalArmor + "</style> <style=cStack>(+" + ConditionalArmor + " per stack)</style>") +
                                           " while" +
                                           (ChangeCondition ? " <style=cIsHealth>under " + d(Threshold) + " health</style>." : " <style=cIsUtility>sprinting</style>.");

        public override void Init()
        {
            ConditionalArmor = ConfigOption(30, "Conditional Armor", "Per Stack. Vanilla is 30");
            Armor = ConfigOption(0f, "Armor", "Vanilla is 0");
            StackArmor = ConfigOption(false, "Stack Armor?", "Vanilla is false");
            ChangeCondition = ConfigOption(false, "Change Condition to Below Health Threshold?", "Vanilla is false");
            Threshold = ConfigOption(0.5f, "Health Threshold", "Decimal. Vanilla is 0");
            base.Init();
        }

        public override void Hooks()
        {
            if (ChangeCondition)
            {
                IL.RoR2.CharacterBody.RecalculateStats += ChangeBehavior;
                Insanity();
            }
            IL.RoR2.CharacterBody.RecalculateStats += ChangeArmor;
            RecalculateStatsAPI.GetStatCoefficients += AddBehavior;
        }

        public static void ChangeBehavior(ILContext il)
        {
            ILCursor c = new ILCursor(il);
            c.GotoNext(MoveType.Before,
                x => x.MatchCallOrCallvirt(typeof(CharacterBody).GetPropertyGetter(nameof(CharacterBody.isSprinting)))
            );
            c.GotoNext(MoveType.Before,
                x => x.MatchCallOrCallvirt(typeof(CharacterBody).GetPropertyGetter(nameof(CharacterBody.isSprinting)))
            );
            c.GotoNext(MoveType.After,
                x => x.MatchCallOrCallvirt(typeof(CharacterBody).GetPropertyGetter(nameof(CharacterBody.isSprinting)))
            );
            c.Emit(OpCodes.Ldarg_0);
            c.EmitDelegate<Func<bool, CharacterBody, bool>>((sprinting, body) => { return body.healthComponent.combinedHealthFraction < Threshold; });
        }

        public static void ChangeArmor(ILContext il)
        {
            ILCursor c = new ILCursor(il);
            c.GotoNext(MoveType.Before,
                x => x.MatchCallOrCallvirt<RoR2.CharacterBody>("get_armor"),
                x => x.MatchLdloc(out _),
                x => x.MatchLdcI4(30)
            );
            c.Index += 3;
            c.EmitDelegate<Func<int, int>>((sdfgsdfhgsghdfv) =>
            {
                return ConditionalArmor;
            });
        }

        public static void ChangeVisual(ILContext il)
        {
            ILCursor c = new ILCursor(il);
            c.GotoNext(MoveType.Before,
                x => x.MatchCallOrCallvirt<RoR2.CharacterBody>("get_isSprinting"),
                x => x.MatchBrfalse(out _),
                x => x.MatchLdarg(0),
                x => x.MatchCallOrCallvirt<RoR2.CharacterBody>("get_inventory"),
                x => x.MatchLdsfld<RoR2.ItemDef>("")
            );
            // unfinished still, lazy
        }

        public static void Insanity()
        {
            On.RoR2.HealthComponent.TakeDamage += (On.RoR2.HealthComponent.orig_TakeDamage orig, HealthComponent self, DamageInfo damageInfo) =>
            {
                orig(self, damageInfo);
                if (self.body && self.body.inventory)
                {
                    float health = self.body.inventory.GetItemCount(RoR2Content.Items.SprintArmor) > 0 ? self.combinedHealthFraction : 0f;
                    if (health >= Threshold && self.combinedHealthFraction < Threshold)
                    {
                        self.body.statsDirty = true;
                    }
                }
            };
            On.RoR2.HealthComponent.Heal += (On.RoR2.HealthComponent.orig_Heal orig, HealthComponent self, float amount, ProcChainMask procChainMask, bool nonRegen) =>
            {
                float ret = orig(self, amount, procChainMask, nonRegen);
                if (self.body && self.body.inventory)
                {
                    float health = self.body.inventory.GetItemCount(RoR2Content.Items.SprintArmor) > 0 ? self.combinedHealthFraction : 1f;
                    if (health < Threshold && self.combinedHealthFraction >= Threshold)
                    {
                        self.body.statsDirty = true;
                    }
                }
                return ret;
            };
        }

        public static void AddBehavior(CharacterBody sender, RecalculateStatsAPI.StatHookEventArgs args)
        {
            if (sender.inventory)
            {
                var stack = sender.inventory.GetItemCount(RoR2Content.Items.SprintArmor);
                if (stack > 0)
                {
                    args.armorAdd += (StackArmor ? Armor * stack : Armor);
                }
            }
        }
    }
}