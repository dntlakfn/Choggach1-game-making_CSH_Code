using UnityEngine;

[CreateAssetMenu(fileName = "MainType_", menuName = "SO/StageUI/MainTypeData")]
public class StageMainTypeDataSO : ScriptableObject
{
    public string enumName;
    public int stageMainTypeIndex;
    public Sprite icon;
    [Header("ToolTip")]
    public string mainTypeName;
    [TextArea] public string mainTypeDescription;
}
