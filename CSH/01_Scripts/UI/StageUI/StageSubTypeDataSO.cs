using UnityEngine;

[CreateAssetMenu(fileName = "StageSubTypeDataSO", menuName = "SO/StageUI/SubTypeData")]
public class StageSubTypeDataSO : ScriptableObject
{
    public string enumName;
    public int stageSubTypeIndex;
    public Sprite icon;
    [Header("ToolTip")]
    public string subTypeName;
    [TextArea] public string subTypeDescription;
}
 