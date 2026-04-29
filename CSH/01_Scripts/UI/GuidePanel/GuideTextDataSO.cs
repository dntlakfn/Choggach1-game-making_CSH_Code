using UnityEngine;

namespace Work.CSH.Code.UIs
{
    [CreateAssetMenu(fileName = "GuideTextDataSO", menuName = "SO/GuideUI/TextData")]
    public class GuideTextDataSO : ScriptableObject
    {
        public int id;
        public string enumName;

        [TextArea] public string GuideText;
    }
}