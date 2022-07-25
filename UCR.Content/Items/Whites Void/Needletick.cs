/*
using Mono.Cecil.Cil;
using MonoMod.Cil;
using System;

namespace UltimateCustomRun.Items.VoidWhites
{
    public class Needletick : ItemBase
    {
        public static float Chance;
        public static int EliteNumber;

        public override string Name => ":: Items :::::: Void Whites :: Needletick";
        public override string InternalPickupToken => "bleedOnHitVoid";
        public override bool NewPickup => false;
        public override string PickupText => "";
        public override string DescText => "<style=cIsDamage>" + Chance + "%</style> <style=cStack>(+" + Chance + "% per stack)</style> chance to <style=cIsDamage>collapse</style> an enemy for <style=cIsDamage>400%</style> base damage. <style=cIsVoid>Corrupts all Tri-Tip Daggers</style>.";

        public override void Init()
        {
            Chance = ConfigOption(10f, "Chance", "Per Stack. Vanilla is 10");
            ROSOption("Void Whites", 0f, 25f, 1f, "6");
            EliteNumber = ConfigOption(10, "Elite/Wake of Vultures Needletick count", "Vanilla is 10");
            ROSOption("Void Whites", 0f, 10f, 1f, "6");
            base.Init();
        }

        public override void Hooks()
        {
            //  IL.RoR2.GlobalEventManager.OnHitEnemy += ChangeChance;
            IL.RoR2.GlobalEventManager.OnHitEnemy += ChangeBullshit;
        }

        private void ChangeBullshit(ILContext il)
        {
            ILCursor c = new(il);

            c.GotoNext(MoveType.Before,
                x => x.MatchLdcI4(10),
                x => x.MatchAdd(),
                x => x.MatchStloc(24)
            );

            c.Remove();
            c.Emit(OpCodes.Ldc_I4_S, EliteNumber);

            c.Remove();
            c.Emit(OpCodes.Ldc_I4_S, EliteNumber);

            c.Next.Operand = EliteNumber;

            c.Index += 1;
            c.Emit(OpCodes.Ldarg_0);
            c.EmitDelegate<Func<int, int, int>>((c, self) =>
            {
                return EliteNumber;
            });
        }

        private void ChangeChance(ILContext il)
        {
            ILCursor c = new(il);

            c.GotoNext(MoveType.Before,
                x => x.MatchLdcR4(10),
                x => x.MatchMul(),
                x => x.MatchLdloc(4)
            );
            c.Index += 1;
            c.Emit(OpCodes.Ldarg_0);
            c.EmitDelegate<Func<float, float, float>>((dam, self) =>
            {
                return Chance;
            });
        }
}
*/
// Specified cast not valid, not even RandomlyAwesome could figure out why
// On hold for now, too lazy to fix EmitDelegates