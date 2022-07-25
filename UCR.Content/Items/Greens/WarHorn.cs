using Mono.Cecil.Cil;
using MonoMod.Cil;

namespace UltimateCustomRun.Items.Greens
{
    public class Warhorn : ItemBase
    {
        public static float Duration;
        public static int BaseDuration;
        public static int StackDuration;

        public override string Name => ":: Items :: Greens :: War Horn";
        public override string InternalPickupToken => "energizedOnEquipmentUse";
        public override bool NewPickup => false;
        public override string PickupText => "";
        public override string DescText => "Activating your Equipment gives you <style=cIsDamage>+" + d(Duration) + " attack speed</style> for <style=cIsDamage>" + BaseDuration + "s</style> <style=cStack>(+" + StackDuration + "s per stack)</style>.";

        public override void Init()
        {
            Duration = ConfigOption(0.7f, "Attack Speed", "Decimal. Vanilla is 0.7");
            BaseDuration = ConfigOption(8, "Base Duration", "Vanilla is 8");
            StackDuration = ConfigOption(4, "Stack Duration", "Per Stack. Vanilla is 4");
            base.Init();
        }

        public override void Hooks()
        {
            IL.RoR2.CharacterBody.RecalculateStats += ChangeAS;
            IL.RoR2.EquipmentSlot.OnEquipmentExecuted += ChangeDuration;
        }

        public static void ChangeAS(ILContext il)
        {
            ILCursor c = new(il);

            c.GotoNext(MoveType.Before,
                //x => x.MatchLdsfld("RoR2.RoR2Content/Buffs", "Energized"),
                //x => x.MatchCallOrCallvirt<RoR2.CharacterBody>("HasBuff"),
                //x => x.MatchBrfalse(out _),
                //x => x.MatchLdloc(out _),
                x => x.MatchLdcR4(0.7f)
            );
            //c.Index += 4;
            c.Next.Operand = Duration;
        }

        public static void ChangeDuration(ILContext il)
        {
            ILCursor c = new(il);

            c.GotoNext(MoveType.Before,
                x => x.MatchLdcI4(8),
                x => x.MatchLdcI4(4)
            );
            c.Remove();
            c.Emit(OpCodes.Ldc_I4, BaseDuration);
            c.Index += 1;
            c.Remove();
            c.Emit(OpCodes.Ldc_I4, StackDuration);
        }
    }
}