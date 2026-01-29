using UnityEngine;
using UnityEditor;
using System.IO;

namespace Actoratect.LayoutSync
{
    public class LayoutSyncWindow : EditorWindow
    {
        private string jsonPath = "";
        private LayoutJson loadedLayout;
        private Transform parentTransform;
        private Vector2 scrollPos;

        private JsonImporter importer = new JsonImporter();

        [MenuItem("Window/Actoratect/Layout Sync Tool")]
        static void Init()
        {
            LayoutSyncWindow window = GetWindow<LayoutSyncWindow>();
            window.titleContent = new GUIContent("Layout Sync Tool");
            window.Show();
        }

        void OnGUI()
        {
            GUILayout.Label("Layout Sync Tool", EditorStyles.boldLabel);
            EditorGUILayout.Space();

            // Maya → Unity セクション
            EditorGUILayout.LabelField("Maya → Unity (インポート)", EditorStyles.boldLabel);
            
            using (new EditorGUILayout.VerticalScope("box"))
            {
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField("JSONファイル:", GUILayout.Width(100));
                jsonPath = EditorGUILayout.TextField(jsonPath);
                
                if (GUILayout.Button("参照...", GUILayout.Width(60)))
                {
                    string selectedPath = EditorUtility.OpenFilePanel("レイアウトJSONを選択", "", "json");
                    if (!string.IsNullOrEmpty(selectedPath))
                    {
                        jsonPath = selectedPath;
                    }
                }
                EditorGUILayout.EndHorizontal();

                EditorGUILayout.Space();

                parentTransform = (Transform)EditorGUILayout.ObjectField(
                    "親オブジェクト (任意):",
                    parentTransform,
                    typeof(Transform),
                    true
                );

                EditorGUILayout.Space();

                if (GUILayout.Button("JSONを読み込み", GUILayout.Height(30)))
                {
                    LoadJsonFile();
                }

                EditorGUILayout.Space();

                // プレビュー表示
                if (loadedLayout != null)
                {
                    EditorGUILayout.LabelField("読み込み済みデータ", EditorStyles.boldLabel);
                    EditorGUILayout.LabelField($"レイアウト名: {loadedLayout.metadata.layoutName}");
                    EditorGUILayout.LabelField($"オブジェクト数: {loadedLayout.objects.Count}");
                    EditorGUILayout.LabelField($"エクスポート日時: {loadedLayout.metadata.exportDate}");

                    EditorGUILayout.Space();

                    if (GUILayout.Button("シーンに配置", GUILayout.Height(35)))
                    {
                        PlaceObjectsInScene();
                    }
                }
            }

            EditorGUILayout.Space(10);

            // Unity → Maya セクション (Phase 2で実装)
            EditorGUILayout.LabelField("Unity → Maya (エクスポート)", EditorStyles.boldLabel);
            using (new EditorGUILayout.VerticalScope("box"))
            {
                EditorGUILayout.HelpBox("Phase 2で実装予定", MessageType.Info);
            }
        }

        private void LoadJsonFile()
        {
            if (string.IsNullOrEmpty(jsonPath))
            {
                EditorUtility.DisplayDialog("エラー", "JSONファイルを選択してください", "OK");
                return;
            }

            loadedLayout = importer.LoadJson(jsonPath);

            if (loadedLayout != null)
            {
                EditorUtility.DisplayDialog(
                    "読み込み完了",
                    $"レイアウト「{loadedLayout.metadata.layoutName}」を読み込みました\nオブジェクト数: {loadedLayout.objects.Count}",
                    "OK"
                );
            }
        }

        private void PlaceObjectsInScene()
        {
            if (loadedLayout == null)
            {
                EditorUtility.DisplayDialog("エラー", "先にJSONを読み込んでください", "OK");
                return;
            }

            var placedObjects = importer.PlaceInScene(loadedLayout, parentTransform);

            EditorUtility.DisplayDialog(
                "配置完了",
                $"{placedObjects.Count}個のオブジェクトをシーンに配置しました",
                "OK"
            );
        }
    }
}