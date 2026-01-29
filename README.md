# Layout Sync Tool

MayaなどのレイアウトデータをUnityへ転送するツールです。

## 概要

背景モデラー/レイアウトアーティストがMayaで作成した3D背景レイアウトを、
Unity側で簡単にインポート・配置できるツールです。

## 対応環境

- **Maya**: 2023以降 (Python 3.x)
- **Unity**: 2021.3 LTS以降

## クイックスタート

### Maya側
```bash
# Mayaスクリプトディレクトリにインストール
python maya/install.py
```

詳細: [Maya側セットアップ](maya/README.md)

### Unity側
```
1. Package Managerを開く
2. "Add package from disk..."を選択
3. unity/com.actoratect.layout-sync/package.jsonを選択
```

詳細: [Unity側セットアップ](unity/README.md)

## ワークフロー

### Maya → Unity

1. Mayaでレイアウトを作成
2. Layout Sync Toolを起動
3. JSONエクスポート
4. UnityでJSONをインポート
5. シーンに自動配置

詳細: [使い方ガイド](docs/getting-started.md)

## ドキュメント

- [docs/getting-started.md](docs/getting-started.md)
- [docs/maya-guide.md](docs/maya-guide.md)
- [docs/unity-guide.md](docs/unity-guide.md)
- [docs/coordinate-systems.md](docs/coordinate-systems.md)
- [shared/SPECIFICATION.md](shared/SPECIFICATION.md)
- [shared/schemas/layout-v1.0.schema.json](shared/schemas/layout-v1.0.schema.json)

## ライセンス

MIT License

## バージョン履歴

- **v0.1.0** (2026-01-30): Phase 1 - Maya→Unity基本機能