using MonoMod.Cil;

namespace WellRoundedBalance.Items.Reds
{
    public class NkuhanasOpinion : ItemBase<NkuhanasOpinion>
    {
        public override string Name => ":: Items ::: Reds :: Nkuhanas Opinion";
        public override ItemDef InternalPickup => RoR2Content.Items.NovaOnHeal;

        public override string PickupText => "Fire haunting skulls when healed.";

        public override string DescText => "Store <style=cIsHealing>100%</style> <style=cStack>(+100% per stack)</style> of healing as <style=cIsHealing>Soul Energy</style>. After your <style=cIsHealing>Soul Energy</style> reaches <style=cIsHealing>10%</style> of your <style=cIsHealing>maximum health</style>, <style=cIsDamage>fire a skull</style> that deals <style=cIsDamage>" + d(baseDamage) + "</style> of your <style=cIsHealing>Soul Energy</style> as <style=cIsDamage>damage</style>.";

        [ConfigField("Base Damage", "Decimal.", 3.5f)]
        public static float baseDamage;

        [ConfigField("Base Range", 25f)]
        public static float baseRange;

        [ConfigField("Proc Coefficient", 0.33f)]
        public static float procCoefficient;

        public override void Init()
        {
            base.Init();
        }

        public override void Hooks()
        {
            IL.RoR2.HealthComponent.ServerFixedUpdate += HealthComponent_ServerFixedUpdate;
            On.RoR2.Orbs.DevilOrb.Begin += DevilOrb_Begin;
        }

        private void DevilOrb_Begin(On.RoR2.Orbs.DevilOrb.orig_Begin orig, RoR2.Orbs.DevilOrb self)
        {
            self.procCoefficient = procCoefficient * globalProc;
            orig(self);
        }

        private void HealthComponent_ServerFixedUpdate(ILContext il)
        {
            ILCursor c = new(il);
            if (c.TryGotoNext(MoveType.Before,
                x => x.MatchLdcR4(2.5f)))
            {
                c.Next.Operand = baseDamage;
            }
            else
            {
                Logger.LogError("Failed to apply Nkuhanas Opinion Damage hook");
            }

            c.Index = 0;

            if (c.TryGotoNext(MoveType.Before,
                x => x.MatchLdcR4(40f)))
            {
                c.Next.Operand = baseRange;
            }
            else
            {
                Logger.LogError("Failed to apply Nkuhanas Opinion Range hook");
            }
        }
    }
}