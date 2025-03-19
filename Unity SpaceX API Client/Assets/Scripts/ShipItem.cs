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

        public void Setup(Ship ship, UnityAction call)
        {
            SetShipTexts(ship);

            ShipImageButton.onClick.AddListener(call);
        }
        private void SetShipTexts(Ship ship)
        {
            AddedNameText.SetText(ship.Name);
            AddedTypeText.SetText(ship.Type);
            AddedHomePortText.SetText(ship.Home_port);
            AddedMissionsNumberText.SetText(ship.Launches.Count.ToString());
        }
    }
}
