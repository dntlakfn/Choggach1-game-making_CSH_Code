using UnityEngine;
using Work.CSH.Code.Enums;

namespace Work.Scripts.UI
{
    [CreateAssetMenu(menuName = "SO/StageUI/MapIconDataList")]
    public class StageUIIconSpriteDataListSO : ScriptableObject
    {
        public StageMapData[] battleMapIcons;
        public StageMapData[] eliteMapIcons;
        public StageMapData[] giftMapIcon;
        public StageMapData[] shopMapIcon;
        public StageMapData[] miniBossMapIcons;
        public StageMapData[] bossMapIcons;
        public StageMapData[] startMapIcon;

        public StageMapData[] GetStageMapDataByType(StageType stageType)
        {
            return stageType switch
            {
                StageType.Battle => battleMapIcons,
                StageType.Elite => eliteMapIcons,
                StageType.Gift => giftMapIcon,
                StageType.Shop => shopMapIcon,
                StageType.MiniBoss => miniBossMapIcons,
                StageType.Boss => bossMapIcons,
                StageType.Start => startMapIcon,
                _ => null
            };
        }

    }
}