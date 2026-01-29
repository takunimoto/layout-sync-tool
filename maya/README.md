# Layout Sync Tool - Maya

## インストール

### 方法1: 自動インストール（推奨）
```bash
cd maya
python install.py
```

### 方法2: 手動インストール

1. `layout_sync_maya`フォルダを以下のいずれかにコピー:
   - Windows: `C:/Users/<ユーザー名>/Documents/maya/scripts/`
   - Mac: `~/Library/Preferences/Autodesk/maya/scripts/`
   - Linux: `~/maya/scripts/`

2. Mayaを再起動

## 使い方

### ツールを起動

Mayaスクリプトエディタ (Python)で実行:
```python
import layout_sync_maya
layout_sync_maya.show()
```

### シェルフに登録（オプション）

1. 上記コードをスクリプトエディタに入力
2. 選択した状態で、中クリックでシェルフにドラッグ
3. シェルフボタンから起動可能に

## トラブルシューティング

### "No module named 'layout_sync_maya'"

- インストール場所が正しいか確認
- Mayaを再起動

### JSONエクスポートできない

- 書き込み権限を確認
- ファイルパスに日本語が含まれていないか確認