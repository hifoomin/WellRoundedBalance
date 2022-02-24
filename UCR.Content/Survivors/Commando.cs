using RoR2;
using System;

namespace UltimateCustomRun.Survivors
{
    internal class Commando : Base
    {
        // Did Commando first as an example
        public override String EntityName => "Commando";

        public override String EntityType => "Survivors";
        public override Tuple<System.Object[], CharacterBody> Entity => new Tuple<System.Object[], CharacterBody>(new System.Object[] { 110f, 1f, 0f, 7f, 80f, 15f, 12f, 1f, 1f, 0f, 1, 1.45f, 33f, 0.2f, 0f, 0f, 0f, 2.4f, 0f, 0f, 0f }, BodyCatalog.FindBodyPrefab("Commando").GetComponent<CharacterBody>());
        // Insert unique code here
    }
}