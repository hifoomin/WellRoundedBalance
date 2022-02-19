using BepInEx;
using R2API;
using R2API.Utils;
using RoR2;
using RoR2.Skills;
using RoR2.Orbs;
using RoR2.Projectile;
using UnityEngine;
using BepInEx.Configuration;
using MonoMod.Cil;

namespace UltimateCustomRun
{
    public class BrilliantBehemoth : ItemBase
    {
        public static float dmg;
        public static float aoe;
        public static float aoestack;
        public override string Name => ":: Items ::: Reds :: Brilliant Behemoth";
        public override string InternalPickupToken => "behemoth";
        public override bool NewPickup => false;
        public override string PickupText => "";

        public override string DescText => "All your <style=cIsDamage>attacks explode</style> in a <style=cIsDamage>" + aoe + "m </style> <style=cStack>(+" + aoestack + "m per stack)</style> radius for a bonus <style=cIsDamage>60%</style> TOTAL damage to nearby enemies.";
        public override void Init()
        {
            dmg = ConfigOption(0.6f, "Damage Increase", "Decimal. Vanilla is 0.6");
            aoe = ConfigOption(4f, "Base Area of Effect", "Vanilla is 4");
            aoestack = ConfigOption(2.5f, "Stack Area of Effect", "Per Stack. Vanilla is 2.5");
            base.Init();
        }

        public override void Hooks()
        {
            IL.RoR2.GlobalEventManager.OnHitAll += ChangeDamage;
            IL.RoR2.GlobalEventManager.OnHitAll += ChangeRadius;
        }
        public static void ChangeDamage(ILContext il)
        {
            ILCursor c = new ILCursor(il);

            c.GotoNext(MoveType.Before,
                x => x.MatchLdcR4(0.6f)
            );
            c.Next.Operand = dmg;
        }
        public static void ChangeRadius(ILContext il)
        {
            ILCursor c = new ILCursor(il);

            c.GotoNext(MoveType.Before,
                x => x.MatchLdcR4(1.5f),
                x => x.MatchLdcR4(2.5f)
            );
            c.Next.Operand = aoe - aoestack;
            c.Index += 1;
            c.Next.Operand = aoestack;
        }
    }
}
