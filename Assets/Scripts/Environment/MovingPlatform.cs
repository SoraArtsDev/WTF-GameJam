// Developed by Pluto
//
// Copyright(c) Sora Arts 2023-2024
//
// This script is covered by a Non-Disclosure Agreement (NDA) and is Confidential.
// Destroy the file immediately if you have not been explicitly granted access.

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Sora.Environment
{
    [System.Serializable]
    public class WayPoint
    {
        public Vector3 position;
        public float haltDuration;
    }

    public class MovingPlatform : MonoBehaviour
    {
        [SerializeField] private WayPoint[] wayPoints;
        [SerializeField] private float timeToNextWayPoint;
        [SerializeField] private bool moveOnAwake;
        
        private bool startMoving;
        private float currentTime;
        private Vector3 targetPosition;
        private int wayPointIndex;

        private void OnEnable()
        {
            transform.localPosition = wayPoints[0].position;

            startMoving = moveOnAwake;
            currentTime = 0.0f;

            wayPointIndex = 1;
            targetPosition = wayPoints[wayPointIndex].position;
        }

        private void Update()
        {
            if(startMoving)
            {
                transform.localPosition = Vector3.MoveTowards(transform.localPosition, targetPosition, currentTime / timeToNextWayPoint);

                currentTime += Time.deltaTime;

                if (Vector3.Distance(transform.localPosition, targetPosition) <= 0.01f)
                {
                    transform.localPosition = targetPosition;
                    startMoving = false;
                    StartCoroutine(ResetHaltDuration());
                }
            }
        }

        private IEnumerator ResetHaltDuration()
        {
            yield return new WaitForSecondsRealtime(wayPoints[wayPointIndex].haltDuration);
            startMoving = true;
            wayPointIndex++;
            if (wayPointIndex == wayPoints.Length)
                wayPointIndex = 0;

            targetPosition = wayPoints[wayPointIndex].position;
            currentTime = 0.0f;
        }

        public void StartMoving()
        {
            startMoving = true;
        }
    }
}