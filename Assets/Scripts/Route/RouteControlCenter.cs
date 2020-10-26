using UnityEngine;
using Lean.Touch;
using System.Collections.Generic;
using UnityEngine.Assertions.Must;
using UnityEngine.Events;
using System;

namespace Route
{
    public class RouteControlCenter : MonoBehaviour
    {
        [SerializeField] private Transform _map;
        [SerializeField] private GameObject _destination;

        private Queue<GameObject> _route = new Queue<GameObject>();

        public Action<GameObject> NewRouteReady;

        public void DestinationReached()
        {
            Destroy(_route.Dequeue());

            if(_route.Count > 0)
                NewRouteReady?.Invoke(_route.Peek());
        }

        private void RecordDestination(LeanFinger finger)
        {
            if (finger.IsOverGui)
                return;

            var x = finger.LastScreenPosition.x;
            var y = finger.LastScreenPosition.y;

            var destination = Instantiate(_destination, new Vector2(x, y), Quaternion.identity, _map);

            _route.Enqueue(destination);

            if (_route.Count == 1)
                NewRouteReady?.Invoke(destination);
        }

        private void Start()
        {
            Lean.Touch.LeanTouch.OnFingerDown += RecordDestination;
        }

        private void OnDestroy()
        {
            foreach (var destination in _route)
                Destroy(destination);
        }
    }
}