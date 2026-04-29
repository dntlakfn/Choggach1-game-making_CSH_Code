using UnityEngine;
using Work.CSH.Code.Enums;

namespace CSH.UI.StageUI
{
    [CreateAssetMenu(fileName = "MapCountDataSO", menuName = "SO/StageUI/Data/StageUISetting")]
    public class StageUISettingDataSO : ScriptableObject
    {
        [Header("MapCount")]
        public int battleMapCount; // 얘 최소 1 이상이여야함
        public int eliteMapCount;
        public int shopMapCount;
        public int bossMapCount;
        public int miniBossMapCount;
        public int giftMapCount;

        [Header("StageNode Setting")]
        public int stageLineCount = 12;
        public int stageNodeCount = 4;

        public int minNonBattleRoomAppearFloor = 0;
        public int maxChestStageCount;
        public int maxShopStageCount;
        public int eliteStageFloor;
        public int miniBossStageFloor;
        public int bossStageFloor;


        public int GetMapCountByStageType(StageType stageType)
        {
            return stageType switch
            {
                StageType.Battle => battleMapCount,
                StageType.Elite => eliteMapCount,
                StageType.Shop => shopMapCount,
                StageType.Boss => bossMapCount,
                StageType.MiniBoss => miniBossMapCount,
                StageType.Gift => giftMapCount,
                _ => 1
            };
        }
    }
}