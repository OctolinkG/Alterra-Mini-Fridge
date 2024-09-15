using Nautilus.Assets;
using Nautilus.Assets.Gadgets;
using Nautilus.Crafting;
using Nautilus.Extensions;
using Nautilus.Utility;
using UnityEngine;

namespace AlterraMiniFridge.BZ.Items.Equipment
{
    public static class AlterraMiniFridge
    {
        public const string ClassId = "AlterraMiniFridge";
        public const string DisplayName = "Alterra Mini Fridge";
        public const string Description = "Powerful mini fridge o store your food and water supplies.";

        public static PrefabInfo PrefabInfo { get; } = PrefabInfo
            .WithTechType(classId: ClassId, displayName: DisplayName, description: Description, unlockAtStart: true)
            .WithIcon(ImageUtils.LoadSpriteFromTexture(Plugin.AssetBundle.LoadAsset<Texture2D>($"{ClassId}.png")));

        public static void Register()
        {
            var customPrefab = new CustomPrefab(PrefabInfo);
            customPrefab.SetGameObject(GetAssetBundlePrefab);

            var recipe = new RecipeData()
            {
                craftAmount = 1,
                Ingredients =
                {
                    new Ingredient(TechType.Titanium, 1),
                    //new Ingredient(TechType.Titanium, 4),
                    //new Ingredient(TechType.WiringKit, 1),
                },
            };

            customPrefab.SetRecipe(recipe)
                .WithFabricatorType(CraftTree.Type.Constructor)
                .WithStepsToFabricatorTab("Interior Modules");

            customPrefab.SetEquipment(EquipmentType.Hand);
            customPrefab.SetPdaGroupCategory(TechGroup.InteriorModules, TechCategory.InteriorModule);

            customPrefab.Register();
        }

        private static GameObject GetAssetBundlePrefab()
        {
            var prefab = Plugin.AssetBundle.LoadAsset<GameObject>($"{PrefabInfo.ClassID}.prefab");
            PrefabUtils.AddBasicComponents(prefab, PrefabInfo.ClassID, PrefabInfo.TechType, LargeWorldEntity.CellLevel.Medium);
            MaterialUtils.ApplySNShaders(prefab);

            SetupConstructable(prefab);

            return prefab;
        }

        private static void SetupConstructable(GameObject prefab)
        {
            var rootModel = prefab.SearchChild("model");
            var constructable = PrefabUtils.AddConstructable(prefab, PrefabInfo.TechType, ConstructableFlags.Inside, rootModel);
            constructable.allowedOnConstructables = true;
            constructable.allowedOnGround = true;
            constructable.allowedOnWall = false;
            constructable.allowedOutside = false;
            constructable.allowedInSub = true;
            constructable.deconstructionAllowed = true;
            constructable.forceUpright = true;
            constructable.rotationEnabled = true;
        }
    }
}