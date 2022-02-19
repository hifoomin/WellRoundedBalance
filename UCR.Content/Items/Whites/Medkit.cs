using RoR2;
using UnityEngine;
using MonoMod.Cil;

namespace UltimateCustomRun
{
    public class Medkit : ItemBase
    {
        public static float flatheal;
        public static float percentheal;
        public static bool stackbuff;
        public static bool isdebuff;

        public override string Name => ":: Items : Whites :: Medkit";
        public override string InternalPickupToken => "medkit";
        public override bool NewPickup => false;

        public override string PickupText => "";

        public override string DescText => "2 seconds after getting hurt, <style=cIsHealing>heal</style> for <style=cIsHealing>" + flatheal + "</style> plus an additional <style=cIsHealing>" + d(percentheal) + " <style=cStack>(+" + d(percentheal) + " per stack)</style></style> of <style=cIsHealing>maximum health</style>.";
        public override void Init()
        {
            flatheal = ConfigOption(20f, "Flat Healing", "Vanilla is 20");
            percentheal = ConfigOption(0.05f, "Percent Healing", "Decimal. Per Stack. Vanilla is 0.05");
            stackbuff = ConfigOption(false, "Stack Buff?", "Vanilla is false");
            isdebuff = ConfigOption(false, "Change to Debuff?", "Vanilla is false");
            base.Init();
        }

        public override void Hooks()
        {
            IL.RoR2.CharacterBody.RemoveBuff_BuffIndex += ChangeFlatHealing;
            IL.RoR2.CharacterBody.RemoveBuff_BuffIndex += ChangePercentHealing;
        }
        public static void ChangeFlatHealing(ILContext il)
        {
            ILCursor c = new ILCursor(il);

            c.GotoNext(MoveType.Before,
                x => x.MatchLdsfld(typeof(RoR2Content.Items), "Medkit"),
                x => x.MatchCallOrCallvirt<Inventory>("GetItemCount"),
                x => x.MatchStloc(0),
                x => x.MatchLdcR4(20f)
            ); ;
            c.Index += 2;
            c.Next.Operand = flatheal;
        }
        public static void ChangePercentHealing(ILContext il)
        {
            ILCursor c = new ILCursor(il);

            c.GotoNext(MoveType.Before,
                x => x.MatchLdarg(0),
                x => x.MatchCallOrCallvirt(typeof(CharacterBody).GetMethod("get_maxHealth")),
                x => x.MatchLdcR4(0.05f)
            );
            c.Index += 2;
            c.Next.Operand = percentheal;
        }
        public static void ChangeBuffBehavior()
        {
            var mh = Resources.Load<BuffDef>("buffdefs/medkitheal");
            mh.canStack = stackbuff;
            mh.isDebuff = isdebuff;
        }
    }
}
