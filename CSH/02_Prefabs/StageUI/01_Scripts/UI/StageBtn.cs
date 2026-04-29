using DG.Tweening;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine.UI.Extensions;
using Work.CSH.Code.Enums;
using Work.CSH.Code.StageUIs;

namespace Work.Scripts.UI
{


    

    public class StageBtn : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        private List<StageSubType> stageSubTypes;
        private StageType stageMainType;
        private Button btn;
        [SerializeField] private UILineRenderer lineRendererPrefab;
        private List<UILineRenderer> lineRenderer;
        private Image iconImage;
        private RectTransform rectTransform;
        private bool isSelected;
        public bool isActive;
        public bool isInteractable;
        public int sceneIdx;
        public Transform tweenReference = null;
        [SerializeField] private Transform stageSubTypesContainer;
        [SerializeField] private Transform stageMainTypeContainer;
        [SerializeField] private GameObject stageTypeIconPrefab;
        [SerializeField] private StageMainTypeListSO mainTypeList;
        [SerializeField] private StageSubTypeListSO subTypeList;
        [SerializeField] private UnityEngine.UI.Outline outline;
        [SerializeField] private Material lineMaterial;


        public Transform StageInfoContainer;
        [SerializeField] private SelectedStageInfo selectedStageInfoPrefab;
        private SelectedStageInfo selectedStageInfo;
        private bool isMouseOver = false;
        private Tween tween;
        public void Initialize()
        {
            btn = GetComponent<Button>();
            
            
            iconImage = GetComponent<Image>();

            rectTransform = GetComponent<RectTransform>();
            stageSubTypes = new List<StageSubType>();
            SetStageMainType(StageType.Battle);
            isActive = true;
            lineRenderer = new List<UILineRenderer>();
            
        }
        private void Update()
        {
            if(IsInteractable() && !isMouseOver)
            {
                transform.localScale = tweenReference.localScale;
            }
        }

        private void OnDestroy()
        {
            tween?.Kill();
        }

        public void LineToNextBtn(Transform lineRendererCotainer, StageBtn[] next)
        {
            for (int i = 0; i < next.Length; i++)
            {
                if(i > lineRenderer.Count - 1)
                    lineRenderer.Add(Instantiate(lineRendererPrefab, lineRendererCotainer));

                if(next.Length == 1 && next[0].isSelected)
                {
                    lineRenderer[i].color = new Color(0.5f, 0.5f, 0.5f, 1f);
                    lineRenderer[i].material = null;
                }
                else if(tween == null)
                {
                    lineMaterial.SetTextureOffset("_MainTex", Vector2.zero);
                    tween = lineMaterial.DOOffset(new Vector2(-2f, 0), 2f).SetEase(Ease.Linear).SetLoops(-1, LoopType.Incremental);
                }
                lineRenderer[i].rectTransform.anchoredPosition = Vector2.zero;

                Vector2[] points = new Vector2[2];

                RectTransformUtility.ScreenPointToLocalPointInRectangle(lineRenderer[i].rectTransform, GetIconPosition(), null, out points[0]);
                RectTransformUtility.ScreenPointToLocalPointInRectangle(lineRenderer[i].rectTransform, next[i].GetIconPosition(), null, out points[1]);
                lineRenderer[i].Points = points;

                lineRenderer[i].SetAllDirty();
            }
        }

        public List<StageSubType> GetStageSubTypes()
        {
            return stageSubTypes;
        }

        public StageType GetStageMainType()
        {
            return stageMainType;
        }

        public Vector2 GetIconPosition()
        {
            return iconImage.transform.position;
        }

        public Vector2 GetIconSize()
        {
            return rectTransform.sizeDelta;
        }

        public Color GetIconColor()
        {
            return iconImage.color;
        }

        public bool GetIsSelected() { return isSelected; }
        public bool IsInteractable()
        {
            return isInteractable;
            
        }

        public void SetStageMainType(StageType type)
        {
            Image iconObj;
            stageMainType = type;

            if (stageMainTypeContainer.childCount > 0)
            {
                iconObj = stageMainTypeContainer.GetChild(0).GetComponent<Image>();
            }
            else
            {
                iconObj = Instantiate(stageTypeIconPrefab, stageMainTypeContainer).GetComponent<Image>();

            }

            iconObj.GetComponent<RectTransform>().sizeDelta = new Vector2 (80, 80);
            iconObj.sprite = mainTypeList.mainTypes[(int)type].icon;
        }

