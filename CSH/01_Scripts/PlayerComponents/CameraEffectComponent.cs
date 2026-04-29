using CIW.Code.Player.Field;
using DG.Tweening;
using System;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using Work.PSB.Code.CoreSystem;
using YIS.Code.Modules;

namespace Work.CSH.Code.PlayerComponents
{
    public class CameraEffectComponent : MonoBehaviour, IModule
    {
        [SerializeField] private CinemachineCamera cinemachine;
        [SerializeField] private TransitionController transitionController;
        [SerializeField] private RectTransform attackText;
        [SerializeField] private VolumeProfile volume;
        [SerializeField] private PixelPerfectCamera pixelPerfectCamera;
        [SerializeField] private PlayerFieldInputSO playerInput;
        [SerializeField] private string sceneName = "BattleScene";

        [Header("Lens Distortion")]
        [SerializeField] private float distortionTarget = -1f;
        [SerializeField] private float distortionDuration = 1f;

        [Header("Zoom")]
        [SerializeField] private float zoomInSize = 5f;
        [SerializeField] private float zoomInDuration = 0.5f;
        [SerializeField] private Ease zoomInEase = Ease.OutCirc;

        [Header("Transition Lead Time")]
        [SerializeField] private float transitionLeadSeconds = 1.4f;
        
        //트랜지션 시작과 복구 여부
        private bool _restoreQueued;
        private bool _transitionStarted;

        //트윈
        private Tween _distortionTween;
        private Tween _leadTween;

        //볼륨 복구용도
        private LensDistortion _distortion;
        private Bloom _bloom;
        private ScreenSpaceLensFlare _lensFlare;

        public void Initialize(ModuleOwner owner)
        {
        }

        private void Awake()
        {
            CacheVolumeOverrides();

            if (transitionController != null)
                transitionController.OnClosed += RestoreAfterClosed;
        }

        private void OnDestroy()
        {
            if (transitionController != null)
                transitionController.OnClosed -= RestoreAfterClosed;

            _distortionTween?.Kill(false);
            _leadTween?.Kill(false);
        }

        private void CacheVolumeOverrides()
        {
            if (volume == null) return;

            volume.TryGet(out _distortion);
            volume.TryGet(out _bloom);
            volume.TryGet(out _lensFlare);
        }

        private void RestoreAfterClosed()
        {
            if (!_restoreQueued) return;
            _restoreQueued = false;

            _distortionTween?.Kill(false);
            _distortionTween = null;

            Time.timeScale = 1f;

            if (pixelPerfectCamera != null)
                pixelPerfectCamera.enabled = true;

            if (_distortion != null)
                _distortion.intensity.value = 0f;

            if (_bloom != null)
                _bloom.intensity.value = 0f;

            if (_lensFlare != null)
                _lensFlare.intensity.value = 0f;
        }

        public void EnemyAttack(Action OnComplete = null)
        {
            playerInput.DisableInput();

            _restoreQueued = true;
            _transitionStarted = false;

            _distortionTween?.Kill(false);
            _leadTween?.Kill(false);

            if (pixelPerfectCamera != null)
                pixelPerfectCamera.enabled = false;

            Time.timeScale = 0.5f;

            if (_bloom != null)
                _bloom.intensity.value = 5f;

            if (_lensFlare != null)
            {
                DOTween.To(() => _lensFlare.intensity.value, x => _lensFlare.intensity.value = x, 15f, 0.1f)
                    .SetEase(Ease.OutCirc)
                    .OnComplete(() =>
                    {
                        DOTween.To(() => _lensFlare.intensity.value, x => _lensFlare.intensity.value = x, 40f, 1.3f)
                            .SetEase(Ease.OutCirc);
                    });
            }

            if (attackText != null)
            {
                attackText.anchoredPosition = new Vector2(-1100, attackText.anchoredPosition.y);
                attackText.DOAnchorPosX(0, 0.5f).SetEase(Ease.OutCirc)
                    .OnComplete(() =>
                    {
                        attackText.DOAnchorPosX(1100, 0.5f).SetEase(Ease.InCirc);
                    });
            }

            if (cinemachine != null)
            {
                DOTween.Kill(cinemachine, complete: false);

                DOTween.To(() => cinemachine.Lens.OrthographicSize,
                           x => cinemachine.Lens.OrthographicSize = x,
                           zoomInSize,
                           zoomInDuration)
                       .SetEase(zoomInEase)
                       .SetTarget(cinemachine);
            }

            if (_distortion != null)
            {
                _distortion.intensity.value = 0f;

                AnimationCurve accelCurve = new AnimationCurve(
                    new Keyframe(0f, 0f, 0.2f, 0.2f),
                    new Keyframe(0.6f, 0.25f, 1.5f, 1.5f),
                    new Keyframe(1f, 1f, 4f, 0f)
                );
                
                _leadTween = DOVirtual.DelayedCall(transitionLeadSeconds, StartTransitionIfNeeded)
                    .SetUpdate(true);

                _distortionTween = DOTween.To(
                        () => _distortion.intensity.value,
                        x => _distortion.intensity.value = x,
                        distortionTarget,
                        distortionDuration
                    )
                    .SetEase(accelCurve)
                    .OnComplete(() =>
                    {
                        FinishAttackEffect(OnComplete);
                    });
            }
            else
            {
                StartTransitionIfNeeded();
                FinishAttackEffect(OnComplete);
            }
        }

        private void StartTransitionIfNeeded()
        {
            if (_transitionStarted) return;
            if (transitionController == null) return;

            _transitionStarted = true;
            transitionController.nextScene = sceneName;
            transitionController.Transition();
        }

        private void FinishAttackEffect(Action onComplete)
        {
            playerInput.EnableInput();
            StartTransitionIfNeeded();
            onComplete?.Invoke();
        }
        
    }
}