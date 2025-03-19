using UnityEditor;
using UnityEngine;

namespace TeslaRoadsterSimulation
{
    [CustomEditor(typeof(OrbitalDataSO))]
    public class OrbitalDataEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();

            OrbitalDataSO script = (OrbitalDataSO)target;

            if (GUILayout.Button("Load CSV"))
            {
                script.LoadCSV();
            }

            if (GUILayout.Button("Save ScriptableObject"))
            {
                SaveData();
            }
        }
        private void SaveData()
        {
            EditorUtility.SetDirty(this);
            AssetDatabase.SaveAssets();
            Debug.Log("ScriptableObject saved!");
        }
    }
}
