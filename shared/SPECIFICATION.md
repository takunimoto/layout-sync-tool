# Layout Sync Tool - JSON仕様書

Version: 1.0.0

## 概要

Maya ↔ Unity間でレイアウトデータを交換するためのJSON形式の仕様。

## JSONスキーマ

### 基本構造
```json
{
  "version": "1.0.0",
  "metadata": { ... },
  "objects": [ ... ]
}
```

### metadata

| フィールド | 型 | 必須 | 説明 |
|-----------|-----|------|------|
| layoutName | string | ○ | レイアウト名 |
| exportDate | string | ○ | エクスポート日時 (ISO 8601) |
| sourceApp | string | ○ | エクスポート元 ("Maya" or "Unity") |
| coordinate | object | ○ | 座標系情報 |

### coordinate

| フィールド | 型 | 値 |
|-----------|-----|-----|
| upAxis | string | "Y" |
| unit | string | "centimeter" (Maya) / "meter" (Unity) |
| handedness | string | "right" (Maya) / "left" (Unity) |

### objects[]

| フィールド | 型 | 必須 | 説明 |
|-----------|-----|------|------|
| id | string | ○ | 一意識別子 |
| name | string | ○ | オブジェクト名 |
| assetName | string | ○ | アセット名（末尾の_数字を除外） |
| sequence | string | - | 連番（末尾の_数字） |
| modelPath | object | ○ | モデルパス情報 |
| transform | object | ○ | トランスフォーム情報 |

### modelPath

| フィールド | 型 | 必須 | 説明 |
|-----------|-----|------|------|
| maya | string | ○ | Mayaモデルパス |
| unity | string | ○ | Unity Prefabパス |

### transform

| フィールド | 型 | 必須 | 説明 |
|-----------|-----|------|------|
| position | float[3] | ○ | ワールド座標 [x, y, z] |
| rotation | float[3] | ○ | Euler角 [x, y, z] (度) |
| scale | float[3] | ○ | スケール [x, y, z] |

## 完全な例
```json
{
  "version": "1.0.0",
  "metadata": {
    "layoutName": "Stage01",
    "exportDate": "2026-01-30T10:30:00",
    "sourceApp": "Maya",
    "coordinate": {
      "upAxis": "Y",
      "unit": "centimeter",
      "handedness": "right"
    }
  },
  "objects": [
    {
      "id": "building_01_001",
      "name": "Building_01_Instance_001",
      "assetName": "Building_01_Instance",
      "sequence": "001",
      "modelPath": {
        "maya": "model/building_01.ma",
        "unity": "Assets/Prefabs/Buildings/Building_01.prefab"
      },
      "transform": {
        "position": [1050.0, 0.0, 2320.0],
        "rotation": [0.0, 45.0, 0.0],
        "scale": [1.0, 1.0, 1.0]
      }
    }
  ]
}
```

## 座標系変換

### Maya → Unity
```
Position:
  Unity.x = Maya.x * 0.01  (cm→m)
  Unity.y = Maya.y * 0.01
  Unity.z = -Maya.z * 0.01  (反転)

Rotation:
  Unity.x = -Maya.x
  Unity.y = -Maya.y
  Unity.z = Maya.z
```

## 変更履歴

- **v1.0.0** (2026-01-30): 初版リリース