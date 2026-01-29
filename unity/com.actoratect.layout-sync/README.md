# Layout Sync Tool - Unity

## インストール

### Package Managerからインストール

1. Unityエディタを開く
2. Window > Package Manager
3. 左上の「+」> Add package from disk...
4. `com.actoratect.layout-sync/package.json`を選択

## 使い方

### ツールを起動
```
Window > Actoratect > Layout Sync Tool
```

### Maya→Unity インポート

1. MayaでエクスポートしたJSONファイルを選択
2. 「JSONを読み込み」ボタンをクリック
3. 「シーンに配置」ボタンをクリック

### 親オブジェクト指定（オプション）

配置したオブジェクトを特定のTransform配下に配置したい場合:
- 「親オブジェクト」フィールドにHierarchyからドラッグ&ドロップ

## 座標系について

Mayaの座標系（Y-up, Right-handed, cm）から
Unity座標系（Y-up, Left-handed, m）に自動変換されます。

詳細: [座標系変換について](../docs/coordinate-systems.md)

## トラブルシューティング

### Prefabが見つからない

JSONの`modelPath.unity`に指定されたパスに
Prefabが存在するか確認してください。

例: `"unity": "Assets/Prefabs/Buildings/Building_01.prefab"`

### 座標がおかしい

- Mayaのワールド座標で配置されているか確認
- スケールが極端に大きい/小さい場合、単位設定を確認