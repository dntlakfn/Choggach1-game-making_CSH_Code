
using CIW.Code.Player.Field;
using CSH.UI.StageUI;
using DG.Tweening;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Work.CSH.Code.Enums;
using Work.PSB.Code.CoreSystem;
using Random = UnityEngine.Random;


namespace Work.Scripts.UI
{

    public enum StageChapter
    {
        forest = 0,
    }

    [Serializable]
    public struct StageMapData
    {
        public Sprite icon;
        public StageSubType[] stageSubTypes; 
    }

    [DefaultExecutionOrder(50)]
    public class StageUI : FieldUiCompo
    {
        [SerializeField] private RectTransform content;
        [SerializeField] private GameObject stageBtnPrefab;
        [SerializeField] private GameObject stageLinePrefab;
        [SerializeField] private StageUIDataSO stageDataSO;
        [SerializeField] private StageUIIconSpriteDataListSO[] mapSpriteList;

        [SerializeField] private StageUISettingDataSO settingData;

        private int chestStageCount = 0;
        private int restStageCount = 0;
        private StageBtn[][] stageBtns;

        [Header("etc")]
        [SerializeField] private GameObject lineRenderers;
        [SerializeField] private ScrollRect scrollRect;
        [SerializeField] private TransitionController transitionController;
        [SerializeField] private PlayerFieldInputSO playerFieldInputSO;
        [SerializeField] private Image playerImage;
        [SerializeField] private Transform stageInfoContainer;
        [SerializeField] private CloudSpawner cloudSpawner;
        [SerializeField] private Transform stageBtnTweenReference;

        private Transform playerPoint;
        private float verticalScrollValue = 0;
        private StageChapter currentChapter = StageChapter.forest;
        private bool isOpened = false;
        private RectTransform rectTransform;

        private int stageLineCount => settingData.stageLineCount;
        private int stageNodeCount => settingData.stageNodeCount;

        private void Awake()
        {
            stageDataSO.Initialize();
            rectTransform = GetComponent<RectTransform>();
            gameObject.SetActive(false);
        }
        private void InitStage()
        {
            if (isOpened)
                return;

            playerImage.gameObject.SetActive(false);
            playerFieldInputSO.DisableInput();

            stageDataSO.Initialize();
            content.sizeDelta = new Vector2(1935f, 500f * (stageLineCount + 1));
            stageBtnTweenReference.DOScale(stageBtnTweenReference.localScale * 1.1f, 1f).SetEase(Ease.OutBack).SetLoops(-1, LoopType.Yoyo);

            if (stageDataSO.LoadData() == null)
                CreateStageMap();
            else
                LoadStageMap(stageDataSO.LoadData());


            ConnectStageBtns();
            UpdateBtns();

            rectTransform.DOAnchorPosX(0f, 0.5f).SetEase(Ease.OutQuart);
            playerImage.gameObject.SetActive(true);

            playerImage.transform.position = playerPoint.position;
            scrollRect.verticalNormalizedPosition = verticalScrollValue;
            isOpened = true;
        }
        private void OnEnable()
        {
            InitStage();

            if (isOpened && OnPopUpEvent.Invoke(weight, this))
                OnOpenPanel();
            else
                OnClosePanel();
        }
        private void OnDestroy()
        {
            stageBtnTweenReference.DOKill();
        }
        public override void PopDown()
        {
            base.PopDown();
            OnClosePanel();
        }
        private void OnOpenPanel()
        {
            playerFieldInputSO.DisableInput();

            rectTransform.DOAnchorPosX(0f, 0.5f).SetEase(Ease.OutQuart);
        }
        private void OnClosePanel()
        {
            rectTransform.DOAnchorPosX(-3000f, 0.5f).SetEase(Ease.OutQuart).OnComplete(() =>
            {
                playerFieldInputSO.EnableInput();
                gameObject.SetActive(false);
            });
        }

