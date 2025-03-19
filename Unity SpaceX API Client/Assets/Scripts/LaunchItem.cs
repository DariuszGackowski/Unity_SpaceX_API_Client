using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using static SpaceXLaunchesBrowser.SpaceXDataManager;

namespace SpaceXLaunchesBrowser
{
    public class LaunchItem : MonoBehaviour
    {
        public TextMeshProUGUI AddedNameText;
        public TextMeshProUGUI AddedPayloadsNumberText;
        public TextMeshProUGUI AddedRocketNameText;
        public TextMeshProUGUI AddedRocketCountryText;
        public GameObject TrueUpcomingImage;
        public GameObject FalseUpcomingImage;
        public Button LunchItemButton;

        public void Setup(Launch launch, UnityAction call)
        {
            SetLunchTexts(launch);
            SetUpcomingInfo(launch.Upcoming);

            LunchItemButton.onClick.AddListener(call);
        }
        private void SetUpcomingInfo(bool upcoming) 
        {
            TrueUpcomingImage.SetActive(false);
            FalseUpcomingImage.SetActive(false);
            if (upcoming)
                TrueUpcomingImage.SetActive(true);
            else
                FalseUpcomingImage.SetActive(true);
        }
        private void SetLunchTexts(Launch launch)
        {
            AddedNameText.SetText(launch.Name);
            AddedPayloadsNumberText.SetText(launch.Payloads.Count.ToString());
            AddedRocketNameText.SetText(launch.Rocket.Name);
            AddedRocketCountryText.SetText(launch.Rocket.Country);
        }
    }
}
