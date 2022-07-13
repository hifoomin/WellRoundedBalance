using Mono.Cecil.Cil;
using MonoMod.Cil;
using System;
using System.Collections.Generic;
using System.Text;

namespace UltimateCustomRun
{
    public class Commando : SurvivorBase
    {
        public static float M1Damage;
        public static float M1FireRate;
        public static float M1ProcCoefficient;
        public static int M1FalloffType;
        public static float M1Radius;

        public static float M2Damage;
        public static float M2ProcCoefficient;

        public static int AltM2Shots;
        public static float AltM2Damage;
        public static float AltM2ProcCoefficient;
        public static int AltM2FalloffType;

        public static float UtilityInitialSpeedCoefficient;
        public static float UtilityFinalSpeedCoefficient;

        public static float AltUtilitySpeedCoefficient;

        public static int SpecialShots;
        public static float SpecialDamage;
        public static float SpecialFireRate;

        public static float AltSpecialDamage;
        public static float AltSpecialRadius;
        public static float AltSpecialProcCoefficient;

        public override string Name => "Skills :: Commando";

        public override string InternalName => "commando";

        public override bool PassiveExists => false;

        public override string PassiveDesc => "";

        public override bool AltPassiveExists => false;

        public override string AltPassiveDesc => "";

        public override string M1Desc => "Rapidly shoot an enemy for <style=cIsDamage>" + d(M1Damage) + " damage</style>.";

        public override bool AltM1Exists => false;

        public override string AltM1Token => "";

        public override string AltM1Desc => "";

        public override bool SecondAltM1Exists => false;

        public override string SecondAltM1Token => "";

        public override string SecondAltM1Desc => "";

        public override bool ThirdAltM1Exists => false;

        public override string ThirdAltM1Token => "";

        public override string ThirdAltM1Desc => "";

        public override string M2Desc => "Fire a <style=cIsDamage>piercing</style> bullet for <style=cIsDamage>" + d(M2Damage) + " damage</style>. Deals <style=cIsDamage>40%</style> more damage every time it passes through an enemy.";

        public override bool AltM2Exists => true;

        public override string AltM2Token => "secondary_alt1";

        public override string AltM2Desc => "Fire two close-range blasts that deal <style=cIsDamage>" + AltM2Shots + "x" + d(AltM2Damage) + " damage</style> total.";

        public override string UtilityDesc => "<style=cIsUtility>Roll</style> a short distance.";

        public override bool AltUtilityExists => true;

        public override string AltUtilityToken => "utility_alt";

        public override string AltUtilityDesc => "style=cIsUtility>Slide</style> on the ground for a short distance. You can <style=cIsDamage>fire while sliding</style>.";

        public override string SpecialDesc => "<style=cIsDamage>Stunning</style>. Fire repeatedly for <style=cIsDamage>" + SpecialShots + "x" + d(SpecialDamage) + " damage</style> per bullet. The number of shots increases with attack speed.";

        public override bool AltSpecialExists => true;

        public override string AltSpecialToken => "special_alt1";

        public override string AltSpecialDesc => "Throw a grenade that explodes for <style=cIsDamage>" + d(AltSpecialDamage) + " damage</style>. Can hold up to 2.";

        public override bool SecondAltSpecialExists => false;

        public override string SecondAltSpecialToken => "";

        public override string SecondAltSpecialDesc => "";

        public override bool ThirdAltSpecialExists => false;

        public override string ThirdAltSpecialToken => "";

        public override string ThirdAltSpecialDesc => "";

        public override void Init()
        {
            M1Damage = ConfigOption(1f, "M1 Damage Coefficient", "Decimal. Vanilla is 1");
            M1FireRate = ConfigOption(6f, "M1 Fire Rate", "Vanilla is 6");
            M1Radius = ConfigOption(0.1f, "M1 Radius", "Vanilla is 0.1");
            M1FalloffType = ConfigOption(1, "M1 Falloff Type", "1 - Default, 2 - Buckshot, 3 - None.\nVanilla is 1");
            base.Init();
        }

        public override void Hooks()
        {
            On.EntityStates.Commando.CommandoWeapon.FirePistol2.OnEnter += M1Changes;
            IL.EntityStates.Commando.CommandoWeapon.FirePistol2.FireBullet += M1Changes2;
            IL.EntityStates.Commando.CommandoWeapon.FirePistol2.FireBullet += M1Changes3;
            IL.EntityStates.Commando.CommandoWeapon.FirePistol2.FireBullet += M1Changes4;
        }

        public static void M1Changes3(ILContext il)
        {
            ILCursor c = new(il);

            c.GotoNext(MoveType.After,
                x => x.MatchStfld("RoR2.BulletAttack", "smartCollision")
            );
            c.Emit(OpCodes.Ldc_R4, M1ProcCoefficient);
            c.Emit(OpCodes.Stfld, ("RoR2.BulletAttack", "procCoefficient"));
            //c.Emit(OpCodes.Dup);
        }

        public static void M1Changes4(ILContext il)
        {
            ILCursor c = new(il);

            c.GotoNext(MoveType.After,
                x => x.MatchStfld("RoR2.BulletAttack", "smartCollision")
            );
            c.Emit(OpCodes.Ldc_I4, M1FalloffType);
            c.Emit(OpCodes.Stfld, ("RoR2.BulletAttack", "procCoefficient"));
            //c.Emit(OpCodes.Dup);
        }

        /*
            StickyBombImpact.falloffModel = Falloff switch
            {
                1 => BlastAttack.FalloffModel.Default,
                2 => BlastAttack.FalloffModel.Buckshot,
                3 => BlastAttack.FalloffModel.None,
                _ => BlastAttack.FalloffModel.Default,
            };
        */

        public static void M1Changes2(ILContext il)
        {
            ILCursor c = new(il);

            c.GotoNext(MoveType.Before,
                x => x.MatchLdcR4(0.1f)
            );
            c.Next.Operand = M1Radius;
        }

        public static void M1Changes(On.EntityStates.Commando.CommandoWeapon.FirePistol2.orig_OnEnter orig, EntityStates.Commando.CommandoWeapon.FirePistol2 self)
        {
            EntityStates.Commando.CommandoWeapon.FirePistol2.baseDuration = 1f / M1FireRate;
            EntityStates.Commando.CommandoWeapon.FirePistol2.damageCoefficient = M1Damage;
            orig(self);
        }
    }
}