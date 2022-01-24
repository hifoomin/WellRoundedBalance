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
            // [Error  : Unity Log] NullReferenceException: Object reference not set to an instance of an object
            // Stack trace:
            // UltimateCustomRun.RoseBuckler+<>c.<Insanity>b__3_0 (On.RoR2.HealthComponent+orig_TakeDamage orig, RoR2.HealthComponent self, RoR2.DamageInfo damageInfo) (at <064868df148e436dadbbf814800d76e5>:IL_000C)
            // DMD<>?-165335808.Hook<RoR2.HealthComponent::TakeDamage>?-1564710144 (RoR2.HealthComponent , RoR2.DamageInfo ) (at <5a36487301ef4d1e9184a5bbbf5c5755>:IL_0014)
            // RoR2.BulletAttack.DefaultHitCallback (RoR2.BulletAttack+BulletHit& hitInfo) (at <da7c19fa62814b28bdb8f3a9223868e1>:IL_02CE)
            // RoR2.BulletAttack.ProcessHit (RoR2.BulletAttack+BulletHit& hitInfo) (at <da7c19fa62814b28bdb8f3a9223868e1>:IL_0028)
            // RoR2.BulletAttack.ProcessHitList (System.Collections.Generic.List`1[T] hits, UnityEngine.Vector3& endPosition, System.Collections.Generic.List`1[T] ignoreList) (at <da7c19fa62814b28bdb8f3a9223868e1>:IL_0099)
            // DMD<FireSingle>?-165335808._RoR2_BulletAttack::FireSingle (RoR2.BulletAttack this, UnityEngine.Vector3 normal, System.Int32 muzzleIndex) (at <78e82462a1754689ab787937e3526634>:IL_019E)
            // DMD<>?-165335808.Trampoline<RoR2.BulletAttack::FireSingle>?1114247168 (RoR2.BulletAttack , UnityEngine.Vector3 , System.Int32 ) (at <e1cb1e725a0349fd85503abc6beab3d4>:IL_0020)
            // SillyHitboxViewer.HitboxViewerMod.BulletAttack_FireSingle (On.RoR2.BulletAttack+orig_FireSingle orig, RoR2.BulletAttack self, UnityEngine.Vector3 normal, System.Int32 muzzleIndex) (at <758913ef92f24ad49857526c5212b051>:IL_0001)
            // DMD<>?-165335808.Hook<RoR2.BulletAttack::FireSingle>?2080950144 (RoR2.BulletAttack , UnityEngine.Vector3 , System.Int32 ) (at <b8eb0e722b3e4ea8b64c060039ce4fb2>:IL_0014)
            // RoR2.BulletAttack.Fire () (at <da7c19fa62814b28bdb8f3a9223868e1>:IL_018E)
            // EntityStates.Commando.CommandoWeapon.FirePistol2.FireBullet (System.String targetMuzzle) (at <da7c19fa62814b28bdb8f3a9223868e1>:IL_0118)
            // EntityStates.Commando.CommandoWeapon.FirePistol2.OnEnter () (at <da7c19fa62814b28bdb8f3a9223868e1>:IL_0050)
            // RoR2.EntityStateMachine.SetState (EntityStates.EntityState newState) (at <da7c19fa62814b28bdb8f3a9223868e1>:IL_0050)
            // RoR2.EntityStateMachine.FixedUpdate () (at <da7c19fa62814b28bdb8f3a9223868e1>:IL_0008)
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
