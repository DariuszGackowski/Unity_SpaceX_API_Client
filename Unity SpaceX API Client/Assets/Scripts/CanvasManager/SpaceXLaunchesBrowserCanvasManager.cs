using DG.Tweening;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using static Lobby.LobbyCanvasManager;
using static SpaceXLaunchesBrowser.SpaceXDataManager;

namespace SpaceXLaunchesBrowser
{
    public class SpaceXLaunchesBrowserCanvasManager : MonoBehaviour
    {
        public static UnityEvent OnStartLoading = new();
        public static UnityEvent OnEndLoading = new();
        public static UnityEvent OnGetLaunches = new();
        public static UnityEvent<Launch> OnGetShips = new();
        public static UnityEvent OnGetImage = new();

        public SpaceXDataManager SpaceXDataManager;
        public ShipImage ShipImage;
        public GameObject LoadingCircle;
        public GameObject ShipDetailsPanel;
        public GameObject ShipImagePanel;
        public GameObject LaunchItemPrefab;
        public GameObject ShipItemPrefab;
        public Transform LaunchItemContent;
        public Transform ShipItemContent;

        public Button LaunchItemBackArrow;
        public Button ShipItemBackArrow;

        private Queue<GameObject> _launchItemPool = new();
        private Queue<GameObject> _shipItemPool = new();

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
            for (int i = 1; i < LaunchItemContent.childCount; i++)
            {
                Transform child = LaunchItemContent.GetChild(i);
                child.gameObject.SetActive(false);
                _launchItemPool.Enqueue(child.gameObject);
            }
        }
        private void ResetShipItems()
        {
            ShipDetailsPanel.SetActive(false);

            for (int i = 1; i < ShipItemContent.childCount; i++)
            {
                Transform child = ShipItemContent.GetChild(i);
                child.gameObject.SetActive(false);
                _shipItemPool.Enqueue(child.gameObject);
            }
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

            foreach (Launch launch in SpaceXDataManager.Launches)
            {
                GameObject launchItemObject = GetLaunchItemFromPool();
                LaunchItem launchItem = launchItemObject.GetComponent<LaunchItem>();
                launchItem.Setup(launch, delegate { SpaceXDataManager.GetAllShips(launch);});
            }
        }
        private void SetupShips(Launch launch)
        {
            if (launch == null) return;

            foreach (Ship ship in launch.Ships)
            {
                GameObject launchItemObject = GetShipItemFromPool();
                ShipItem shipItem = launchItemObject.GetComponent<ShipItem>();
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
        private GameObject GetLaunchItemFromPool()
        {
            if (_launchItemPool.Count > 0)
            {
                GameObject pooledItem = _launchItemPool.Dequeue();
                pooledItem.SetActive(true);
                return pooledItem;
            }
            else
            {
                return Instantiate(LaunchItemPrefab, LaunchItemContent);
            }
        }
        private GameObject GetShipItemFromPool()
        {
            if (_launchItemPool.Count > 0)
            {
                GameObject pooledItem = _shipItemPool.Dequeue();
                pooledItem.SetActive(true);
                return pooledItem;
            }
            else
            {
                return Instantiate(ShipItemPrefab, ShipItemContent);
            }
        }
    }
}
