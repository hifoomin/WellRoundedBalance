using System;
using RoR2.Items;
using RoR2BepInExPack.Utilities;
using R2API;
using R2API.Networking.Interfaces;
using RoR2.UI;
using TMPro;
using R2API.Networking;

namespace WellRoundedBalance.Mechanics.Interactables {
    public class ReworkedLunarCoins : MechanicBase<ReworkedLunarCoins>
    {
        public override string Name => ":: Mechanics ::::::::::::: Reworked Lunar Coins";
        [ConfigField("Starting Coin Count", 3)]
        public static int StartingCoinCount;
        [ConfigField("Monsoon Extra Starting Coins", 1)]
        public static int BonusStartingCoin;
        private static FixedConditionalWeakTable<NetworkUser, CoinBank> UserCoinBanks = new();

        public override void Hooks()
        {
            On.RoR2.CostTypeCatalog.Register += ChangeLunarCostType;
            On.RoR2.Run.Start += GiveStartingCoins;
            On.RoR2.NetworkUser.AwardLunarCoins += UseCustomCoins;
            On.RoR2.NetworkUser.DeductLunarCoins += UseCustomCoinsDeduct;
            On.RoR2.UI.HUD.Update += OverrideCoinText;
            On.RoR2.PurchaseInteraction.Start += ChangeCosts;

            NetworkingAPI.RegisterMessageType<CoinBankSync>();
        }

        private void ChangeCosts(On.RoR2.PurchaseInteraction.orig_Start orig, PurchaseInteraction self)
        {
            orig(self);
            if (self.displayNameToken == "BAZAAR_SEER_NAME") {
                self.cost = 1;
                self.Networkcost = 1;
            }
        }

        private void OverrideCoinText(On.RoR2.UI.HUD.orig_Update orig, HUD self)
        {
            orig(self);
            int coins = 0;
            if (self.localUserViewer != null && self.localUserViewer.currentNetworkUser) {
                CoinBank bank = UserCoinBanks.GetOrCreateValue(self.localUserViewer.currentNetworkUser);
                coins = bank.coins;
            }

            self.lunarCoinText.targetValue = coins;
        }

        private void UseCustomCoinsDeduct(On.RoR2.NetworkUser.orig_DeductLunarCoins orig, NetworkUser self, uint count)
        {
            CoinBank bank = UserCoinBanks.GetOrCreateValue(self);
            bank.coins -= (int)count;
            if (bank.coins < 0) {
                bank.coins = 0;
            }
            bank.Sync(self);
        }

        private void UseCustomCoins(On.RoR2.NetworkUser.orig_AwardLunarCoins orig, NetworkUser self, uint count)
        {
            CoinBank bank = UserCoinBanks.GetOrCreateValue(self);
            bank.coins += (int)count;
            bank.Sync(self);
        }

        private void GiveStartingCoins(On.RoR2.Run.orig_Start orig, Run self)
        {
            orig(self);

            if (NetworkServer.active) {
                DifficultyDef difficulty = DifficultyCatalog.GetDifficultyDef(self.selectedDifficulty);
                for (int i = 0; i < PlayerCharacterMasterController.instances.Count; i++) {
                    PlayerCharacterMasterController.instances[i].networkUser.DeductLunarCoins((uint)UserCoinBanks.GetOrCreateValue(PlayerCharacterMasterController.instances[i].networkUser).coins);
                    PlayerCharacterMasterController.instances[i].networkUser.AwardLunarCoins((uint)StartingCoinCount + (difficulty.countsAsHardMode ? (uint)BonusStartingCoin : 0));
                }
            }
        }

        private void ChangeLunarCostType(On.RoR2.CostTypeCatalog.orig_Register orig, CostTypeIndex costType, CostTypeDef costTypeDef)
        {
            if (costType == CostTypeIndex.LunarCoin) {
                costTypeDef.isAffordable = (CostTypeDef def, CostTypeDef.IsAffordableContext context) => {
                    NetworkUser user = Util.LookUpBodyNetworkUser(context.activator.gameObject);
                    if (user) {
                        CoinBank bank = UserCoinBanks.GetOrCreateValue(user);
                        bank.Sync(user);
                        return bank.coins >= context.cost;
                    }
                    return false;
                };

                costTypeDef.payCost = (CostTypeDef.PayCostContext context, CostTypeDef.PayCostResults results) => {
                    NetworkUser user = Util.LookUpBodyNetworkUser(context.activator.gameObject);
                    if (user)
                    {
                        CoinBank bank = UserCoinBanks.GetOrCreateValue(user);
                        bank.coins -= context.cost;
                        if (bank.coins < 0) {
                            bank.coins = 0;
                        }
                        bank.Sync(user);
                        MultiShopCardUtils.OnNonMoneyPurchase(context);
                    }
                };
            }

            orig(costType, costTypeDef);
        }

        public class CoinBank {
            public NetworkUser owner;
            public int coins;

            public void Sync(NetworkUser user) {
                this.owner = user;

                if (NetworkServer.active) {
                    new CoinBankSync(owner.gameObject, coins).Send(R2API.Networking.NetworkDestination.Clients);
                }
                else {
                    new CoinBankSync(owner.gameObject, coins).Send(R2API.Networking.NetworkDestination.Server);
                }
            }
        }

        public class CoinBankSync : INetMessage
        {
            public GameObject target;
            public int newCount;
            public void OnReceived()
            {
                NetworkUser user = target.GetComponent<NetworkUser>();

                if (user) {
                    CoinBank bank = UserCoinBanks.GetOrCreateValue(user);
                    bank.coins = newCount;
                }
            }

            public void Deserialize(NetworkReader reader)
            {
                target = reader.ReadGameObject();
                newCount = reader.ReadInt32();
            }

            public void Serialize(NetworkWriter writer)
            {
                writer.Write(target);
                writer.Write(newCount);
            }

            public CoinBankSync(GameObject target, int count) {
                this.target = target;
                this.newCount = count;
            }

            public CoinBankSync() { }
        }
    }
}