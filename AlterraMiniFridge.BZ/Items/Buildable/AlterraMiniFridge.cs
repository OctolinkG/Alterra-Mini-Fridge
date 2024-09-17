using AlterraMiniFridge.Runtime;
using Nautilus.Assets;
using Nautilus.Assets.Gadgets;
using Nautilus.Crafting;
using Nautilus.Extensions;
using Nautilus.Utility;
using System;
using System.ComponentModel;
using UnityEngine;

#if SN
using static CraftData;
#endif

namespace AlterraMiniFridge.Items.Equipment
{
    public static class AlterraMiniFridge
    {
        public const string ClassId = "AlterraMiniFridge";
        public const string DisplayName = "Alterra Mini Fridge";
        public const string Description = "Are you a fan of cold fish? Cold drinks? The all-new Alterra Mini Fridge has you covered!";

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
                    new Ingredient(TechType.Titanium, 2),
                    new Ingredient(TechType.Glass, 1),
#if SN
                    new Ingredient(TechType.BluePalmSeed, 1),
#elif BZ
                    new Ingredient(TechType.IceFruit, 1),
#endif
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
            SetupStorage(prefab);
            SetupExtraScript(prefab);

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

        private static void SetupStorage(GameObject prefab)
        {
            var wasActive = prefab.activeSelf;

            if (wasActive) prefab.SetActive(false);

            var storageRoot = prefab.FindChild("model");

            var childObjectIdentifier = storageRoot.AddComponent<ChildObjectIdentifier>();
            childObjectIdentifier.ClassId = $"{PrefabInfo.ClassID}Container";

            var container = prefab.AddComponent<StorageContainer>();
            container.prefabRoot = prefab;
            container.width = 4;
            container.height = 6;
            container.storageRoot = childObjectIdentifier;
            container.preventDeconstructionIfNotEmpty = true;
            container.hoverText = $"Open Mini Fridge";
            prefab.AddComponent<AlterraMiniFridgeContainer>().CopyComponent(container);

            UnityEngine.Object.DestroyImmediate(container);

            if (wasActive) prefab.SetActive(true);
        }

        private static void SetupExtraScript(GameObject prefab)
        {
            var fridgeController = prefab.AddComponent<AlterraMiniFridgeController>();
        }

        private static void SetupMaterials(GameObject prefab)
        {
            try
            {
                foreach (Renderer renderer in prefab.GetComponentsInChildren<Renderer>())
                {
                    foreach (Material material in renderer.materials)
                    {
                        if (material == null) continue;


                        material.EnableKeyword("MARMO_EMISSION");
                        //material.SetColor(ShaderPropertyID._GlowColor, LightColor);
                        material.SetFloat(ShaderPropertyID._GlowStrength, 3.0f);
                        material.SetFloat(ShaderPropertyID._GlowStrengthNight, 5.0f);
                    }
                }
            }
            catch (Exception ex)
            {
                Plugin.Logger.LogError($"Error updating materials: {ex}");
            }
        }
    }
}