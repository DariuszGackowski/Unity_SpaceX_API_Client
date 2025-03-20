using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using static SpaceXLaunchesBrowser.SpaceXDataManager;

namespace SpaceXLaunchesBrowser
{
    public class ShipItem : MonoBehaviour
    {
        public TextMeshProUGUI AddedNameText;
        public TextMeshProUGUI AddedTypeText;
        public TextMeshProUGUI AddedHomePortText;
        public TextMeshProUGUI AddedMissionsNumberText;
        public Button ShipImageButton;
        [Space]
        public MaskableGraphic MaskableGraphic;
        public void Setup(Ship ship, UnityAction functionToAdd)
        {
            SetShipTexts(ship);

            ShipImageButton.onClick.AddListener(functionToAdd);

            MaskableGraphic.onCullStateChanged.AddListener(OnMaskStateChanged);
        }
        private void SetShipTexts(Ship ship)
        {
            AddedNameText.SetText(ship.Name);
            AddedTypeText.SetText(ship.Type);
            AddedHomePortText.SetText(ship.Home_port);
            AddedMissionsNumberText.SetText(ship.Launches.Count.ToString());
        }
        private void OnMaskStateChanged(bool isCulled)
        {
            if (isCulled)
            {
                Debug.Log($"{gameObject.name} zosta³ ukryty przez maskê!");

                //SpaceXLaunchesBrowserCanvasManager.OnShipItemBecameInvisible.Invoke(this);
            }
            else
            {
                Debug.Log($"{gameObject.name} jest widoczny!");
            }
        }
        private void OnDestroy()
        {
            MaskableGraphic.onCullStateChanged.RemoveListener(OnMaskStateChanged);
        }
    }
}