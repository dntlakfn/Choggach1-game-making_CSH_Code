using CIW.Code.FSM;
using UnityEngine;

[CreateAssetMenu(fileName = "StageSubTypeListSO", menuName = "SO/StageUI/SubTypeList")]
public class StageSubTypeListSO : ScriptableObject
{
    public string enumName;
    public StageSubTypeDataSO[] subTypes;
}
