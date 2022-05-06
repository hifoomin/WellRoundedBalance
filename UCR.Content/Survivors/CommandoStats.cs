using RoR2;
using System;

namespace UltimateCustomRun.Survivors
{
    internal class CommandoStats : Base
    {
        // Did Commando first as an example
        public override string EntityName => "Commando";

        public override string EntityType => "Survivors";
        public override Tuple<object[], CharacterBody> Entity => new Tuple<object[], CharacterBody>(new object[] { 110f, 1f, 0f, 7f, 80f, 15f, 12f, 1f, 1f, 0f, 1, 1.45f, 33f, 0.2f, 0f, 0f, 0f, 2.4f, 0f, 0f, 0f }, BodyCatalog.FindBodyPrefab("Commando").GetComponent<CharacterBody>());
        // Insert unique code here
    }
}