        public void CreateStageMap()
        {
            stageBtns = new StageBtn[stageLineCount+1][];
            for (int i = 1; i < content.childCount; i++)
            {
                Destroy(content.GetChild(i));
            }

            for(int i = 0; i <= stageLineCount; i++)
            {
                RectTransform tr = Instantiate(stageLinePrefab, content.transform).GetComponent<RectTransform>();
                tr.sizeDelta = new Vector2(500 * stageNodeCount, 150);

                if (i == 0)
                {
                    stageBtns[i] = new StageBtn[1];
                    StageBtn btn = CreateStageNode(tr, i, 0, StageType.Start, new Vector2(300, 300), Color.white);
                    btn.ActiveBtn(false);
                    btn.SetIsSelected(true);

                    playerPoint = btn.transform;
                }
                else if (i == settingData.miniBossStageFloor)
                {
                    stageBtns[i] = new StageBtn[1];
                    StageBtn btn = CreateStageNode(tr, i, 0, StageType.MiniBoss, new Vector2(400, 400), Color.yellow);

                }
                else if (i == settingData.bossStageFloor)
                {
                    stageBtns[i] = new StageBtn[1];
                    StageBtn btn = CreateStageNode(tr, i, 0, StageType.Boss, new Vector2(500, 500), new Color(1f, 0.6f, 0.6f));
                }
                else
                {
                    stageBtns[i] = new StageBtn[stageNodeCount];
                    for (int j = 0; j < stageNodeCount; j++)
                    {
                        StageBtn btn = CreateStageNode(tr, i, j, StageType.Battle, new Vector2(300, 300), Color.white);

                        if (i == settingData.eliteStageFloor)
                            btn.SetStageMainType(StageType.Elite);
                    }
                }
            }

            SettingChestAndRestStage();
            AddBtnEvents();
            cloudSpawner.transform.SetParent(content.transform);
            cloudSpawner.AwakeCloudSpawner();

            scrollRect.verticalNormalizedPosition = 0;
        }
        public void SettingChestAndRestStage()
        {
            Debug.Assert((settingData.minNonBattleRoomAppearFloor + 1) < stageLineCount, "minNonBattleRoomAppearFloor is too high. Please check the value.");
            List<int> line = new List<int>();
            for (int j = settingData.minNonBattleRoomAppearFloor; j < stageLineCount; j++)
            {
                line.Add(j - 1);
            }
            line.Remove(settingData.miniBossStageFloor);
            line.Remove(settingData.miniBossStageFloor - 1);



            for (int i = 0; i < (settingData.maxChestStageCount + settingData.maxShopStageCount); i++)
            {
                int a = line[Random.Range(0, line.Count)];

                List<int> rows = new List<int>();

                for (int j = 0; j < stageBtns[a].Length; j++)
                {
                    if (stageBtns[a][j].GetStageMainType() != StageType.Battle)
                    {
                        rows.Add(i);
                    }
                }
                int b = RandomNum(0, stageBtns[a].Length, rows.ToArray());


                if (chestStageCount < settingData.maxChestStageCount)
                {
                    stageBtns[a][b].SetStageMainType(StageType.Gift);
                    chestStageCount++;
                }
                else if (restStageCount < settingData.maxShopStageCount)
                {
                    stageBtns[a][b].SetStageMainType(StageType.Shop);
                    restStageCount++;
                }

            }

            if (stageLineCount >= 2)
            {
                foreach (var a in stageBtns[settingData.bossStageFloor - 1])
                {
                    a.SetStageMainType(StageType.Shop);
                }
            }
            if (stageLineCount > settingData.miniBossStageFloor)
            {
                foreach (var a in stageBtns[settingData.miniBossStageFloor - 1])
                {
                    a.SetStageMainType(StageType.Shop);
                }
            }


        }
        public void ConnectStageBtns()
        {
            bool notSelected = false;
            for (int i = 1; i < stageBtns.Length - 1; i++)
            {
                int selectedIdx = -1;
                for (int k = 0; k < stageBtns[i].Length; k++)
                {
                    if (stageBtns[i][k].GetIsSelected())
                    {
                        selectedIdx = k;
                        playerImage.transform.position = stageBtns[i][k].GetIconPosition();
                        playerPoint = stageBtns[i][k].transform;
                        break;
                    }
                }
                if (selectedIdx == -1) { notSelected = true; }
                if (!notSelected)
                {
                    int e = 1;
                    for (int j = 0; j < stageBtns[i+1].Length; j++)
                    {
                        if (stageBtns[i+1][j].GetIsSelected()) // 이미 선택된 버튼이 있는 경우
                        {
                            stageBtns[i][selectedIdx].LineToNextBtn(lineRenderers.transform, new StageBtn[1] { stageBtns[i+1][j] });
                            stageBtns[i + 1][j].ActiveBtn(false);
                            break;

                        }
                        else if (e == stageBtns[i+1].Length) // 줄 1개를 다 돌았는데도 선택된 버튼이 없는 경우
                        {
                            stageBtns[i][selectedIdx].LineToNextBtn(lineRenderers.transform, stageBtns[i + 1]);
                            notSelected = true;
                        }
                        else
                        {
                            e++;
                        }
                    }
                }
                else
                {
                    for (int j = 0; j < stageBtns[i + 1].Length; j++)
                    {
                        stageBtns[i + 1][j].SetStageTypesActive(false);
                        stageBtns[i + 1][j].ActiveBtn(false);
                    }
                }

            }
        }
        public void UpdateBtns()
        {
            bool notSelected = false;
            for (int i = 0; i < stageBtns.Length - 1; i++)
            {
                int selectedIdx = -1;
                for (int k = 0; k < stageBtns[i].Length; k++)
                {
                    if (stageBtns[i][k].GetIsSelected())
                    {
                        selectedIdx = k;
                        break;
                    }
                }
                if (selectedIdx == -1) { notSelected = true; }
                if (!notSelected)
                {
                    int e = 1;
                    for (int j = 0; j < stageBtns[i + 1].Length; j++)
                    {
                        if (stageBtns[i + 1][j].GetIsSelected())
                        {
                            stageBtns[i][selectedIdx].LineToNextBtn(lineRenderers.transform, new StageBtn[1] { stageBtns[i + 1][j] });
                            break;

                        }
                        else if (e == stageBtns[i + 1].Length)
                        {
                            stageBtns[i][selectedIdx].LineToNextBtn(lineRenderers.transform, stageBtns[i + 1]);
                            notSelected = true;
                        }
                        else
                        {
                            e++;
                        }
                    }

                }

            }
        }
        public void AddBtnEvents()
        {
            for (int i = 0; i < stageBtns.Length; i++)
            {
                for (int j = 0; j < stageBtns[i].Length; j++)
                {
                    if (i == 1)
                    {
                        stageBtns[i][j].SetInteractable(true);
                        stageBtns[i][j].ActiveBtn(true);
                    }
                    else
                    {
                        stageBtns[i][j].SetInteractable(false);
                    }

                    int _i = i;
                    int _j = j;
                    int rand;
                    StageType btnStageType = stageBtns[_i][_j].GetStageMainType();

                    stageBtns[_i][_j].AddClickEvent(() =>
                    {
                        playerImage.transform.DOMove(stageBtns[_i][_j].GetIconPosition() + new Vector2(0, 2), 0.3f);

                        stageBtns[_i][_j].SetIsSelected(true);
                        stageBtns[_i][_j].ActiveBtn(false);
                        for (int k = 0; k < stageBtns[_i].Length; k++)
                        {
                            int _k = k;
                            if (_j == _k) continue;
                            stageBtns[_i][_k].SetInteractable(false);
                            stageBtns[_i][_k].SetIsSelected(false);
                            stageBtns[_i][_k].SetActive(false);

                        }
                        for (int k = 0; k < stageBtns[_i - 1].Length; k++)
                        {
                            stageBtns[_i - 1][k].DeleteLineRenderer(_j);
                        }

                    });

                    if (btnStageType != StageType.Boss)
                    {
                        stageBtns[_i][_j].AddClickEvent(() =>
                        {
                            for (int e = 0; e < stageBtns[_i + 1].Length; e++)
                            {
                                stageBtns[_i + 1][e].isInteractable = true;
                                stageBtns[_i + 1][e].isActive = true;
                            }

                        });
                    }

                    string sceneNumberStr = string.Empty;
                    foreach (char c in SceneManager.GetActiveScene().name.Split('_').Last())
                    {
                        if (c > '9' || c < '0') continue;
                        sceneNumberStr += c;
                    }

                    rand = RandomNum(1, settingData.GetMapCountByStageType(btnStageType) + 1, sceneNumberStr != string.Empty ? new int[] { int.Parse(sceneNumberStr) } : null);

                    stageBtns[_i][_j].sceneIdx = rand;
                    int sceneIdx = rand;
                    stageBtns[_i][_j].AddClickEvent(() => SaveStageMap());


                    SetBtnMoveSceneEvent(_i, _j, btnStageType, sceneIdx);

                }
            }
        }
        public void LoadBtnEvents(int _i, int _j, int sceneIdx)
        {
            stageBtns[_i][_j].AddClickEvent(() =>
            {
                playerImage.transform.DOMove(stageBtns[_i][_j].GetIconPosition() + new Vector2(0, 2), 0.3f);
                stageBtns[_i][_j].SetIsSelected(true);
                stageBtns[_i][_j].ActiveBtn(false);
                for (int k = 0; k < stageBtns[_i].Length; k++)
                {
                    int _k = k;
                    if (_j == _k) continue;
                    stageBtns[_i][_k].SetInteractable(false);
                    stageBtns[_i][_k].SetIsSelected(false);
                    stageBtns[_i][_k].SetActive(false);

                }
                for (int k = 0; k < stageBtns[_i - 1].Length; k++)
                {
                    stageBtns[_i - 1][k].DeleteLineRenderer(_j);
                }
            });


            if (stageBtns[_i][_j].GetStageMainType() != StageType.Boss)
            {
                stageBtns[_i][_j].AddClickEvent(() =>
                {
                    for (int e = 0; e < stageBtns[_i + 1].Length; e++)
                    {
                        stageBtns[_i + 1][e].isInteractable = true;
                        stageBtns[_i + 1][e].isActive = true;

                    }

                });
            }

            stageBtns[_i][_j].AddClickEvent(() => SaveStageMap());

            StageType btnStageType = stageBtns[_i][_j].GetStageMainType();

            SetBtnMoveSceneEvent(_i, _j, btnStageType, sceneIdx);




        }

