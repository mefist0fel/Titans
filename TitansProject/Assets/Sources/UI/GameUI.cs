using Model;
using System;
using UnityEngine;
using UnityEngine.UI;
using View;
using System.Collections.Generic;

namespace UI {
    public sealed class GameUI : UILayer {
        [SerializeField]
        private Text statusText; // Set from editor
        [SerializeField]
        private GameObject modulesPanel; // Set from editor
        [SerializeField]
        private ModuleUIPanel[] modules = new ModuleUIPanel[12]; // Set from editor
        [SerializeField]
        private ModuleUIPanel buildTitanModule; // Set from editor
        [SerializeField]
        private ModuleUIPanel upgradeTitanModule; // Set from editor
        [SerializeField]
        private RectTransform BuildContextMenu; // Set from editor
        [SerializeField]
        private Button FullScreenHolder; // Set from editor

        // Rockets panel
        [SerializeField]
        private GameObject skillPanel; // Set from editor
        [SerializeField]
        private ModuleUIPanel OnBuildRocketButton; // Set from editor
        [SerializeField]
        private Button OnFireRocketButton; // Set from editor
        [SerializeField]
        private Button OnCancelRocketButton; // Set from editor
        [SerializeField]
        private Text RocketsCount; // Set from editor
        [SerializeField]
        private UIMarkerController markerControllerPrefab; // Set from editor
        [SerializeField]
        private UICancelTaskQueue cancelTaskButtons; // Set from editor


        private TitanView selectedTitan;
        private ModuleSlot selectedSlot;
        private List<UIMarkerController> markers = new List<UIMarkerController>();

        public enum ModuleType { }

        public void AddMarker(TitanView titan, PlanetView planet) {
            var marker = Instantiate(markerControllerPrefab, transform);
            marker.Init(titan, planet);
            markers.Add(marker);
        }

        public void RemoveMarker(TitanView titan) {
            for (int i = 0; i < markers.Count; i++) {
                if (markers[i].Titan == null || markers[i].Titan == titan) {
                    markers[i].Destroy();
                }
            }
        }

        private void Start() {
            HideContextMenu();
            SelectTitan(null);
        }

        float winTimer = 0;
        private void Update() {
            winTimer -= Time.deltaTime;
            if (winTimer < 0) {
                winTimer = 0.5f;
                //   if (Game.CheckWinConditions()) {
                //       UILayer.Show<WinUI>();
                //   }
                //   if (Game.CheckLoseConditions()) {
                //       UILayer.Show<LoseUI>();
                //   }
            }
            if (Input.GetKeyUp(KeyCode.Escape) && !UILayer.IsLayerShowed<PauseMenuUI>()) {
                UILayer.ShowModal<PauseMenuUI>();
            }
        }

        public override void UpdateState() {
            UpdateModules();
        }

        public void SelectTitan(TitanView titan = null) {
            // if (selectedTitan != null) {
            //     selectedTitan.UnSubscribe(UpdateTitanStatus);
            // }
            selectedTitan = titan;
            // if (selectedTitan != null) {
            //     selectedTitan.Subscribe(UpdateTitanStatus);
            // }
            UpdateTitanStatus();
        }

        public void UpdateTitanStatus() {
            if (selectedTitan == null || !selectedTitan.Titan.IsAlive) {
                selectedTitan = null;
                statusText.text = "";
                modulesPanel.SetActive(false);
                skillPanel.SetActive(false);
                HideContextMenu();
                return;
            }
            string status = "Energy: " + selectedTitan.Titan.ResourceUnits;
            statusText.text = status;
            // enemy
            if (selectedTitan.Titan.Faction.ID == 1 || !selectedTitan.Titan.IsAlive) {
                modulesPanel.SetActive(true);
                skillPanel.SetActive(false);
                HideContextMenu();
                UpdateModules();
                return;
            }
            modulesPanel.SetActive(true);
            UpdateModules();
            //  var maxRocket = selectedTitan.GetComponentMaxRocketsCount();
            //  skillPanel.SetActive(maxRocket > 0);
            //  if (maxRocket > 0) {
            //      OnFireRocketButton.gameObject.SetActive(!RocketAimView.IsActive());
            //      OnCancelRocketButton.gameObject.SetActive(RocketAimView.IsActive());
            //      var rocketCount = selectedTitan.GetComponentRocketsCount();
            //      RocketsCount.text = rocketCount + "/" + maxRocket;
            //      OnFireRocketButton.interactable = rocketCount > 0;
            //  }
        }

