﻿using KonstructionUI;
using System;
using System.IO;
using UnityEngine;
using USITools;

namespace Konstruction
{
    [KSPScenario(
        ScenarioCreationOptions.AddToAllGames,
        GameScenes.FLIGHT)]
    public class KonstructionScenario : ScenarioModule
    {
        public GameObject KonstructorResourcePanelPrefab { get; private set; }
        public GameObject KonstructorWindowPrefab { get; private set; }
        public GameObject ResourceTransferWindowPrefab { get; private set; }
        public ServiceManager ServiceManager { get; private set; }

        public override void OnAwake()
        {
            base.OnAwake();

            var usiTools = USI_AddonServiceManager.Instance;
            if (usiTools != null)
            {
                ServiceManager = usiTools.ServiceManager;

                try
                {
                    // Setup dependency injection for Konstruction services
                    var serviceCollection = usiTools.ServiceCollection;
                    serviceCollection.AddSingletonService<KonstructionPersistance>();

                    // Setup UI prefabs
                    var filePath = Path.Combine(KSPUtil.ApplicationRootPath,
                        "GameData/UmbraSpaceIndustries/Konstruction/Assets/UI/Konstruction.prefabs");
                    var prefabs = AssetBundle.LoadFromFile(filePath);
                    KonstructorWindowPrefab = prefabs.LoadAsset<GameObject>("KonstructorWindow");
                    KonstructorResourcePanelPrefab = prefabs.LoadAsset<GameObject>("RequiredResourcePanel");
                    ResourceTransferWindowPrefab = prefabs.LoadAsset<GameObject>("ResourceTransferWindow");

                    // Register UI prefabs in window manager
                    var windowManager = ServiceManager.GetService<WindowManager>();
                    windowManager
                        .RegisterWindow<KonstructorWindow>(KonstructorWindowPrefab)
                        .RegisterPrefab<RequiredResourcePanel>(KonstructorResourcePanelPrefab)
                        .RegisterWindow<ResourceTransferWindow>(ResourceTransferWindowPrefab);
                }
                catch (ServiceAlreadyRegisteredException) { }
                catch (Exception ex)
                {
                    Debug.LogError("[KONSTRUCTION] KonstructionScenario: " + ex.Message);
                }
            }
        }

        public override void OnLoad(ConfigNode gameNode)
        {
            base.OnLoad(gameNode);

            var persister = ServiceManager.GetService<KonstructionPersistance>();
            persister.Load(gameNode);
        }

        public override void OnSave(ConfigNode gameNode)
        {
            base.OnSave(gameNode);

            var persister = ServiceManager.GetService<KonstructionPersistance>();
            persister.Save(gameNode);
        }
    }
}
