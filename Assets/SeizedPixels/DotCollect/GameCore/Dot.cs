using System.Collections;
using DG.Tweening;
using UnityEngine;

namespace SeizedPixels.DotCollect.GameCore
{
    public class Dot : MonoBehaviour
    {
        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.gameObject.CompareTag("DotCollector"))
                StartCoroutine(Collect());

            if (other.gameObject.CompareTag("Bounds"))
                PlayerController.Instance.StartCoroutine("Die");
        }

        private IEnumerator Collect()
        {
            GetComponent<SpriteRenderer>().DOFade(0, 0.5f);

            if (PlayerController.Instance.State == GameState.Ingame)
                PlayerController.Instance.Score++;

            if (Camera.main != null)
                Camera.main.GetComponent<AudioSource>().PlayOneShot(PlayerController.Instance.CollectSound);

            transform.DOMove(Vector3.zero, 0.5f);
            yield return new WaitForSeconds(0.5f);
            Destroy(this);
        }
    }
}