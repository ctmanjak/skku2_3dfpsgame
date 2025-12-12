using System;
using UnityEngine;

namespace Drum
{
    [RequireComponent(typeof(Rigidbody))]
    public class DrumMove : MonoBehaviour
    {
        private Rigidbody _rigidbody;

        private void Awake()
        {
            _rigidbody = GetComponent<Rigidbody>();
        }

        public void AddForce(Vector3 force)
        {
            _rigidbody.AddForce(force);
        }

        public void AddTorque(Vector3 torque)
        {
            _rigidbody.AddTorque(torque);
        }
    }
}