        public void AddStageSubType(StageSubType type)
        {
            if (!stageSubTypes.Contains(type))
            {

                stageSubTypes.Add(type);
                Image iconObj = Instantiate(stageTypeIconPrefab, stageSubTypesContainer).GetComponent<Image>();
                iconObj.GetComponent<RectTransform>().sizeDelta = new Vector2(70, 70);
                iconObj.sprite = subTypeList.subTypes[(int)type].icon;
            }
            else
                Debug.LogWarning("이미 해당 스테이지 타입이 존재합니다.");
        }

        public void SetStageTypesActive(bool v)
        {
            stageMainTypeContainer.gameObject.SetActive(v);
            stageSubTypesContainer.gameObject.SetActive(v);
        }

        public void SetIcon(Sprite icon)
        {
            iconImage.sprite = icon;
        }

        public void SetIconSizeAndColor(Vector2 size, Color color)
        {
            rectTransform.sizeDelta = size;
            iconImage.color = color;
        }

        public void SetIsSelected(bool isSelected)
        {
            this.isSelected = isSelected;
        }


        public void AddClickEvent(UnityAction action)
        {
            btn.onClick.AddListener(action);
        }

        public void RemoveClickEvent(UnityAction action) { btn.onClick.RemoveListener(action); }

        public void ActiveBtn(bool isActive)
        {
            btn.interactable = isActive;
            //gameObject.SetActive(isActive);
            isInteractable = isActive;

            

        }

        public void SetInteractable(bool v)
        {
            Debug.Log($"SetInteractable: {v}");
            isInteractable = v;
            btn.interactable = v;
        }

        public void SetActive(bool v)
        {
            btn.interactable = v;
            isInteractable = v;
            iconImage.color = new Color(iconImage.color.r, iconImage.color.g, iconImage.color.b, v ? 1f : 0f);
            stageMainTypeContainer.gameObject.SetActive(v);
            stageSubTypesContainer.gameObject.SetActive(v);
            
            isActive = v;

            if (isActive && isInteractable)
            {
                transform.localScale = Vector3.one;

            }
        }

        public void DeleteLineRenderer(int idx)
        {
            if(lineRenderer.Count <= 0) return;
            for(int i = 0; i < lineRenderer.Count; i++)
            {
                if (i == idx) continue;
                lineRenderer[i].gameObject.SetActive(false);

            }
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            if (!btn.interactable) return;

            isMouseOver = true;
            outline.effectColor = new Color(0f, 0f, 0f, 1f);

            transform.localScale = Vector3.one * 1.1f;
            StageInfoContainer.position = transform.position + new Vector3(0f, 100f,0);

            var containerRect = StageInfoContainer.GetComponent<RectTransform>();

            if (selectedStageInfo == null)
            {
                float width = 0f;
                float height = 0f;

                selectedStageInfo = Instantiate(selectedStageInfoPrefab, StageInfoContainer);
                var mainTypeData = mainTypeList.mainTypes[(int)stageMainType];

                var rect = selectedStageInfo.GetComponent<RectTransform>();
                width = rect.sizeDelta.x;
                height += rect.sizeDelta.y;

                selectedStageInfo.SetInfo(mainTypeData.icon, mainTypeData.mainTypeName, mainTypeData.mainTypeDescription);
                for(int i = 0; i < stageSubTypes.Count; i++)
                {
                    selectedStageInfo = Instantiate(selectedStageInfoPrefab, StageInfoContainer);
                    var subTypeData = subTypeList.subTypes[(int)stageSubTypes[i]];
                    selectedStageInfo.SetInfo(subTypeData.icon, subTypeData.subTypeName, subTypeData.subTypeDescription);

                    height += selectedStageInfo.GetComponent<RectTransform>().sizeDelta.y;

                }

                //Vector2 pos = Input.mousePosition;



                //if (pos.x + width > Screen.width)
                //    pos.x -= width;

                //if (pos.y + height > Screen.height)
                //    pos.y -= height;
                

                //containerRect.anchoredPosition = pos;
            }

        }

        public void OnPointerExit(PointerEventData eventData)
        {
            if (!btn.interactable) return;

            isMouseOver = false;
            outline.effectColor = new Color(0f, 0f, 0f, 0.5f);


            for (int i = 0; i < StageInfoContainer.childCount; i++)
            {
                Destroy(StageInfoContainer.GetChild(i).gameObject);
            }
            selectedStageInfo = null;
        }
    }
}