/*
using MonoMod.Cil;
using RoR2;
using RoR2.Projectile;
using System.Collections.Generic;
using UnityEngine;

namespace UltimateCustomRun.Items.Yellows
{
    public class MiredUrn : ItemBase
    {
        public static int Limit;
        public static int StackLimit;

        public override string Name => ":: Items :::: Yellows :: Mired Urn";
        public override string InternalPickupToken => "siphonOnLowHealth";
        public override bool NewPickup => false;

        public override string PickupText => "";

        public override string DescText => "While in combat, the nearest 1<style=cStack>(+1 per stack)</style> enemies to you within <style=cIsDamage>13m</style> will be 'tethered' to you, dealing <style=cIsDamage>100%</style> damage per second, applying <style=cIsDamage>tar</style>, and <style=cIsHealing>healing</style> you for <style=cIsHealing>100%</style> of the damage dealt.";

        public override void Init()
        {
            Limit = ConfigOption(4, "Ally Limit", "Vanilla is 4");
            StackLimit = ConfigOption(4, "Stack Ally Limit", "Per Stack. Vanilla is 4");
            base.Init();
        }

        public override void Hooks()
        {
            On.RoR2.Items.SiphonOnLowHealthItemBodyBehavior.OnEnable += Changes;
        }

        private void Changes(On.RoR2.Items.SiphonOnLowHealthItemBodyBehavior.orig_OnEnable orig, RoR2.Items.SiphonOnLowHealthItemBodyBehavior self)
        {
            var prefab = LegacyResourcesAPI.Load<GameObject>("Prefabs/NetworkedObjects/BodyAttachments/SiphonNearbyBodyAttachment");
            Main.UCRLogger.LogInfo("mired urn prefab has these components: " + prefab.GetComponents(typeof(Component)));
            orig(self);
        }
    }
}
*/