using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;
using Work.CSH.Code.UIs;
using YIS.Code.Defines;

[CustomEditor(typeof(GuideTextDataSOList))]
public class GuideTextDataSOListEditor : UnityEditor.Editor
{
    [SerializeField] private VisualTreeAsset stateListView = default;

    public override VisualElement CreateInspectorGUI()
    {
        VisualElement root = new VisualElement();
        InspectorElement.FillDefaultInspector(root, serializedObject, this);
        stateListView.CloneTree(root);

        root.Q<Button>("GenerateButton").clicked += HandleGenerateButtonClick;

        return root;
    }

    private void HandleGenerateButtonClick()
    {
        GuideTextDataSOList list = target as  GuideTextDataSOList ;

        int index = 0;
        string enumString = string.Join(", ", list.list.Select(so =>
        {
            so.id = index;
            EditorUtility.SetDirty(so);
            return $"{so.enumName} = {index++}";
        }));

        string scriptPath = AssetDatabase.GetAssetPath(MonoScript.FromScriptableObject(this));
        string dirName = Path.GetDirectoryName(scriptPath);
        DirectoryInfo parentDirectory = Directory.GetParent(dirName);
        string path = parentDirectory.FullName;
        string code = string.Format(CodeFormat.CSHEnumsFormat, list.enumName, enumString);

        File.WriteAllText($"{path}/{list.enumName}.cs", code);

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
    }
}