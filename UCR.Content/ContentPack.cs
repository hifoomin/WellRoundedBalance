using System.Collections;
using RoR2.ContentManagement;

namespace UltimateCustomRun
{
    public class ContentPacks : IContentPackProvider
    {
        public ContentPack contentPack = new();
        public string identifier => "HIFU.UltimateCustomRun";

        public void Initialize()
        {
            ContentManager.collectContentPackProviders += ContentManager_collectContentPackProviders;
        }

        private void ContentManager_collectContentPackProviders(ContentManager.AddContentPackProviderDelegate addContentPackProvider)
        {
            addContentPackProvider(this);
        }

        public IEnumerator LoadStaticContentAsync(LoadStaticContentAsyncArgs args)
        {
            contentPack.identifier = identifier;
            contentPack.projectilePrefabs.Add(Main.projectilePrefabContent.ToArray());
            contentPack.entityStateTypes.Add(Main.entityStateContent.ToArray());
            contentPack.skillDefs.Add(Main.skillDefContent.ToArray());
            args.ReportProgress(1f);
            yield break;
        }

        public IEnumerator GenerateContentPackAsync(GetContentPackAsyncArgs args)
        {
            ContentPack.Copy(contentPack, args.output);
            args.ReportProgress(1f);
            yield break;
        }

        public IEnumerator FinalizeAsync(FinalizeAsyncArgs args)
        {
            args.ReportProgress(1f);
            yield break;
        }
    }
}