        public void SaveStageMap()
        {
            stageDataSO.stageBtnDatas = new List<List<StageBtnData>>();
            for (int i = 0; i < stageBtns.Length; i++)
            {
                List<StageBtnData> lineData = new List<StageBtnData>();
                for (int j = 0; j < stageBtns[i].Length; j++)
                {
                    StageBtn btn = stageBtns[i][j];
                    StageBtnData data = new StageBtnData();
                    data.stageType = btn.GetStageMainType();
                    data.iconSize = btn.GetIconSize();
                    data.iconColor = btn.GetIconColor();
                    data.isInteractable = btn.isInteractable;
                    data.isSelected = btn.GetIsSelected();
                    data.isActive = btn.isActive;
                    data.sceneIdx = btn.sceneIdx;
                    lineData.Add(data);
                }
                stageDataSO.stageBtnDatas.Add(lineData);
            }
            stageDataSO.SaveData(stageDataSO.stageBtnDatas, scrollRect.verticalNormalizedPosition);
        }
        public void LoadStageMap(StageDataWrapper stageBtnDatas)
        {
            stageBtns = new StageBtn[stageLineCount + 1][];
            for (int i = 1; i < content.childCount; i++)
            {
                Destroy(content.GetChild(i));
            }

            for (int i = 0; i < stageBtnDatas.allStages.Count; i++)
            {
                RectTransform tr = Instantiate(stageLinePrefab, content.transform).GetComponent<RectTransform>();

                tr.sizeDelta = new Vector2(500 * stageBtnDatas.allStages[i].row.Count, 100);

                stageBtns[i] = new StageBtn[stageBtnDatas.allStages[i].row.Count];

                for (int j = 0; j < stageBtnDatas.allStages[i].row.Count; j++)
                {
                    StageBtnData data = stageBtnDatas.allStages[i].row[j];
                    var btn = CreateStageNode(tr, i, j, data.stageType, data.iconSize, data.iconColor);
                    btn.sceneIdx = data.sceneIdx;


                    for (int k = 0; k < data.stageSubTypes.Count; k++)
                    {
                        btn.AddStageSubType(data.stageSubTypes[k]);
                    }
                    btn.SetActive(data.isActive);

                    btn.SetIsSelected(data.isSelected);
                    btn.SetInteractable(data.isInteractable);

                    if (data.stageType == StageType.Start)
                    {
                        playerPoint = stageBtns[i][j].transform;

                        btn.ActiveBtn(false);
                    }
                    else
                    {
                        LoadBtnEvents(i, j, btn.sceneIdx);
                    }
                }
            }

            cloudSpawner.transform.SetParent(content.transform);
            cloudSpawner.AwakeCloudSpawner();
            verticalScrollValue = stageBtnDatas.scrollValue;
            scrollRect.verticalNormalizedPosition = verticalScrollValue;
        }
        public void ChangeChapter(StageChapter newChapter)
        {
            currentChapter = newChapter;
            CreateStageMap();
        }

