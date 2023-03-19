using Mono.Cecil.Cil;
using MonoMod.Cil;

namespace WellRoundedBalance.Items.Greens
{
    public class OldGuillotine : ItemBase
    {
        public static GameObject guillotineVFX;
        public override string Name => ":: Items :: Greens :: Old Guillotine";
        public override ItemDef InternalPickup => RoR2Content.Items.ExecuteLowHealthElite;

        public override string PickupText => "Instantly kill low health Elite monsters.";

        public override string DescText => "Instantly kill Elite monsters below <style=cIsHealth>" + healthThreshold + "% <style=cStack>(+" + healthThreshold + "% per stack)</style> health</style>.";

        [ConfigField("Health Threshold", 25f)]
        public static float healthThreshold;

        public override void Init()
        {
            base.Init();
        }

        public override void Hooks()
        {
            IL.RoR2.CharacterBody.OnInventoryChanged += ChangeThreshold;
            Changes();
            IL.RoR2.HealthComponent.TakeDamage += HealthComponent_TakeDamage;
        }

        private void HealthComponent_TakeDamage(ILContext il)
        {
            ILCursor c = new(il);

            if (c.TryGotoNext(MoveType.Before,
                x => x.MatchLdsfld(typeof(HealthComponent.AssetReferences), "executeEffectPrefab")))
            {
                c.Remove();
                c.Emit<OldGuillotine>(OpCodes.Ldsfld, nameof(guillotineVFX));
            }
            else
            {
                Main.WRBLogger.LogError("Failed to apply Old Guillotine VFX hook");
            }
        }

        public static void ChangeThreshold(ILContext il)
        {
            ILCursor c = new(il);

            if (c.TryGotoNext(MoveType.Before,
                    x => x.MatchLdcR4(13f)))
            {
                c.Next.Operand = healthThreshold;
            }
            else
            {
                Logger.LogError("Failed to apply Old Guillotine Threshold hook");
            }
        }

        private void Changes()
        {
            guillotineVFX = PrefabAPI.InstantiateClone(Utils.Paths.GameObject.OmniImpactExecute.Load<GameObject>(), "Old Guillotine Execution", false);
            guillotineVFX.transform.localScale = new Vector3(2f, 2f, 2f);

            ContentAddition.AddEffect(guillotineVFX);
        }
    }
}