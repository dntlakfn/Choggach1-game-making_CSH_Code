using UnityEngine;

[CreateAssetMenu(fileName = "StageMainTypeList", menuName = "SO/StageUI/MainTypeList")]
public class StageMainTypeListSO : ScriptableObject
{
    public string enumName;
    public StageMainTypeDataSO[] mainTypes;
}
