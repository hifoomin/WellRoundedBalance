/*using Mono.Cecil.Cil;
using MonoMod.Cil;
using RoR2;
using RoR2.Items;
using RoR2.Projectile;
using System;
using UnityEngine;

namespace WellRoundedBalance.Items.Yellows
{
    public class Planula : ItemBase
    {
        public override string Name => ":: Items :::: Yellows :: Planula";
        public override string InternalPickupToken => "parentEgg";

        public override string PickupText => "Summon the unmatched power of the sun after standing still for 0.5 seconds.";

        public override string DescText => "After standing still for <style=cIsHealing>0.5</style> seconds, summon a miniature sun for <style=cIsUtility>10</style> seconds that damages all monsters. Recharges every <style=cIsUtility>60</style> <style=cStack>(-33% per stack)</style> seconds.";

        public override void Init()
        {
            Init();
        }

        public override void Hooks()
        {
            IL.RoR2.HealthComponent.TakeDamage += HealthComponent_TakeDamage;
            CharacterBody.onBodyInventoryChangedGlobal += CharacterBody_onBodyInventoryChangedGlobal;
        }

        private void CharacterBody_onBodyInventoryChangedGlobal(CharacterBody body)
        {
            if (NetworkServer.active)
            {
                var stack = body.inventory.GetItemCount(RoR2Content.Items.ParentEgg);
                body.AddItemBehavior<PlanulaBehavior>(stack);
            }
        }

        private void HealthComponent_TakeDamage(ILContext il)
        {
            ILCursor c = new(il);

            if (c.TryGotoNext(MoveType.Before,
                x => x.MatchLdcR4(15f),
                x => x.MatchMul()))
            {
                c.Next.Operand = 0f;
            }
            else
            {
                Main.WRBLogger.LogError("Failed to apply Planula Healing hook");
            }
        }
    }

    public class PlanulaBehavior : CharacterBody.ItemBehavior
    {
        private void FixedUpdate()
        {
            if (!NetworkServer.active)
            {
                return;
            }

            var condition = stack > 0 && body.GetNotMoving();

            if (sunGameObject != condition)
            {
                if (condition)
                {
                    sunGameObject = Instantiate(sunPrefab, body.footPosition + new Vector3(0f, 200f, 0f), Quaternion.identity);
                    sunGameObject.GetComponent<GenericOwnership>().ownerObject = base.gameObject;
                    NetworkServer.Spawn(sunGameObject);
                }
                else
                {
                    Destroy(sunGameObject);
                    sunGameObject = null;
                }
            }
            /*if (sunTeamFilter)
            {
                sunTeamFilter.teamIndex = body.teamComponent.teamIndex;
            }
        }

        private void OnDisable()
        {
            if (sunGameObject)
            {
                Destroy(sunGameObject);
            }
        }

        private static GameObject sunPrefab = Utils.Paths.GameObject.GrandParentSun.Load<GameObject>();

        private GameObject sunGameObject;

        private TeamFilter sunTeamFilter;
    }
}*/