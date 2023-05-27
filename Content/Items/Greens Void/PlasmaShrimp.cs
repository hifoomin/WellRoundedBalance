using RoR2.Audio;

namespace WellRoundedBalance.Items.VoidGreens
{
    public class PlasmaShrimp : ItemBase<PlasmaShrimp>
    {
        public override string Name => ":: Items :::::: Voids :: Plasma Shrimp";
        public override ItemDef InternalPickup => DLC1Content.Items.MissileVoid;

        public override string PickupText => "While you have shield, fire missiles on every hit. <style=cIsVoid>Corrupts all AtG Missile Mk. 1s</style>.";
        public override string DescText => "Gain a <style=cIsHealing>shield</style> equal to <style=cIsHealing>" + d(percentShield) + "</style> of your maximum health. While you have a <style=cIsHealing>shield</style>, hitting an enemy fires a missile that deals <style=cIsDamage>" + d(totalDamage) + "</style> <style=cStack>(+" + d(totalDamage) + " per stack)</style> TOTAL damage. <style=cIsVoid>Corrupts all AtG Missile Mk. 1s</style>.";

        [ConfigField("TOTAL Damage", "Decimal.", 0.2f)]
        public static float totalDamage;

        [ConfigField("Percent Shield", "Decimal.", 0.2f)]
        public static float percentShield;

        [ConfigField("Proc Coefficient", 0.2f)]
        public static float procCoefficient;

        public override void Init()
        {
            base.Init();
        }

        public override void Hooks()
        {
            IL.RoR2.CharacterBody.RecalculateStats += CharacterBody_RecalculateStats;
            IL.RoR2.GlobalEventManager.OnHitEnemy += GlobalEventManager_OnHitEnemy;
            GlobalEventManager.onServerDamageDealt += GlobalEventManager_onServerDamageDealt;
            On.RoR2.Inventory.GiveItem_ItemIndex_int += Inventory_GiveItem_ItemIndex_int;
            Changes();
        }

        private void Inventory_GiveItem_ItemIndex_int(On.RoR2.Inventory.orig_GiveItem_ItemIndex_int orig, Inventory self, ItemIndex itemIndex, int count)
        {
            if (NetworkServer.active)
            {
                if (itemIndex == DLC1Content.Items.MissileVoid.itemIndex)
                {
                    var master = self.gameObject.GetComponent<CharacterMaster>();
                    if (master)
                    {
                        var body = master.GetBody();
                        if (body)
                        {
                            body.gameObject.AddComponent<PlasmaShrimpController>();
                        }
                    }
                }
            }

            orig(self, itemIndex, count);
        }

        private void GlobalEventManager_onServerDamageDealt(DamageReport report)
        {
            if (report.damageInfo.procCoefficient <= 0)
            {
                return;
            }

            var attackerBody = report.attackerBody;
            if (!attackerBody)
            {
                return;
            }

            var victim = report.victim;
            if (!victim)
            {
                return;
            }

            var inventory = attackerBody.inventory;
            if (!inventory)
            {
                return;
            }

            var attackerHealthComponent = attackerBody.healthComponent;
            if (!attackerHealthComponent)
            {
                return;
            }

            var plimpComponent = attackerBody.GetComponent<PlasmaShrimpController>();
            if (!plimpComponent)
            {
                return;
            }

            var stack = inventory.GetItemCount(DLC1Content.Items.MissileVoid);
            if (stack <= 0)
            {
                attackerBody.gameObject.RemoveComponent<PlasmaShrimpController>();
                return;
            }

            if (attackerHealthComponent.shield > 0f)
            {
                if (plimpComponent.canPlayOrbSound)
                {
                    Util.PlaySound("Play_item_void_critGlasses", attackerBody.gameObject);
                    plimpComponent.canPlayOrbSound = false;
                }
                if (plimpComponent.canPlayImpactSound)
                {
                    Util.PlaySound("Play_item_void_missle_explode", victim.gameObject);
                    plimpComponent.canPlayImpactSound = false;
                }
            }
        }

        private void GlobalEventManager_OnHitEnemy(ILContext il)
        {
            ILCursor c = new(il);

            if (c.TryGotoNext(MoveType.Before,
                    x => x.MatchLdcR4(0.4f)))
            {
                c.Next.Operand = totalDamage;
            }
            else
            {
                Logger.LogError("Failed to apply Plasma Shrimp Damage hook");
            }

            c.Index = 0;

            if (c.TryGotoNext(MoveType.Before,
                    x => x.MatchLdcR4(0.2f),
                    x => x.MatchStfld<RoR2.Orbs.GenericDamageOrb>("procCoefficient")))
            {
                c.Next.Operand = procCoefficient * globalProc;
            }
            else
            {
                Logger.LogError("Failed to apply Plasma Shrimp Proc Coefficient hook");
            }
        }

        private void CharacterBody_RecalculateStats(ILContext il)
        {
            ILCursor c = new(il);

            if (c.TryGotoNext(MoveType.Before,
                    x => x.MatchCallOrCallvirt<CharacterBody>("get_maxHealth"),
                    x => x.MatchLdcR4(0.1f)))
            {
                c.Index += 1;
                c.Next.Operand = percentShield;
            }
            else
            {
                Logger.LogError("Failed to apply Plasma Shrimp Shield hook");
            }
        }

        private void Changes()
        {
            var orbEffect = Utils.Paths.GameObject.MissileVoidOrbEffect.Load<GameObject>();
            var effectComponent = orbEffect.GetComponent<EffectComponent>();
            effectComponent.soundName = null;

            var impactEffect = Utils.Paths.GameObject.VoidImpactEffect.Load<GameObject>();
            var effectComponent2 = impactEffect.GetComponent<EffectComponent>();
            effectComponent2.soundName = null;
        }
    }

    public class PlasmaShrimpController : MonoBehaviour
    {
        public float orbTimer;
        public float impactTimer;
        public float orbInterval = 0.5f;
        public float impactInterval = 0.33f;
        public bool canPlayOrbSound = true;
        public bool canPlayImpactSound = true;

        public void FixedUpdate()
        {
            if (canPlayOrbSound == false)
            {
                orbTimer += Time.fixedDeltaTime;
                if (orbTimer >= orbInterval)
                {
                    canPlayOrbSound = true;
                    orbTimer = 0f;
                }
            }
            if (canPlayImpactSound == false)
            {
                impactTimer += Time.fixedDeltaTime;
                if (impactTimer >= impactInterval)
                {
                    canPlayImpactSound = true;
                    impactTimer = 0f;
                }
            }
        }
    }
}