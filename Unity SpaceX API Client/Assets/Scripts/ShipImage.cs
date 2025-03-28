using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace SpaceXLaunchesBrowser
{
    public class ShipImage : MonoBehaviour
    {
        public Button BackArrow;
        public Image Image;
        public void Setup(UnityAction functionToAdd)
        {
            Image.sprite = SpaceXDataManager.CurrentShipSprite;

            BackArrow.onClick.AddListener(functionToAdd);
        }
    }
}