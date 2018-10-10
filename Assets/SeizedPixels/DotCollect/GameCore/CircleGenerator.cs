using System;
using UnityEngine;
using Random = UnityEngine.Random;

namespace SeizedPixels.DotCollect.GameCore
{
    public class CircleGenerator : MonoBehaviour
    {
        [SerializeField] private float _minSize = 0.23f;
        [SerializeField] private float _maxSize = 0.4f;
        [SerializeField] private float _segmentFactor = 3;
        [SerializeField] private float _density = 10;
        [SerializeField] private float _gapSize = 0.3f;
        [SerializeField] private float _radius = 5f;
        [SerializeField] private GameObject _lineRendererPrefab;

        public int Holes = 4;

        private float _previous;

        private void Start()
        {
            _previous = Random.Range(0, 2 * Mathf.PI);

            float total = 2 * Mathf.PI;
            int sizeCount = 2 * Holes;
            float[] sizes = new float[sizeCount];

            float start = Time.realtimeSinceStartup;

            generate:
            float sum = 0;
            for (int i = 0; i < sizeCount; i++)
            {
                // Generation
                float value = Random.value;
                if (i % 2 == 0)
                {
                    value *= _segmentFactor;
                }

                sum += sizes[i] = value;
            }

            for (int i = 0; i < sizeCount; i++)
            {
                float n = sizes[i] *= total / sum;

                // Validation
                if (n < _minSize || i % 2 == 1 && n > _maxSize)
                {
                    if (Time.realtimeSinceStartup - start > 3)
                    {
                        throw new TimeoutException();
                    }

                    goto generate;
                }
            }

            for (int i = 0; i < sizeCount; i += 2)
            {
                Add(sizes[i]);
            }
        }

        private void Add(float size)
        {
            Create(_previous, size);
            _previous += _gapSize + size;
        }

        private void Create(float offset, float size)
        {
            LineRenderer lineRenderer = Instantiate(_lineRendererPrefab, transform, false).GetComponent<LineRenderer>();
            int pointCount = (int) (size * _density);
            float innerRadius = _radius - lineRenderer.startWidth / 2;
            float outerRadius = _radius + lineRenderer.startWidth / 2;
            Vector3[] middlePoints = new Vector3[pointCount];
            Vector2[] colliderPoints = new Vector2[pointCount * 2];
            for (int i = 0; i < pointCount; i++)
            {
                float p = i / _density + offset;
                middlePoints[i] = new Vector3(Mathf.Cos(p), Mathf.Sin(p)) * _radius;
                colliderPoints[2 * pointCount - i - 1] = new Vector2(Mathf.Cos(p), Mathf.Sin(p)) * innerRadius;
                colliderPoints[i] = new Vector2(Mathf.Cos(p), Mathf.Sin(p)) * outerRadius;
            }

            lineRenderer.positionCount = pointCount;
            lineRenderer.SetPositions(middlePoints);
            lineRenderer.gameObject.SetActive(true);
            PolygonCollider2D poly = GetComponent<PolygonCollider2D>();
            GetComponent<PolygonCollider2D>().SetPath(poly.pathCount++, colliderPoints);
        }
    }
}