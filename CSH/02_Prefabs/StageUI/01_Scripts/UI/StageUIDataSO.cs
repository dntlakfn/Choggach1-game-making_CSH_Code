using System;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using Newtonsoft.Json;
using Work.CSH.Code.FSMSystem;
using Work.CSH.Code.Enums;

namespace Work.Scripts.UI
{
    [System.Serializable]
    public class StageRow { public List<StageBtnData> row; }

    [System.Serializable]
    public class StageDataWrapper { public List<StageRow> allStages; public float scrollValue; }

    [Serializable]
    public struct StageBtnData
    {
        public Color iconColor;
        public Vector2 iconSize;
        public int nextIndex;
        public StageType stageType;
        public List<StageSubType> stageSubTypes;
        public bool isInteractable;
        public bool isSelected;
        public bool isActive;
        public int sceneIdx;
        public StageBtnData(Color iconColor, Vector2 iconSize, int nextIndex, StageType stageType, List<StageSubType> stageSubTypes, bool isInteractable, bool isSelected, bool isActive, int sceneIdx)
        {
            this.iconColor = iconColor;
            this.iconSize = iconSize;
            this.nextIndex = nextIndex;
            this.stageType = stageType;
            this.stageSubTypes = stageSubTypes;
            this.isInteractable = isInteractable;
            this.isSelected = isSelected;
            this.isActive = isActive;
            this.sceneIdx = sceneIdx;
        }

    }
    [CreateAssetMenu(fileName = "StageUIDataSO", menuName = "SO/StageUI/Data/StageUISaveData")]
    public class StageUIDataSO : ScriptableObject
    {
        public List<List<StageBtnData>> stageBtnDatas;

        private string filePath;

        private void Awake()
        {
            Initialize();
        }

        public void Initialize()
        {
            // 저장 경로 설정 (플랫폼마다 안전한 persistentDataPath 권장)
            filePath = Path.Combine(Application.persistentDataPath, "StageData.json");
            stageBtnDatas = new List<List<StageBtnData>>();
            Debug.Log($"저장 경로: {filePath}");
        }

        public void DeleteJson()
        {
            Debug.Log($"저장된 데이터를 삭제합니다: {filePath}");
            if (File.Exists(filePath))
                File.Delete(filePath);
        }

        // JSON으로 저장하는 함수
        public void SaveData(List<List<StageBtnData>> data, float verticalScrollValue)
        {
            StageDataWrapper wrapper = new StageDataWrapper { allStages = new List<StageRow>() };
            foreach (var list in data)
            {
                wrapper.allStages.Add(new StageRow { row = list });
            }
            wrapper.scrollValue = verticalScrollValue;
            string json = JsonUtility.ToJson(wrapper, true);
            File.WriteAllText(filePath, json);
            Debug.Log($"데이터가 저장되었습니다: {filePath}");
        }

        // JSON에서 불러오는 함수
        public StageDataWrapper LoadData()
        {
            if (File.Exists(filePath))
            {
                Debug.Log($"데이터를 불러옵니다: {filePath}");

                // 1. 파일에서 JSON 문자열 읽기
                string json = File.ReadAllText(filePath);

                // 2. JSON 문자열을 구조체로 역직렬화
                return JsonUtility.FromJson<StageDataWrapper>(json);

            }

            Debug.LogWarning("저장된 파일이 없습니다. 기본값을 반환합니다.");
            return null;
        }

    }


  
}