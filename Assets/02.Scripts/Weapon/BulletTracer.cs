using System.Collections;
using Core;
using UnityEngine;

namespace Weapon
{
    [RequireComponent(typeof(LineRenderer))]
    public class BulletTracer : MonoBehaviour
    {
        private LineRenderer _lineRenderer;
        private Transform _muzzleTransform;

        private void Awake()
        {
            _lineRenderer = GetComponent<LineRenderer>();
        }

        public void Initialize(Transform muzzle)
        {
            _muzzleTransform = muzzle;
        }

        public void Fire(Vector3 endPosition)
        {
            _lineRenderer.SetPosition(0, _muzzleTransform.position);
            _lineRenderer.SetPosition(1, endPosition);

            StartCoroutine(FireRoutine());
        }

        private IEnumerator FireRoutine()
        {
            yield return new WaitForSeconds(0.08f);
            PoolManager.Release(gameObject);
        }
    }
}