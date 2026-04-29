using UnityEngine;
using Work.CSH.Code.Enums;

namespace Work.CSH.Code.UIs
{
    [CreateAssetMenu(fileName = "GuideTextDataSOList", menuName = "SO/GuideUI/DataList")]
    public class GuideTextDataSOList : ScriptableObject
    {
        public string enumName;
        public GuideTextDataSO[] list;

        public string GetGuideText(GuideTextType type)
        {
            foreach (var data in list)
            {
                if (data.enumName == type.ToString())
                {
                    return data.GuideText;
                }
            }
            Debug.LogWarning($"GuideTextDataSOList: No guide text found for type {type}");
            return string.Empty;
        }
    }
}