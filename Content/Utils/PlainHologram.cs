using TMPro;

namespace WellRoundedBalance.Utils
{
    public static class PlainHologram
    {
        private static GameObject _hologramContentPrefab;

        public static GameObject hologramContentPrefab // for use with IHologramContentProvider.GetHologramContentPrefab()
        {
            get
            {
                if (!_hologramContentPrefab)
                {
                    _hologramContentPrefab = PrefabAPI.InstantiateClone(Addressables.LoadAssetAsync<GameObject>("RoR2/Base/Common/VFX/CostHologramContent.prefab").WaitForCompletion(), "WRB Hologram", false);
                    var costHologramContent = hologramContentPrefab.GetComponent<CostHologramContent>();
                    var plainHologramContent = hologramContentPrefab.AddComponent<PlainHologramContent>();
                    plainHologramContent.targetTextMesh = costHologramContent.targetTextMesh;
                    Object.Destroy(costHologramContent);
                }
                return _hologramContentPrefab;
            }
        }

        public class PlainHologramContent : MonoBehaviour
        {
            public void FixedUpdate()
            {
                if (targetTextMesh)
                {
                    targetTextMesh.SetText(text);
                    targetTextMesh.color = color;
                }
            }

            public string text;
            public Color color = Color.white;
            public TextMeshPro targetTextMesh;
        }
    }
}