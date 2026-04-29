using System.IO;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;
using System.Linq;
using YIS.Code.Defines;

namespace Work.CSH.Code.Enums.Editors
{
    [CustomEditor(typeof(StageMainTypeListSO))]
    public class StageMainTypeListSOEditor : UnityEditor.Editor
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
            StageMainTypeListSO list = target as StageMainTypeListSO;

            int index = 0;
            string enumString = string.Join(", ", list.mainTypes.Select(so =>
            {
                so.stageMainTypeIndex = index;
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
}