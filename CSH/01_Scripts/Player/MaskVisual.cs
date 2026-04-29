using UnityEngine;

namespace Work.CSH.Code.Player
{
    public class MaskVisual : MonoBehaviour
    {
        [SerializeField] private SpriteRenderer maskRenderer;
        [SerializeField] private SpriteRenderer parentRenderer;
        [SerializeField] private Animator parentAnimator;



        private void Update()
        {
            maskRenderer.sprite = parentRenderer.sprite;
            maskRenderer.flipX = parentRenderer.flipX;
        }
    }
}