        private void UpdateModules() {
            for (int i = 0; i < modules.Length; i++) {
                if (modules[i] == null)
                    throw new NullReferenceException("Module slot dont set in interface");
                ModuleSlot slot = null;
                if (selectedTitan.Titan.ModuleSlots.Length > i)
                    slot = selectedTitan.Titan.ModuleSlots[i];
                modules[i].SetModule(slot);
            }
            //  for (int i = 0; i < modules.Length; i++) {
            //      bool needShow = false;
            //      if (selectedTitan.SlotLevel.Length > i)
            //          needShow = selectedTitan.SlotLevel[i] <= selectedTitan.Level;
            //      if (modules[i] != null) {
            //          modules[i].SetActive(needShow);
            //      }
            //      ITitanModule module = null;
            //      if (selectedTitan.Modules.Length > i)
            //          module = selectedTitan.Modules[i];
            //      if (modules[i] != null) {
            //          modules[i].SetModule(module);
            //      }
            //  }
            //  buildTitanModule.SetModule(selectedTitan.Modules[12]);
            //  upgradeTitanModule.SetModule(selectedTitan.Modules[13]);
            //  upgradeTitanModule.gameObject.SetActive(selectedTitan.Level < TitanViewOld.MaxLevel);
            //  OnBuildRocketButton.SetModule(selectedTitan.Modules[14]);
            LayoutRebuilder.ForceRebuildLayoutImmediate(modules[0].transform.parent.GetComponent<RectTransform>());
        }

        public void ShowTaskCancelButtons(List<Titan.Task> taskPoints) {
            cancelTaskButtons.Show(taskPoints, selectedTitan.Titan);
        }

        public void OnSelectNextTitanClick() {
            Game.SelectNextTitan();
        }

        public void OnBuildModuleClick(int moduleId) { // Set from editor
            if (selectedTitan == null)
                return;
            if (selectedTitan.Titan.Faction.ID == 1)
                return;
            if (selectedTitan.Titan.ModuleSlots.Length <= moduleId || selectedTitan.Titan.ModuleSlots[moduleId] == null)
                return;
            selectedSlot = selectedTitan.Titan.ModuleSlots[moduleId];
            var selectModule = UILayer.ShowModal<SelectModuleUI>();
            selectModule.Init(modules[moduleId].RectTransform.anchoredPosition + new Vector2(0, 800), selectedSlot);
            // var module = new ModuleData("laser", 5, 2f);
            // selectedSlot.Attach(Model.ModulesFactory.CreateBuildModule(module, selectedSlot));
            // UpdateModules();

            // FullScreenHolder.gameObject.SetActive(true);
            // BuildContextMenu.gameObject.SetActive(true);
            // BuildContextMenu.position = modules[moduleId].transform.position;
        }

        public void OnBuildTitanClick() { // Set from editor

            //  if (selectedTitan.FactionId == 1)
            //      return;
            //  if (selectedTitan.Modules[12] != null)
            //      return;
            //  selectedTitan.BuildTitan(Config.Modules["titan"]);
        }

        public void OnUpgradeTitanClick() { // Set from editor

            //  if (selectedTitan.FactionId == 1)
            //      return;
            //  if (selectedTitan.Modules[13] != null)
            //      return;
            //  selectedTitan.BuildUpgrade(Config.Modules["titan_upgrade"]);
        }

        public void OnSelectBuildWeaponModuleClick() { // Set from editor
           // var module = new ModuleData("weapon", 5, 2f);
            //selectedSlot.Attach(Model.ModulesFactory.CreateBuildModule(module, selectedSlot));
            //   var module = Config.Modules["weapon"];
            //   selectedTitan.BuildModule(module, selectedSlot);
            //   HideContextMenu();
        }

        public void OnSelectBuildRocketModuleClick() { // Set from editor


            //  var module = Config.Modules["rocket"];
            //  selectedTitan.BuildModule(module, selectedSlot);
            //  HideContextMenu();
        }

        public void OnSelectBuildShieldModuleClick() { // Set from editor

            //  var module = Config.Modules["shield"];
            //  selectedTitan.BuildModule(module, selectedSlot);
            //  HideContextMenu();
        }

        public void OnSelectBuildAntiAirModuleClick() { // Set from editor

            //  var module = Config.Modules["anti_air"];
            //  selectedTitan.BuildModule(module, selectedSlot);
            //  HideContextMenu();
        }

        public void OnSelectRocketFireButton() {
            Game.OnSelectRocketStrike();
            OnFireRocketButton.gameObject.SetActive(false);
            OnCancelRocketButton.gameObject.SetActive(true);
        }
        public void OnCancelRocketFireButton() {
            RocketAimView.Hide();
            OnFireRocketButton.gameObject.SetActive(true);
            OnCancelRocketButton.gameObject.SetActive(false);
        }
        public void OnAddRocketButton() {
            // if (selectedTitan.Modules[14] != null)
            //     return;
            // var rocketCount = selectedTitan.GetComponentRocketsCount();
            // var maxRocketCount = selectedTitan.GetComponentMaxRocketsCount();
            // if (rocketCount >= maxRocketCount)
            //     return;
            // selectedTitan.BuildRocket(Config.Modules["add_rocket"]);
        }

        public void CancelContextMenu() { // Set from editor
            HideContextMenu();
        }

        private void HideContextMenu() {
            FullScreenHolder.gameObject.SetActive(false);
            BuildContextMenu.gameObject.SetActive(false);
        }
    }
}