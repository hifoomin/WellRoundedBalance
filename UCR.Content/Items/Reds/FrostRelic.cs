using MonoMod.Cil;
using RoR2;
using UnityEngine;

namespace UltimateCustomRun.Items.Reds
{
    public class FrostRelic : ItemBase
    {
        public static float AttackInterval;
        public static float BaseRadius;
        public static float RadiusPerKill;
        public static float Damage;
        public static float Duration;
        public static float ProcCoefficient;
        public static int Maximum;
        public static int StackMaximum;
        public static bool CameraChanges;
        public static bool Guide;
        public override string Name => ":: Items ::: Reds :: Frost Relic";
        public override string InternalPickupToken => "icicle";
        public override bool NewPickup => false;
        public override string PickupText => "";

        public override string DescText => "Killing an enemy surrounds you with an <style=cIsDamage>ice storm</style> that deals <style=cIsDamage>" + d(Damage / AttackInterval) + " damage per second</style> and <style=cIsUtility>slows</style> enemies by <style=cIsUtility>80%</style> for <style=cIsUtility>1.5s</style>. The storm <style=cIsDamage>grows with every kill</style>, increasing its radius by <style=cIsDamage>" + RadiusPerKill + "m</style>. Stacks up to <style=cIsDamage>" + (BaseRadius + RadiusPerKill * Maximum) + "m</style> <style=cStack>(+" + RadiusPerKill * StackMaximum + "m per stack)</style>.";

        public override void Init()
        {
            AttackInterval = ConfigOption(0.25f, "Attack Interval", "Vanilla is 0.25");
            BaseRadius = ConfigOption(6f, "Base Radius", "Vanilla is 6");
            RadiusPerKill = ConfigOption(2f, "Icicle Radius", "Per Icicle. Vanilla is 2");
            Damage = ConfigOption(3f, "Damage Per Tick", "Decimal. Vanilla is 3");
            Duration = ConfigOption(5f, "Duration", "Vanilla is 5");
            ProcCoefficient = ConfigOption(0.2f, "Proc Coefficient Per Tick", "Vanilal is 0.2");
            Maximum = ConfigOption(6, "Maximum Icicles Amount", "Vanilla is 6");
            StackMaximum = ConfigOption(6, "Stack Maximum Icicles Amount", "Per Stack. Vanilla is 6");
            CameraChanges = ConfigOption(false, "Disable Camera Changes?", "Vanilla is false");
            Guide = ConfigOption(true, "Formulas", "1 / Attack Interval = Attacks per Second\nBase Radius * (Base Icicle Cap + Icicle Cap) = Max Radius\nDamage / Attack Interval = DPS\nProc Coefficient / Attack Interval = PPS");
            base.Init();
        }

        public override void Hooks()
        {
            Changes();
            if (CameraChanges)
            {
                IL.RoR2.IcicleAuraController.OnIciclesActivated += ChangeCamera;
            }
        }

        public static void Changes()
        {
            var f = LegacyResourcesAPI.Load<GameObject>("prefabs/networkedobjects/IcicleAura");
            var fi = f.GetComponent<IcicleAuraController>();
            fi.baseIcicleAttackInterval = Duration;
            // (1 / Interval) = aps
            fi.icicleBaseRadius = BaseRadius;
            // 6 + max icicles * Radius per icicle
            fi.icicleRadiusPerIcicle = RadiusPerKill;
            fi.icicleDamageCoefficientPerTick = Damage;
            // Damage coeff * (1 / Interval) = dps
            fi.icicleDuration = Duration;
            fi.icicleProcCoefficientPerTick = ProcCoefficient;
            // proc coeff * (1 / Interval) = pps
            fi.baseIcicleMax = Maximum;
            // max icicles
            fi.icicleMaxPerStack = StackMaximum;
            // per stack
            var fb = f.GetComponent<BuffWard>();
            fb.interval = Duration;
        }

        public static void ChangeCamera(ILContext il)
        {
            ILCursor c = new(il);

            c.GotoNext(MoveType.Before,
                x => x.MatchLdfld<IcicleAuraController.OwnerInfo>("cameraTargetParams")
            );
        }
    }
}