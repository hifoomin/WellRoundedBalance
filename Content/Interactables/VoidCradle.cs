namespace WellRoundedBalance.Interactables
{
    internal class VoidCradle : InteractableBase
    {
        public override string Name => ":: Interactables : Void Cradle";

        public override void Init()
        {
            base.Init();
        }

        public override void Hooks()
        {
            GameObject VoidCradle = Utils.Paths.GameObject.VoidChest.Load<GameObject>();
            PurchaseInteraction interaction = VoidCradle.GetComponent<PurchaseInteraction>();
            interaction.costType = CostTypeIndex.None;
            interaction.cost = 0;
            interaction.contextToken = "WRB_VOIDCHEST_CONTEXT";
            LanguageAPI.Add("WRB_VOIDCHEST_CONTEXT", "Open? (<style=cDeath>10% Curse</style>)");
            VoidCradle.AddComponent<CradleManager>();
        }

        private class CradleManager : MonoBehaviour
        {
            public PurchaseInteraction interaction => GetComponent<PurchaseInteraction>();

            private void Start()
            {
                interaction.onPurchase.AddListener(OnPurchase);

                if (TeleporterInteraction.instance)
                {
                    TeleporterInteraction.onTeleporterBeginChargingGlobal += Explode;
                }
            }

            public void OnPurchase(Interactor interactor)
            {
                if (interactor.GetComponent<CharacterBody>())
                {
                    CharacterBody body = interactor.GetComponent<CharacterBody>();
                    float amount = body.healthComponent.fullCombinedHealth * 0.1f;
                    float curse = Mathf.RoundToInt(amount / body.healthComponent.fullCombinedHealth * 100f);
                    for (int i = 0; i < curse; i++)
                    {
                        body.AddBuff(RoR2Content.Buffs.PermanentCurse);
                    }
                }
            }

            public void Explode(TeleporterInteraction instance)
            {
                if (TeleporterInteraction.instance)
                {
                    TeleporterInteraction.onTeleporterBeginChargingGlobal -= Explode;
                }

                EffectManager.SpawnEffect(Utils.Paths.GameObject.ExplodeOnDeathVoidExplosionEffect.Load<GameObject>(), new EffectData
                {
                    origin = transform.position,
                    scale = 3f
                }, true);
                Destroy(gameObject);
            }
        }
    }
}