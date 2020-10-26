using Route;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Events;

namespace Game
{
    public class SimpleObjectOperator : MonoBehaviour
    {
        [SerializeField] private RouteControlCenter _routeControlCenter;
        [SerializeField] private GameObject _mark;
        [SerializeField] private Transform _map;

        private List<GameObject> _trajectory = new List<GameObject>();

        private const double ROTATION_FAULT = .001;
        private const int ROTATION_SPEED = 10;
        private const int MOVEMENT_SPEED = 1000;
        private const int MARK_DISTANCE = 10000;

        public void StartRoute(GameObject destination)
        {
            StartCoroutine(MoveTo(destination));
        }

        private void Start()
        {
            _routeControlCenter.NewRouteReady += StartRoute;
        }

        private void OnDestroy()
        {
            foreach (GameObject mark in _trajectory)
                Destroy(mark);
        }

        private IEnumerator MoveTo(GameObject destination)
        {
            yield return StartCoroutine(RotateTo(destination));

            yield return StartCoroutine(ReachTo(destination));
        }

        private IEnumerator RotateTo(GameObject target)
        {
            Vector3 vectorToTarget = target.transform.position - transform.position;

            float angleToTarget = (Mathf.Atan2(vectorToTarget.y, vectorToTarget.x) * Mathf.Rad2Deg) - 90;
            Quaternion quaternionToTarget = Quaternion.AngleAxis(angleToTarget, Vector3.forward);

            while (IsRotationComplete(gameObject, quaternionToTarget, ROTATION_FAULT))
            {
                yield return null;

                transform.rotation = Quaternion.Slerp(transform.rotation, quaternionToTarget, Time.deltaTime * ROTATION_SPEED);
            }

        }

        private IEnumerator ReachTo(GameObject target)
        {
            Vector2 lastMarkPosition = transform.position;

            while (IsTargetReached(gameObject, target))
            {
                yield return null;

                transform.position = Vector2.MoveTowards(transform.position, target.transform.position, Time.deltaTime * MOVEMENT_SPEED);

                if (IsNeedToMark(lastMarkPosition, gameObject, MARK_DISTANCE))
                {
                    lastMarkPosition = MarkPosition().transform.position;
                }
            }

            _routeControlCenter.DestinationReached();

            MarkPosition();
        }

        private GameObject MarkPosition()
        {
            var mark = Instantiate(_mark, new Vector2(transform.position.x, transform.position.y), new Quaternion(), _map);  //Not sure about the task if the old trajectory needs to be deleted. In that case, I would use object pooling.

            _trajectory.Add(mark);

            return mark;
        }

        private static bool IsRotationComplete(GameObject simpleObject, Quaternion angel, double fault)
        {
            return Math.Abs(Math.Abs(simpleObject.transform.rotation.eulerAngles.z) - Math.Abs(angel.eulerAngles.z)) >= fault;
        }

        private static bool IsTargetReached(GameObject simpleObject, GameObject target)
        {
            return simpleObject.transform.position != target.transform.position;
        }

        private static bool IsNeedToMark(Vector2 lastMarkPosition, GameObject simpleObject, int distance)
        {
            return (lastMarkPosition - (Vector2)simpleObject.transform.position).sqrMagnitude > distance;
        }
    }
}