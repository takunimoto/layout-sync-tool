using UnityEngine;
using UnityEditor;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace Actoratect.LayoutSync
{
    public class ImportResult
    {
        public List<GameObject> PlacedObjects { get; } = new List<GameObject>();
        public int MissingPrefabCount { get; set; }
        public int MissingParentCount { get; set; }
    }

    public class JsonImporter
    {
        public LayoutJson LoadJson(string jsonPath)
        {
            if (!File.Exists(jsonPath))
            {
                Debug.LogError($"JSONファイルが見つかりません: {jsonPath}");
                return null;
            }

            string jsonText = File.ReadAllText(jsonPath);
            LayoutJson layoutData = JsonUtility.FromJson<LayoutJson>(jsonText);

            if (layoutData == null)
            {
                Debug.LogError("JSONのパースに失敗しました");
                return null;
            }

            Debug.Log($"レイアウト読み込み: {layoutData.metadata.layoutName} ({layoutData.objects.Count}オブジェクト)");
            return layoutData;
        }

        public ImportResult PlaceInScene(LayoutJson layoutData, Transform parent = null)
        {
            ImportResult result = new ImportResult();
            Dictionary<string, GameObject> pathToObject = new Dictionary<string, GameObject>();
            var ordered = layoutData.objects
                .OrderBy(o => GetPathDepth(o.path))
                .ToList();

            foreach (var obj in ordered)
            {
                string assetName = string.IsNullOrEmpty(obj.assetName) ? obj.name : obj.assetName;
                GameObject prefab = FindPrefabSourceUnderParent(parent, assetName);

                if (prefab == null)
                {
                    prefab = LoadPrefab(obj.modelPath.unity);
                }

                if (prefab == null)
                {
                    Debug.LogWarning($"Prefabが見つかりません: {obj.modelPath.unity} (ID: {obj.id})");
                    result.MissingPrefabCount += 1;
                    continue;
                }

                // Prefabをシーンにインスタンス化
                GameObject instance = (GameObject)PrefabUtility.InstantiatePrefab(prefab);
                instance.name = obj.name;

                Transform targetParent = null;
                if (!string.IsNullOrEmpty(obj.parentPath) && pathToObject.TryGetValue(obj.parentPath, out var parentObj))
                {
                    targetParent = parentObj.transform;
                }
                else if (parent != null)
                {
                    targetParent = parent;
                }
                else if (!string.IsNullOrEmpty(obj.parentPath))
                {
                    result.MissingParentCount += 1;
                }

                if (targetParent != null)
                {
                    instance.transform.SetParent(targetParent, true);
                }

                // 座標変換して配置
                Vector3 mayaPos = obj.transform.GetPosition();
                Vector3 mayaRot = obj.transform.GetRotation();
                Vector3 mayaScale = obj.transform.GetScale();

                instance.transform.position = CoordinateConverter.MayaToUnityPosition(mayaPos);
                instance.transform.eulerAngles = CoordinateConverter.MayaToUnityRotation(mayaRot);
                instance.transform.localScale = mayaScale;

                // Undo登録
                Undo.RegisterCreatedObjectUndo(instance, "Place Layout Object");

                result.PlacedObjects.Add(instance);

                if (!string.IsNullOrEmpty(obj.path))
                {
                    pathToObject[obj.path] = instance;
                }
            }

            Debug.Log($"配置完了: {result.PlacedObjects.Count}個のオブジェクト");
            return result;
        }

        private GameObject LoadPrefab(string prefabPath)
        {
            return AssetDatabase.LoadAssetAtPath<GameObject>(prefabPath);
        }

        private GameObject FindPrefabSourceUnderParent(Transform parent, string assetName)
        {
            if (parent == null || string.IsNullOrEmpty(assetName))
            {
                return null;
            }

            var regex = new Regex($"^{Regex.Escape(assetName)}(_[0-9]+)?$");
            foreach (Transform child in parent.GetComponentsInChildren<Transform>(true))
            {
                if (!regex.IsMatch(child.name))
                {
                    continue;
                }

                var source = PrefabUtility.GetCorrespondingObjectFromSource(child.gameObject);
                if (source != null)
                {
                    return source;
                }

                return child.gameObject;
            }

            return null;
        }

        private int GetPathDepth(string path)
        {
            if (string.IsNullOrEmpty(path))
            {
                return 0;
            }

            return path.Split(new[] { '|' }, System.StringSplitOptions.RemoveEmptyEntries).Length;
        }
    }
}