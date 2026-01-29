using UnityEngine;
using UnityEditor;
using System.IO;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace Actoratect.LayoutSync
{
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

        public List<GameObject> PlaceInScene(LayoutJson layoutData, Transform parent = null)
        {
            List<GameObject> placedObjects = new List<GameObject>();

            foreach (var obj in layoutData.objects)
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
                    continue;
                }

                // Prefabをシーンにインスタンス化
                GameObject instance = (GameObject)PrefabUtility.InstantiatePrefab(prefab);
                instance.name = obj.name;

                if (parent != null)
                {
                    instance.transform.SetParent(parent);
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

                placedObjects.Add(instance);
            }

            Debug.Log($"配置完了: {placedObjects.Count}個のオブジェクト");
            return placedObjects;
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
    }
}