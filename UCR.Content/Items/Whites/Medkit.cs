using MonoMod.Cil;
using RoR2;
using UnityEngine;

namespace UltimateCustomRun.Items.Whites
{
    public class Medkit : ItemBase
    {
        public static float FlatHealing;
        public static float PercentHealing;
        public static bool StackBuff;
        public static bool IsDebuff;

        public override string Name => ":: Items : Whites :: Medkit";
        public override string InternalPickupToken => "medkit";
        public override bool NewPickup => false;

        public override string PickupText => "";

        public override string DescText => "2 seconds after getting hurt, <style=cIsHealing>heal</style> for <style=cIsHealing>" + FlatHealing + "</style> plus an additional <style=cIsHealing>" + d(PercentHealing) + " <style=cStack>(+" + d(PercentHealing) + " per stack)</style></style> of <style=cIsHealing>maximum health</style>.";

        public override void Init()
        {
            FlatHealing = ConfigOption(20f, "Flat Healing", "Vanilla is 20");
            ROSOption("Whites", 0f, 50f, 5f, "1");
            PercentHealing = ConfigOption(0.05f, "Percent Healing", "Decimal. Per Stack. Vanilla is 0.05");
            ROSOption("Whites", 0f, 1f, 0.01f, "1");
            StackBuff = ConfigOption(false, "Stack Buff?", "Vanilla is false");
            ROSOption("Whites", 0f, 5f, 0.01f, "1");
            base.Init();
        }

        public override void Hooks()
        {
            IL.RoR2.CharacterBody.RemoveBuff_BuffIndex += ChangeFlatHealing;
            IL.RoR2.CharacterBody.RemoveBuff_BuffIndex += ChangePercentHealing;
        }

        public static void ChangeFlatHealing(ILContext il)
        {
            ILCursor c = new(il);

            c.GotoNext(MoveType.Before,
                x => x.MatchLdsfld(typeof(RoR2Content.Items), "Medkit"),
                x => x.MatchCallOrCallvirt<Inventory>("GetItemCount"),
                x => x.MatchStloc(0),
                x => x.MatchLdcR4(20f)
            ); ;
            c.Index += 2;
            c.Next.Operand = FlatHealing;
        }

        public static void ChangePercentHealing(ILContext il)
        {
            ILCursor c = new(il);

            c.GotoNext(MoveType.Before,
                x => x.MatchLdarg(0),
                x => x.MatchCallOrCallvirt(typeof(CharacterBody).GetMethod("get_maxHealth")),
                x => x.MatchLdcR4(0.05f)
            );
            c.Index += 2;
            c.Next.Operand = PercentHealing;
        }
    }
}