using System;
using System.Collections.Generic;
using UnityEngine;
using RG.OrbitalElements;
using DG.Tweening;
using UnityEngine.Events;

namespace TeslaRoadsterSimulation
{
    public class RoadsterOrbitManager : MonoBehaviour
    {
        public UnityEvent OnChangePosition = new();

        public OrbitalDataSO OrbitalDataSO;
        [Space]
        public RoadsterSimulationCanvasManager RoadsterSimulationCanvasManager;
        [Space]
        public Transform SunTransform;
        public GameObject RoadsterModel;
        public LineRenderer FlightPathLine;

        private const int _flightPathLength = 20;
        private const float _rotationCameraSpeed = 5f;
        private const float _customSimulationSpeed = 864000f; // 10 days per second
        private int _currentIndex = 0;
        private DateTime _currentDateTime;
        private readonly List<Vector3> _flightPath = new();

        private void Start()
        {
            InitializeSimulation();
        }

        private void Update()
        {
            SimulateRoadsterOrbit();
            HandleTouchInput();
        }

        private void InitializeSimulation()
        {
            if (!ValidateDependencies()) return;

            DOTween.Init();
            OnChangePosition.AddListener(UpdateRoadsterPosition);

            _currentDateTime = DateTime.Parse(OrbitalDataSO.OrbitalDataList[0].DateUTC);
            FlightPathLine.positionCount = _flightPathLength;

            Debug.Log($"RoadsterOrbit script started. Loaded orbital data records: {OrbitalDataSO.OrbitalDataList.Count}");

            Vector3 initialPosition = CalculateRoadsterPosition(OrbitalDataSO.OrbitalDataList[0]);
            RoadsterModel.transform.position = initialPosition;
            _flightPath.AddRange(new Vector3[_flightPathLength]);
            _flightPath.ForEach(pos => pos = initialPosition);
        }

        private bool ValidateDependencies()
        {
            if (OrbitalDataSO == null || SunTransform == null || RoadsterModel == null || FlightPathLine == null || RoadsterSimulationCanvasManager == null || RoadsterSimulationCanvasManager.OrbitalDataText == null)
            {
                Debug.LogError("Missing assignments in the Inspector!");
                return false;
            }
            if (OrbitalDataSO.OrbitalDataList.Count == 0)
            {
                Debug.LogError("No orbital data in Scriptable Object!");
                return false;
            }
            return true;
        }

        private void SimulateRoadsterOrbit()
        {
            if (OrbitalDataSO.OrbitalDataList.Count == 0) return;

            UpdateTime();
            UpdateRoadsterFlightPath();
            DisplayOrbitalData();
        }

        private void HandleTouchInput()
        {
            if (Input.touchCount <= 0) return;

            Touch touch = Input.GetTouch(0);
            if (touch.phase == TouchPhase.Moved)
            {
                Vector3 rotation = new Vector3(touch.deltaPosition.y, -touch.deltaPosition.x) * _rotationCameraSpeed * Time.deltaTime;
                Camera.main.transform.Rotate(rotation, Space.Self);
            }
        }

        private void UpdateTime()
        {
            _currentDateTime = _currentDateTime.AddSeconds(Time.deltaTime * _customSimulationSpeed);
            DateTime lastDate = DateTime.Parse(OrbitalDataSO.OrbitalDataList[^1].DateUTC);

            if (_currentDateTime > lastDate)
            {
                _currentDateTime = DateTime.Parse(OrbitalDataSO.OrbitalDataList[0].DateUTC);
                _currentIndex = 0;
                Debug.Log("Simulation looped back to the beginning.");
            }

            for (int i = _currentIndex; i < OrbitalDataSO.OrbitalDataList.Count; i++)
            {
                if (DateTime.Parse(OrbitalDataSO.OrbitalDataList[i].DateUTC) >= _currentDateTime)
                {
                    _currentIndex = i;
                    OnChangePosition.Invoke();
                    break;
                }
            }
        }

        private void UpdateRoadsterPosition()
        {
            if (_currentIndex + 1 >= OrbitalDataSO.OrbitalDataList.Count) return;

            OrbitalData currentData = OrbitalDataSO.OrbitalDataList[_currentIndex];
            OrbitalData nextData = OrbitalDataSO.OrbitalDataList[_currentIndex + 1];

            float duration = (float)(DateTime.Parse(nextData.DateUTC) - DateTime.Parse(currentData.DateUTC)).TotalSeconds / _customSimulationSpeed;
            Vector3 nextPosition = CalculateRoadsterPosition(nextData);

            RoadsterModel.transform.DOKill();
            RoadsterModel.transform.DOMove(nextPosition, duration).SetEase(Ease.InOutSine).SetUpdate(UpdateType.Fixed);
        }

        private void UpdateRoadsterFlightPath()
        {
            _flightPath.Insert(0, RoadsterModel.transform.position);
            if (_flightPath.Count > _flightPathLength) _flightPath.RemoveAt(_flightPathLength);

            FlightPathLine.positionCount = _flightPath.Count;
            FlightPathLine.SetPositions(_flightPath.ToArray());
        }

        private void DisplayOrbitalData()
        {
            OrbitalData currentData = OrbitalDataSO.OrbitalDataList[_currentIndex];
            string text = $"" +
                $"Date (UTC): {currentData.DateUTC}\n" +
                $"Date (Local): {_currentDateTime.ToLocalTime()}\n" +
                $"Semi-major axis: {currentData.SemiMajorAxis} au\n" +
                $"Eccentricity: {currentData.Eccentricity}\n" +
                $"Inclination: {currentData.Inclination} degrees\n" +
                $"Longitude of asc. node: {currentData.LongitudeOfAscNode} degrees\n" +
                $"Argument of periapsis: {currentData.ArgumentOfPeriapsis} degrees\n" +
                $"Mean Anomaly: {currentData.MeanAnomaly} degrees\n" +
                $"True Anomaly: {currentData.TrueAnomaly} degrees";

            RoadsterSimulationCanvasManager.SetToOrbitalDataText(text);
        }

        private Vector3 CalculateRoadsterPosition(OrbitalData data)
        {
            Vector3Double position = Calculations.CalculateOrbitalPosition(
                data.SemiMajorAxis,
                data.Eccentricity,
                data.Inclination,
                data.LongitudeOfAscNode,
                data.ArgumentOfPeriapsis,
                data.TrueAnomaly
            );

            return new Vector3((float)position.x / 1000f, (float)position.y / 1000f, (float)position.z / 1000f);
        }
    }
}