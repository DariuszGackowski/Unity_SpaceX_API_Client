using UnityEngine;
using UnityEngine.SceneManagement;

namespace Lobby
{
    public class LobbyCanvasManager : MonoBehaviour
    {
        public enum GameScenes
        {
            Lobby,
            SpaceXLaunchesBrowser,
            TeslaRoadsterSimulation
        }

        public void LoadSpaceXLaunchesBrowserScene()
        {
            SceneManager.LoadScene(GameScenes.SpaceXLaunchesBrowser.ToString());
        }

        public void LoadTeslaRoadsterSimulationScene()
        {
            SceneManager.LoadScene(GameScenes.TeslaRoadsterSimulation.ToString());
        }

        public void ExitGame()
        {
            Application.Quit();
        }
    }
}
