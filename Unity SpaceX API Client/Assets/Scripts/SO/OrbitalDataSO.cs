using System.Collections.Generic;
using System.Globalization;
using UnityEditor;
using UnityEngine;

namespace TeslaRoadsterSimulation
{

    [System.Serializable]
    public class OrbitalData
    {
        public double EpochJD;
        public string DateUTC;
        public double SemiMajorAxis;
        public double Eccentricity;
        public double Inclination;
        public double LongitudeOfAscNode;
        public double ArgumentOfPeriapsis;
        public double MeanAnomaly;
        public double TrueAnomaly;

        public OrbitalData(double epochJD, string dateUTC, double semiMajorAxis, double eccentricity, double inclination, double longitudeOfAscNode, double argumentOfPeriapsis, double meanAnomaly, double trueAnomaly)
        {
            EpochJD = epochJD;
            DateUTC = dateUTC;
            SemiMajorAxis = semiMajorAxis;
            Eccentricity = eccentricity;
            Inclination = inclination;
            LongitudeOfAscNode = longitudeOfAscNode;
            ArgumentOfPeriapsis = argumentOfPeriapsis;
            MeanAnomaly = meanAnomaly;
            TrueAnomaly = trueAnomaly;
        }
        public static bool TryParseValues(string[] values, out OrbitalData data)
        {
            data = null;

            if (values.Length < 9)
                return false;

            if (double.TryParse(values[0], NumberStyles.Float, CultureInfo.InvariantCulture, out double epochJD) &&
                double.TryParse(values[2], NumberStyles.Float, CultureInfo.InvariantCulture, out double semiMajorAxis) &&
                double.TryParse(values[3], NumberStyles.Float, CultureInfo.InvariantCulture, out double eccentricity) &&
                double.TryParse(values[4], NumberStyles.Float, CultureInfo.InvariantCulture, out double inclination) &&
                double.TryParse(values[5], NumberStyles.Float, CultureInfo.InvariantCulture, out double longitudeOfAscNode) &&
                double.TryParse(values[6], NumberStyles.Float, CultureInfo.InvariantCulture, out double argumentOfPeriapsis) &&
                double.TryParse(values[7], NumberStyles.Float, CultureInfo.InvariantCulture, out double meanAnomaly) &&
                double.TryParse(values[8], NumberStyles.Float, CultureInfo.InvariantCulture, out double trueAnomaly))
            {
                data = new OrbitalData(epochJD, values[1], semiMajorAxis, eccentricity, inclination, longitudeOfAscNode,
                    argumentOfPeriapsis, meanAnomaly, trueAnomaly);
                return true;
            }

            return false;
        }
    }
    [CreateAssetMenu(fileName = "NewOrbitalDataSO", menuName = "ScriptableObjects/OrbitalDataSO")]
    public class OrbitalDataSO : ScriptableObject
    {
        public TextAsset CSVFile;
        public List<OrbitalData> OrbitalDataList = new();

        public void LoadCSV()
        {
            if (CSVFile == null)
            {
                Debug.LogError("CSV File not assigned!");
                return;
            }

            OrbitalDataList.Clear();

            string[] lines = CSVFile.text.Split('\n');
            for (int i = 1; i < lines.Length; i++)
            {
                string line = lines[i].Trim();

                if (string.IsNullOrEmpty(line) || line.StartsWith(",")) continue;


                string[] values = line.Split(',');

                if (OrbitalData.TryParseValues(values, out OrbitalData data))
                {
                    OrbitalDataList.Add(data);
                }
                else
                {
                    Debug.LogWarning($"Failed to parse line {i}: {line}");
                }
            }

            Debug.Log("Orbital data loaded successfully!");
        }
    }
}