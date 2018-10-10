using System;
using System.Collections;
using UnityEngine;
using Random = UnityEngine.Random;

namespace SeizedPixels.DotCollect.GameCore
{
    public class DotSpawner : MonoBehaviour
    {
        [SerializeField] private Transform _dotParent;
        private float _startGravity;
        private float _previousSpawnPosX;

        [HideInInspector] public bool DoSpawn;
        public Dot Dot;

        private void Awake()
        {
            _startGravity = Dot.GetComponent<Rigidbody2D>().gravityScale;
            StartCoroutine(SpawnDots());
        }

        private IEnumerator SpawnDots()
        {
            if (DoSpawn)
            {
                Bounds bounds = GetComponent<EdgeCollider2D>().bounds;
                float spawnPosX = Random.Range(bounds.min.x, bounds.max.x);

                while (Math.Abs(spawnPosX - _previousSpawnPosX) < .25)
                {
                    spawnPosX = Random.Range(bounds.min.x, bounds.max.x);
                }

                Transform dot = Instantiate(Dot, new Vector3(spawnPosX, transform.position.y, transform.position.z), Quaternion.identity).transform;
                dot.parent = _dotParent;
                _previousSpawnPosX = spawnPosX;
            }

            double cooldown = 0.4 + 0.7 * Math.Pow(1 - 0.02, PlayerController.Instance.Score);
            yield return new WaitForSeconds(Random.Range((float) (cooldown - 0.15), (float) (cooldown + 0.15)));
            
            StartCoroutine(SpawnDots());
        }

        public void UpdateScore(int newScore)
        {
            if (newScore % 10 == 0)
            {
                Dot.GetComponent<Rigidbody2D>().gravityScale += 0.05f;
            }
        }

        public void ResetSpawner()
        {
            Dot.GetComponent<Rigidbody2D>().gravityScale = _startGravity;
        }
    }
}