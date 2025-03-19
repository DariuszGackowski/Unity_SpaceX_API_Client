using UnityEngine;
using static Lobby.LobbyCanvasManager;
using UnityEngine.SceneManagement;
using TMPro;

namespace TeslaRoadsterSimulation
{
    public class RoadsterSimulationCanvasManager : MonoBehaviour
    {
        public TextMeshProUGUI OrbitalDataText;
        public void LoadLobbyScene()
        {
            SceneManager.LoadScene(GameScenes.Lobby.ToString());
        }
        public void SetToOrbitalDataText(string text) 
        {
            OrbitalDataText.SetText(text);
        }
    }
}
