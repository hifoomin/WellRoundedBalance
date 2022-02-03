using MonoMod.Cil;
using R2API;
using RoR2;

namespace UltimateCustomRun
{
    public class BerzerkersPauldron : Based
    {
        public static float armor;
        public static float buffarmor;
        public static float basebuffdur;
        public static float stackbuffdur;
        public static float killreq;

        public override string Name => ":: Items :: Greens :: Berzerkers Pauldron";
        public override string InternalPickupToken => "warCryOnMultiKill";
        public override bool NewPickup => false;
        // remember to change when killreq works
        public override string PickupText => "";
        // "Enter a frenzy after killing " + killreq + " enemies in quick succession.");

        bool bArmor = armor != 0f;
        bool bBuffArmor = buffarmor != 0f;
        float bFullDur = basebuffdur + stackbuffdur;
       
        public override string DescText => (bArmor ? "<style=cIsHealing>Increase armor</style> by <style=cIsHealing>" + armor + "</style> <style=cStack>(+" + armor + " per stack)</style>. " : "") +
                                           "<style=cIsDamage>Killing " + "" /* change killcount */ + " 4 enemies</style> within <style=cIsDamage>1</style> second sends you into a <style=cIsDamage>frenzy</style> for <style=cIsDamage>" + bFullDur + "s</style> <style=cStack>(+" + stackbuffdur + "s per stack)</style>. Increases <style=cIsUtility>movement speed</style> by <style=cIsUtility>50%</style>, <style=cIsDamage>attack speed</style> by <style=cIsDamage>100%</style>" +
                                           (bBuffArmor ? " and <style=cIsHealing>armor</style> by <style=cIsHealing>" + buffarmor + "</style>." : "");


        public override void Init()
        {
            armor = ConfigOption(0f, "Unconditional Armor", "Per Stack. Vanilla is 0");
            buffarmor = ConfigOption(0f, "Buff Armor", "Vanilla is 0");
            basebuffdur = ConfigOption(6f, "Base Buff Duration", "Vanilla is 6");
            stackbuffdur = ConfigOption(4f, "Stack Buff Duration", "Per Stack. Vanilla is 4");
            base.Init();
        }

        public override void Hooks()
        {
            // IL.RoR2.CharacterBody.AddMultiKill += ChangeKillCount;
            IL.RoR2.CharacterBody.AddMultiKill += ChangeBuffDuration;
            RecalculateStatsAPI.GetStatCoefficients += AddBehavior;
        }
        public static void ChangeKillCount(ILContext il)
        {
            ILCursor c = new ILCursor(il);

            c.GotoNext(MoveType.Before,
                x => x.MatchLdarg(0),
                x => x.MatchCallOrCallvirt<RoR2.CharacterBody>("get_multiKillCount"),
                x => x.MatchLdcI4(4)
            );
            c.Index += 2;
            c.Next.Operand = killreq;
            // WHY DOES THIS NOT WORK
        }
        public static void ChangeBuffDuration(ILContext il)
        {
            ILCursor c = new ILCursor(il);

            c.GotoNext(MoveType.Before,
                x => x.MatchLdsfld("RoR2.RoR2Content/Buffs", "WarCryBuff"),
                x => x.MatchLdcR4(2f),
                x => x.MatchLdcR4(4f)
            );
            c.Index += 1;
            c.Next.Operand = basebuffdur - stackbuffdur;
            c.Index += 1;
            c.Next.Operand = stackbuffdur;
        }
        public static void AddBehavior(CharacterBody sender, RecalculateStatsAPI.StatHookEventArgs args)
        {
            if (sender.inventory)
            {
                var buff = sender.HasBuff(RoR2Content.Buffs.WarCryBuff);
                var stack = sender.inventory.GetItemCount(RoR2Content.Items.WarCryOnMultiKill);
                if (stack > 0 && buff)
                {
                    args.armorAdd += buffarmor;
                }
                args.armorAdd += armor;
            }
        }
        // TODO: Ask Moffein for his Pauldron changes and implement them :plead
    }
}
