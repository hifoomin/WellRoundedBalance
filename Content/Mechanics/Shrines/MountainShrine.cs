using System;
using WellRoundedBalance.Items.Whites;
using WellRoundedBalance.Mechanic;

namespace WellRoundedBalance.Mechanics.Shrines
{
    internal class MountainShrine : MechanicBase
    {
        public static BuffDef mountainShrineBuff;
        public override string Name => ":: Mechanics :::::::::::: Mountain Shrine";

        public override void Init()
        {
            var warbanner = Utils.Paths.Texture2D.texBuffWarbannerIcon.Load<Texture2D>();

            mountainShrineBuff = ScriptableObject.CreateInstance<BuffDef>();
            mountainShrineBuff.isHidden = false;
            mountainShrineBuff.canStack = true;
            mountainShrineBuff.isDebuff = false;
            mountainShrineBuff.buffColor = new Color32(72, 180, 255, 255);
            mountainShrineBuff.iconSprite = Sprite.Create(warbanner, new Rect(0f, 0f, (float)warbanner.width, (float)warbanner.height), new Vector2(0f, 0f));

            ContentAddition.AddBuffDef(mountainShrineBuff);
            base.Init();
        }

        public override void Hooks()
        {
            On.RoR2.CombatDirector.OnEnable += CombatDirector_OnEnable;
            RecalculateStatsAPI.GetStatCoefficients += RecalculateStatsAPI_GetStatCoefficients;
            CharacterBody.onBodyStartGlobal += CharacterBody_onBodyStartGlobal;
        }

        private void CharacterBody_onBodyStartGlobal(CharacterBody body)
        {
            var teleporter = TeleporterInteraction.instance;
            if (teleporter != null)
            {
                for (int i = 0; i < teleporter.shrineBonusStacks; i++)
                {
                    body.AddBuff(mountainShrineBuff);
                }
            }
        }

        private void RecalculateStatsAPI_GetStatCoefficients(CharacterBody sender, RecalculateStatsAPI.StatHookEventArgs args)
        {
            args.attackSpeedMultAdd += 0.1f * sender.GetBuffCount(mountainShrineBuff);
            args.moveSpeedMultAdd += 0.1f * sender.GetBuffCount(mountainShrineBuff);
        }

        private void CombatDirector_OnEnable(On.RoR2.CombatDirector.orig_OnEnable orig, RoR2.CombatDirector self)
        {
            self.minRerollSpawnInterval /= 1.35f;
            self.maxRerollSpawnInterval /= 1.35f;
            self.creditMultiplier += 0.25f;
            self.eliteBias *= 0.9f;
            var teleporter = TeleporterInteraction.instance;
            if (teleporter != null)
            {
                for (int i = 0; i < teleporter.shrineBonusStacks; i++)
                {
                    self.creditMultiplier *= 1.05f;
                    self.expRewardCoefficient *= 0.94f;
                    self.goldRewardCoefficient *= 0.94f;
                }
            }
            orig(self);
        }
    }
}