using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using static SpaceXLaunchesBrowser.SpaceXDataManager;

namespace SpaceXLaunchesBrowser
{
    public class LaunchItem : MonoBehaviour
    {
        [Space]
        public int Identifier;
        public bool ActiveSelf => gameObject.activeSelf;

        [Space]
        public TextMeshProUGUI AddedNameText;
        public TextMeshProUGUI AddedPayloadsNumberText;
        public TextMeshProUGUI AddedRocketNameText;
        public TextMeshProUGUI AddedRocketCountryText;
        public GameObject TrueUpcomingImage;
        public GameObject FalseUpcomingImage;
        public Button LunchItemButton;

        [Space]
        public RectTransform RectTransform;

        public void Setup(int identifier, Launch launch, UnityAction functionToAdd)
        {
            Identifier = identifier;

            SetLunchTexts(launch);
            SetUpcomingInfo(launch.Upcoming);

            LunchItemButton.onClick.AddListener(functionToAdd);
        }
        public void SetActive()
        {
            gameObject.SetActive(true);
        }
        public void SetInactive()
        {
            gameObject.SetActive(false);
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