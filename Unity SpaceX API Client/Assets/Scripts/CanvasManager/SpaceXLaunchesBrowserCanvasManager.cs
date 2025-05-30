using DG.Tweening;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using static Lobby.LobbyCanvasManager;
using static SpaceXLaunchesBrowser.SpaceXDataManager;
using System.Linq;

namespace SpaceXLaunchesBrowser
{
    public class SpaceXLaunchesBrowserCanvasManager : MonoBehaviour
    {
        public static UnityEvent OnStartLoading = new();
        public static UnityEvent OnEndLoading = new();
        public static UnityEvent OnGetLaunches = new();
        public static UnityEvent<Launch> OnGetShips = new();
        public static UnityEvent OnGetImage = new();
        public static List<LaunchItem> LaunchItems = new();

        public SpaceXDataManager SpaceXDataManager;
        public ShipImage ShipImage;
        public GameObject LoadingCircle;
        public GameObject ShipDetailsPanel;
        public GameObject ShipImagePanel;
        public GameObject LaunchItemPrefab;
        public GameObject ShipItemPrefab;

        public RectTransform ShipItemContent;
        public RectTransform LaunchItemContent;

        public ContentSizeFitter LaunchItemContentSizeFitter;

        public Button LaunchItemBackArrow;
        public Button ShipItemBackArrow;

        private void Start()
        {
            DOTween.Init();

            ResetCanvas();

            OnStartLoading.AddListener(ShowLoadingCircle);
            OnEndLoading.AddListener(HideLoadingCircle);
            OnGetLaunches.AddListener(SetupLaunches);

            OnGetShips.AddListener(launch =>
            {
                ResetShipItems();
                ShipDetailsPanel.SetActive(true);
                SetupShips(launch);
            });

            OnGetImage.AddListener(delegate { ShipImagePanel.SetActive(true); SetupImage(); });

            LaunchItemBackArrow.onClick.AddListener(delegate { ResetLaunchItems(); LoadLobbyScene(); });
            ShipItemBackArrow.onClick.AddListener(ResetShipItems);

            SpaceXDataManager.GetAllLaunches();
        }
        private void LoadLobbyScene()
        {
            SceneManager.LoadScene(GameScenes.Lobby.ToString());
        }
        private void ResetCanvas()
        {
            LoadingCircle.SetActive(false);
            ShipImagePanel.SetActive(false);
            ShipDetailsPanel.SetActive(false);
        }
        private void ResetLaunchItems()
        {
        }
        private void ResetShipItems()
        {
            ShipDetailsPanel.SetActive(false);
        }
        private void ShowLoadingCircle()
        {
            LoadingCircle.SetActive(true);
            LoadingCircle.transform.DORotate(new Vector3(0, 0, -360), 4f, RotateMode.LocalAxisAdd)
                .SetLoops(-1, LoopType.Incremental)
                .SetEase(Ease.Linear);
        }
        private void HideLoadingCircle()
        {
            LoadingCircle.transform.DOKill();
            LoadingCircle.SetActive(false);
        }
        private void SetupLaunches()
        {
            if (SpaceXDataManager.Launches == null || SpaceXDataManager.Launches.Count == 0) return;

            for (int i = 0; i < SpaceXDataManager.Launches.Count; i++)
            {
                LaunchItem launchItem = GetLaunchItem();
                if (launchItem == null) continue;

                LaunchItems.Add(launchItem);

                if (launchItem.Identifier <= 10)
                {
                    launchItem.SetActive();
                }
                else
                {
                    launchItem.SetInactive();
                }
                Launch launch = SpaceXDataManager.Launches[i];
                launchItem.Setup(i, launch, delegate { SpaceXDataManager.GetAllShips(launch); });
            }
            Canvas.ForceUpdateCanvases();

            LaunchItemContentSizeFitter.verticalFit = ContentSizeFitter.FitMode.Unconstrained;
        }
        private void SetupShips(Launch launch)
        {
            if (launch == null) return;

            foreach (Ship ship in launch.Ships)
            {
                ShipItem shipItem = GetShipItem();

                if (shipItem == null) continue;

                shipItem.Setup(ship, delegate { SpaceXDataManager.LoadImage(ship.Image); });
            }
        }
        private void SetupImage()
        {
            ShipImage.Setup(BackFromShipImage);
        }
        private void BackFromShipImage()
        {
            ShipImagePanel.SetActive(false);
        }
        private LaunchItem GetLaunchItem()
        {
            if (Instantiate(LaunchItemPrefab, LaunchItemContent).TryGetComponent(out LaunchItem launchItem))
                return launchItem;

            return null;
        }
        private ShipItem GetShipItem()
        {
            if (Instantiate(ShipItemPrefab, ShipItemContent).TryGetComponent(out ShipItem shipItem))
                return shipItem;

            return null;
        }
    }
}
