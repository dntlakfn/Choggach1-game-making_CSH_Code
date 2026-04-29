using UnityEngine;
using UnityEngine.UI;

namespace CSH.Scripts.UIs
{
    [RequireComponent(typeof(Graphic))]
    public class LineTextureTiler : MonoBehaviour
    {
        [Header("타일링 설정")]
        [Tooltip("숫자가 작을수록 텍스처가 촘촘하게 반복됩니다.")]
        public float tileSize = 50f;

        [Header("선의 총 길이 (실시간 조절용)")]
        [Tooltip("스크립트를 통해 이 값을 실시간 선의 길이로 업데이트 해주세요.")]
        public float currentLineLength = 500f;

        private Graphic uiGraphic;
        private Material instancedMaterial;

        void Start()
        {
            uiGraphic = GetComponent<Graphic>();

            // 중요: 다른 UI에 영향이 가지 않도록 이 선 전용 복제 매테리얼을 생성합니다.
            if (uiGraphic.material != null)
            {
                instancedMaterial = new Material(uiGraphic.material);
                uiGraphic.material = instancedMaterial;
            }
        }

        void Update()
        {
            if (instancedMaterial == null || tileSize <= 0) return;

            // 선의 길이에 비례하여 타일링 횟수를 계산
            float repeatCount = currentLineLength / tileSize;

            // 매테리얼의 Tiling X 값을 실시간으로 변경 (Y는 1로 고정)
            instancedMaterial.mainTextureScale = new Vector2(repeatCount, 1f);
        }
    }
}