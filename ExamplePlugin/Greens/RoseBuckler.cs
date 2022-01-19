using R2API;
using R2API.Utils;
using RoR2;
using Mono.Cecil.Cil;
using MonoMod.Cil;
using System;

namespace UltimateCustomRun
{
    static class RoseBuckler
    {
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
            c.EmitDelegate<Func<bool, CharacterBody, bool>>((sprinting, body) => { return body.healthComponent.combinedHealthFraction < Main.RoseBucklerThreshold.Value; });
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
                return Main.RoseBucklerArmor.Value;
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
                float health = self.body.inventory.GetItemCount(RoR2Content.Items.SprintArmor) > 0 ? self.combinedHealthFraction : 0f;
                orig(self, damageInfo);
                if (health >= Main.RoseBucklerThreshold.Value && self.combinedHealthFraction < Main.RoseBucklerThreshold.Value)
                {
                    self.body.statsDirty = true;
                }

            };
            On.RoR2.HealthComponent.Heal += (On.RoR2.HealthComponent.orig_Heal orig, HealthComponent self, float amount, ProcChainMask procChainMask, bool nonRegen) =>
            {
                float health = self.body.inventory.GetItemCount(RoR2Content.Items.SprintArmor) > 0 ? self.combinedHealthFraction : 1f;
                float ret = orig(self, amount, procChainMask, nonRegen);
                if (health < Main.RoseBucklerThreshold.Value && self.combinedHealthFraction >= Main.RoseBucklerThreshold.Value)
                {
                    self.body.statsDirty = true;
                }
                return ret;
            };
            // im going insane why is this just the j
        }

        public static void AddBehavior(CharacterBody sender, RecalculateStatsAPI.StatHookEventArgs args)
        {
            if (sender.inventory)
            {
                var stack = sender.inventory.GetItemCount(RoR2Content.Items.SprintArmor);
                if (stack > 0)
                {
                    args.armorAdd += Main.RoseBucklerArmorAlways.Value * stack;
                }
            }
        }
    }
}
