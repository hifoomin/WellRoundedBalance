using MonoMod.Cil;
using System;

namespace WellRoundedBalance.Equipment
{
    public class TheCrowdfunder : EquipmentBase
    {
        public override string Name => ":: Equipment :: The Crowdfunder";
        public override EquipmentDef InternalPickup => RoR2Content.Equipment.GoldGat;

        public override string PickupText => "Toggle to fire. Costs gold per bullet.";

        public override string DescText => "Wind up a continuous barrage that shoots up to <style=cIsDamage>" + maximumFireRate + " times per second</style>, dealing <style=cIsDamage>" + d(damage) + " damage per shot</style> (extremely low). Costs $1 per bullet." +
                                           (fixGoldScaling ? " Cost scales over time." : " Cost increases by $1 every 4 levels.");

        [ConfigField("Wind Up Duration", "", 3f)]
        public static float windUpDuration;

        [ConfigField("Minimum Fire Rate", "", 3f)]
        public static float minimumFireRate;

        [ConfigField("Maximum Fire Rate", "", 7f)]
        public static float maximumFireRate;

        [ConfigField("Fix Gold Scaling?", "", true)]
        public static bool fixGoldScaling;

        [ConfigField("Proc Coefficient", "", 1f)]
        public static float procCoefficient;

        [ConfigField("Damage", "Decimal.", 1f)]
        public static float damage;

        public override void Init()
        {
            base.Init();
        }

        public override void Hooks()
        {
            On.EntityStates.GoldGat.GoldGatFire.OnEnter += Changes;
            if (fixGoldScaling)
            {
                IL.EntityStates.GoldGat.GoldGatFire.FireBullet += GoldGatFire_FireBullet;
            }
            Changess();
        }

        private void GoldGatFire_FireBullet(ILContext il)
        {
            ILCursor c = new(il);

            if (c.TryGotoNext(MoveType.Before,
                x => x.MatchConvU8(),
                x => x.MatchLdloc(out _)))
            {
                c.Index += 2;
                c.EmitDelegate<Func<int, int>>((self) =>
                {
                    return Run.instance ? Run.instance.GetDifficultyScaledCost(1) : self;
                });
            }
            else
            {
                Logger.LogError("Failed to apply The Crowdfunder Gold Scaling hook");
            }
        }

        private void Changes(On.EntityStates.GoldGat.GoldGatFire.orig_OnEnter orig, EntityStates.GoldGat.GoldGatFire self)
        {
            EntityStates.GoldGat.GoldGatFire.windUpDuration = windUpDuration;
            EntityStates.GoldGat.GoldGatFire.minFireFrequency = minimumFireRate;
            EntityStates.GoldGat.GoldGatFire.maxFireFrequency = maximumFireRate;
            EntityStates.GoldGat.GoldGatFire.procCoefficient = procCoefficient;
            EntityStates.GoldGat.GoldGatFire.damageCoefficient = damage;
            orig(self);
        }

        private void Changess()
        {
            var crunderTracer = Utils.Paths.GameObject.TracerGoldGat.Load<GameObject>();
            crunderTracer.transform.localScale = new Vector3(2f, 2f, 2f);
        }
    }
}