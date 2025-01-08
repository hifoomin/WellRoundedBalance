using Mono.Cecil.Cil;
using MonoMod.Cil;

namespace WellRoundedBalance.Items.Greens
{
    public class OldGuillotine : ItemBase<OldGuillotine>
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
            IL.RoR2.HealthComponent.TakeDamageProcess += HealthComponent_TakeDamageProcess;
        }

        private void HealthComponent_TakeDamageProcess(ILContext il)
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
            var transform = guillotineVFX.transform;
            transform.localScale = new Vector3(1.5f, 1.5f, 1.5f);

            transform.GetChild(0).gameObject.SetActive(false);

            var hitspark = transform.GetChild(1).GetComponent<ParticleSystem>();
            var hitsparkMain = hitspark.main;
            hitsparkMain.duration = 1.5f;
            hitsparkMain.startLifetime = new ParticleSystem.MinMaxCurve(1.5f, 1.5f);

            var flash = transform.GetChild(2).GetComponent<ParticleSystem>();
            var flashMain = flash.main;
            flashMain.duration = 1.5f;
            flashMain.startLifetime = new ParticleSystem.MinMaxCurve(0.2f, 0.2f);

            var dash = transform.GetChild(3).GetComponent<ParticleSystem>();
            var dashMain = dash.main;
            dashMain.duration = 1.5f;
            dashMain.startLifetime = new ParticleSystem.MinMaxCurve(1.5f, 1.5f);

            transform.GetChild(4).gameObject.SetActive(false);

            var slash = transform.GetChild(5).GetComponent<ParticleSystem>();
            var slashMain = slash.main;
            slashMain.duration = 1.5f;
            slashMain.startLifetime = new ParticleSystem.MinMaxCurve(1.5f, 1.5f);

            var hitspark2 = transform.GetChild(6).GetComponent<ParticleSystem>();
            var hitspark2Main = hitspark2.main;
            hitspark2Main.startLifetime = new ParticleSystem.MinMaxCurve(0.35f, 0.35f);
            hitspark2Main.startSpeed = new ParticleSystem.MinMaxCurve(6f, 6f);
            ContentAddition.AddEffect(guillotineVFX);

            var hitspark3 = hitspark2.transform.GetChild(0).GetComponent<ParticleSystem>();
        }
    }
}