        private StageBtn CreateStageNode(RectTransform parent, int i, int j, StageType stageType, Vector2 size, Color color)
        {
            StageBtn btn = Instantiate(stageBtnPrefab, parent).GetComponent<StageBtn>();

            btn.Initialize();
            btn.SetStageMainType(stageType);
            btn.SetIconSizeAndColor(size, color);
            btn.StageInfoContainer = stageInfoContainer;
            btn.tweenReference = stageBtnTweenReference;
            
            stageBtns[i][j] = btn;

            return btn;
        }
        private void SetBtnMoveSceneEvent(int i, int j, StageType type, int sceneIdx)
        {
            Debug.Log($"Setting move scene event for button at line {i}, node {j} with stage type {type} and scene index {sceneIdx}");
            StageMapData stageMapData = mapSpriteList[(int)currentChapter].GetStageMapDataByType(type)[sceneIdx - 1];
            stageBtns[i][j].SetIcon(stageMapData.icon);
            for (int k = 0; k < stageMapData.stageSubTypes.Length; k++)
                stageBtns[i][j].AddStageSubType(stageMapData.stageSubTypes[k]);

            stageBtns[i][j].AddClickEvent(() => { playerImage.transform.DOMove(stageBtns[i][j].GetIconPosition() + new Vector2(0, 2), 0.3f).OnComplete(() => MoveMap($"Map_{currentChapter}_{type.ToString()}_{sceneIdx}")); });

        }
        private void SetBtnMoveSceneEvent(StageBtn btn, StageType type, int sceneIdx)
        {
            StageMapData stageMapData = mapSpriteList[(int)currentChapter].GetStageMapDataByType(type)[sceneIdx - 1];

            btn.SetIcon(stageMapData.icon);
            for (int k = 0; k < stageMapData.stageSubTypes.Length; k++)
                btn.AddStageSubType(stageMapData.stageSubTypes[k]);

            if (type == StageType.Battle)
                btn.AddClickEvent(() => { MoveMap($"Map_{currentChapter}_{sceneIdx}"); });
            else
                btn.AddClickEvent(() => { MoveMap($"Map_{currentChapter}_{type.ToString()}{sceneIdx}"); });
        }
        private int RandomNum(int minInclusive, int maxExclusive, int[] exceptNums = null)
        {
            List<int> candidates = new List<int>();

            for (int i = minInclusive; i < maxExclusive; i++)
            {
                
                if (exceptNums != null && Array.Exists(exceptNums, x => x == i)) continue;
                candidates.Add(i);
            }

            if (candidates.Count == 0) return minInclusive;

            return candidates[Random.Range(0, candidates.Count)];
        }
        private void MoveMap(string sceneName)
        {
            transitionController.nextScene = sceneName;

            transitionController.Transition();
        }

    }
}