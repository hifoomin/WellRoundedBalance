using MonoMod.Cil;

namespace WellRoundedBalance.Items.Greens
{
    public class WillOTheWisp : ItemBase<WillOTheWisp>
    {
        public override string Name => ":: Items :: Greens :: Will O The Wisp";
        public override ItemDef InternalPickup => RoR2Content.Items.ExplodeOnDeath;

        public override string PickupText => "Detonate enemies on kill.";

        public override string DescText => "On killing an enemy, spawn a <style=cIsDamage>lava pillar</style> in a <style=cIsDamage>" + baseRange + "m</style>" +
                                            (rangePerStack > 0 ? " <style=cStack>(+" + rangePerStack + "m per stack)</style>" : "") +
                                            " radius for <style=cIsDamage>" + d(baseDamage) + "</style> <style=cStack>(+" + d(damagePerStack) + " per stack)</style> base damage.";

        [ConfigField("Base Damage", "Decimal.", 2.4f)]
        public static float baseDamage;

        [ConfigField("Damage Per Stack", "Decimal.", 1.2f)]
        public static float damagePerStack;

        [ConfigField("Base Range", 12f)]
        public static float baseRange;

        [ConfigField("Range Per Stack", 0f)]
        public static float rangePerStack;

        [ConfigField("Proc Chance", 0f)]
        public static float procChance;

        public override void Init()
        {
            base.Init();
        }

        public override void Hooks()
        {
            IL.RoR2.GlobalEventManager.OnCharacterDeath += Changes;
            ChangeProc();
        }

        public static void Changes(ILContext il)
        {
            ILCursor c = new(il);

            if (c.TryGotoNext(MoveType.Before,
                    x => x.MatchLdcR4(3.5f),
                    x => x.MatchLdcR4(1f),
                    x => x.MatchLdloc(out _),
                    x => x.MatchLdcI4(1),
                    x => x.MatchSub(),
                    x => x.MatchConvR4(),
                    x => x.MatchLdcR4(0.8f)))
            {
                c.Next.Operand = baseDamage;
                c.Index += 6;
                c.Next.Operand = damagePerStack / baseDamage;
            }
            else
            {
                Logger.LogError("Failed to apply Will o' The Wisp Damage hook");
            }

            c.Index = 0;

            if (c.TryGotoNext(MoveType.Before,
                x => x.MatchLdcR4(12f),
                x => x.MatchLdcR4(2.4f)))
            {
                c.Next.Operand = baseRange;
                c.Index += 1;
                c.Next.Operand = rangePerStack;
            }
            else
            {
                Logger.LogError("Failed to apply Will o' The Wisp Range hook");
            }
        }

        public static void ChangeProc()
        {
            var w = LegacyResourcesAPI.Load<GameObject>("prefabs/networkedobjects/WilloWispDelay").GetComponent<DelayBlast>();
            w.procCoefficient = procChance * globalProc;
        }
    }
}