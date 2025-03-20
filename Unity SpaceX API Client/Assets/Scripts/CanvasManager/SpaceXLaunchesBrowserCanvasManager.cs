using DG.Tweening;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using static Lobby.LobbyCanvasManager;
using static SpaceXLaunchesBrowser.SpaceXDataManager;
using System.Linq;
using Unity.VisualScripting;

namespace SpaceXLaunchesBrowser
{
    public class SpaceXLaunchesBrowserCanvasManager : MonoBehaviour
    {
        public static LaunchItem LastPoppedLaunchItem;
        public static ShipItem LastPoppedShipItem;

        public static UnityEvent OnStartLoading = new();
        public static UnityEvent OnEndLoading = new();
        public static UnityEvent OnGetLaunches = new();
        public static UnityEvent<Launch> OnGetShips = new();
        public static UnityEvent OnGetImage = new();
        public static UnityEvent OnShipItemDownScrollDirection = new();
        public static UnityEvent OnShipItemUpScrollDirection = new();
        public static UnityEvent OnLaunchItemDownScrollDirection = new();
        public static UnityEvent OnLaunchItemUpScrollDirection = new();
        public static UnityEvent<ShipItem, float> OnShipItemBecameInvisible = new();
        public static UnityEvent<LaunchItem, float> OnLaunchItemBecameInvisible = new();

        public SpaceXDataManager SpaceXDataManager;
        public ShipImage ShipImage;
        public GameObject LoadingCircle;
        public GameObject ShipDetailsPanel;
        public GameObject ShipImagePanel;
        public GameObject LaunchItemPrefab;
        public GameObject ShipItemPrefab;

        public Transform ShipItemContent;
        public Transform LaunchItemContent;

        public RectTransform LaunchItemViewportRectTransform;
        public ScrollRect LaunchItemScrollRect;

        public Button LaunchItemBackArrow;
        public Button ShipItemBackArrow;

        private Stack<LaunchItem> _launchItemPool = new();
        private Stack<ShipItem> _shipItemPool = new();

        private void Start()
        {
            DOTween.Init();

            ResetCanvas();

            OnStartLoading.AddListener(ShowLoadingCircle);
            OnEndLoading.AddListener(HideLoadingCircle);
            OnGetLaunches.AddListener(SetupLaunches);
            OnShipItemBecameInvisible.AddListener(PushShipItem);
            OnLaunchItemBecameInvisible.AddListener(PushLaunchItem);
            OnShipItemDownScrollDirection.AddListener(ReversedPopShipItem);
            OnShipItemUpScrollDirection.AddListener(PopShipItem);
            OnLaunchItemDownScrollDirection.AddListener(ReversedPopLaunchItem);
            OnLaunchItemUpScrollDirection.AddListener(PopLaunchItem);

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
            _launchItemPool.Clear();
        }
        private void ResetShipItems()
        {
            ShipDetailsPanel.SetActive(false);

            _shipItemPool.Clear();
        }
        private void PushShipItem(ShipItem shipItem, float scrollDirection)
        {
            shipItem.gameObject.SetActive(false);
            _shipItemPool.Push(shipItem);
        }
        private void PopShipItem()
        {
            ShipItem pooledItem = _shipItemPool.Pop();
            pooledItem.gameObject.SetActive(true);
        }
        private void ReversedPopShipItem()
        {
            if (_shipItemPool.Count <= 0) return;

            List<ShipItem> workList = _shipItemPool.ToList();
            workList.Reverse();

            ShipItem pooledItem = workList[0];
            pooledItem.gameObject.SetActive(true);
        }
        private void PushLaunchItem(LaunchItem launchItem, float scrollDirection)
        {
            if (scrollDirection < 0)
            {
                SpaceXLaunchesBrowserCanvasManager.OnLaunchItemUpScrollDirection.Invoke();
            }
            else if (scrollDirection > 0)
            {
                SpaceXLaunchesBrowserCanvasManager.OnLaunchItemDownScrollDirection.Invoke();
            }

            launchItem.gameObject.SetActive(false);
            _launchItemPool.Push(launchItem);
        }
        private void PopLaunchItem()
        {
            if (_launchItemPool.Count <= 0) return;

            LaunchItem pooledItem = _launchItemPool.Pop();
            pooledItem.gameObject.SetActive(true);

            LastPoppedLaunchItem = pooledItem;
        }
        private void ReversedPopLaunchItem()
        {
            if (_launchItemPool.Count <= 0) return;

            List<LaunchItem> workList = _launchItemPool.ToList();
            workList.Reverse();

            LaunchItem pooledItem = workList[0];
            pooledItem.gameObject.SetActive(true);

            LastPoppedLaunchItem = pooledItem;
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
                LaunchItem launchItem = GetLaunchItem();

                if (launchItem == null) continue;

                launchItem.Setup(launch, delegate { SpaceXDataManager.GetAllShips(launch); }, LaunchItemViewportRectTransform, LaunchItemScrollRect);
            }
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
