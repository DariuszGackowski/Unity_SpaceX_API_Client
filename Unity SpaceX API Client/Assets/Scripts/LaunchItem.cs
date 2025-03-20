using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using static SpaceXLaunchesBrowser.SpaceXDataManager;

namespace SpaceXLaunchesBrowser
{
    public class LaunchItem : MaskableGraphic
    {
        public TextMeshProUGUI AddedNameText;
        public TextMeshProUGUI AddedPayloadsNumberText;
        public TextMeshProUGUI AddedRocketNameText;
        public TextMeshProUGUI AddedRocketCountryText;
        public GameObject TrueUpcomingImage;
        public GameObject FalseUpcomingImage;
        public Button LunchItemButton;

        public bool sPopped;
        [Space]
        public RectTransform RectTransform;

        private ScrollRect _scrollRect;
        private RectTransform _maskRectTransform;
        private float _previousScrollPosition;
        private float _scrollDirection;

        private readonly Vector3[] _corners = new Vector3[4];
        private readonly Vector3[] _maskcorners = new Vector3[4];
        protected Rect Rect
        {
            get
            {
                RectTransform.GetWorldCorners(_corners);

                Vector2 min = _corners[0];
                Vector2 max = _corners[0];

                for (int i = 1; i < 4; i++)
                {
                    min.x = Mathf.Min(_corners[i].x, min.x);
                    min.y = Mathf.Min(_corners[i].y, min.y);
                    max.x = Mathf.Max(_corners[i].x, max.x);
                    max.y = Mathf.Max(_corners[i].y, max.y);
                }

                return new Rect(min, max - min);
            }
        }
        protected Rect MaskRect
        {
            get
            {
                _maskRectTransform.GetWorldCorners(_maskcorners);

                Vector2 min = _maskcorners[0];
                Vector2 max = _maskcorners[0];

                for (int i = 1; i < 4; i++)
                {
                    min.x = Mathf.Min(_maskcorners[i].x, min.x);
                    min.y = Mathf.Min(_maskcorners[i].y, min.y);
                    max.x = Mathf.Max(_maskcorners[i].x, max.x);
                    max.y = Mathf.Max(_maskcorners[i].y, max.y);
                }

                return new Rect(min, max - min);
            }
        }
        public void Setup(Launch launch, UnityAction call, RectTransform rectTransform, ScrollRect scrollRect)
        {
            SetLunchTexts(launch);
            SetUpcomingInfo(launch.Upcoming);

            _maskRectTransform = rectTransform;
            _scrollRect = scrollRect;
            _previousScrollPosition = _scrollRect.verticalNormalizedPosition;

            _scrollRect.onValueChanged.AddListener(delegate { Cull(Rect); CheckScrollDirection(); });
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
        private void Cull(Rect clipRect)
        {
            var cull = !clipRect.Overlaps(MaskRect);

            if (cull)
            {
                SpaceXLaunchesBrowserCanvasManager.OnLaunchItemBecameInvisible.Invoke(this, _scrollDirection);
            }
        }
        private void CheckScrollDirection()
        {
            float currentScrollPosition = _scrollRect.verticalNormalizedPosition;
            _scrollDirection =  currentScrollPosition - _previousScrollPosition;
            _previousScrollPosition = currentScrollPosition;
        }
    }
}