"""
Layout Sync Tool for Maya - インストールスクリプト
"""
import os
import shutil
import sys
import platform


def get_maya_scripts_dir():
    """Mayaスクリプトディレクトリを取得"""
    system = platform.system()
    
    if system == "Windows":
        base = os.path.expanduser("~/Documents/maya")
    elif system == "Darwin":  # Mac
        base = os.path.expanduser("~/Library/Preferences/Autodesk/maya")
    else:  # Linux
        base = os.path.expanduser("~/maya")
    
    # 最新バージョンを探す
    if os.path.exists(base):
        versions = [d for d in os.listdir(base) if d.isdigit()]
        if versions:
            latest = max(versions)
            scripts_dir = os.path.join(base, latest, "scripts")
            if os.path.exists(scripts_dir):
                return scripts_dir
    
    # デフォルト
    return os.path.join(base, "scripts")


def install():
    """インストール実行"""
    source_dir = os.path.join(os.path.dirname(__file__), "layout_sync_maya")
    
    if not os.path.exists(source_dir):
        print("エラー: layout_sync_mayaフォルダが見つかりません")
        return False
    
    target_base = get_maya_scripts_dir()
    target_dir = os.path.join(target_base, "layout_sync_maya")
    
    print(f"インストール先: {target_dir}")
    
    # ディレクトリ作成
    os.makedirs(target_base, exist_ok=True)
    
    # 既存ファイルがあれば削除
    if os.path.exists(target_dir):
        response = input(f"{target_dir} は既に存在します。上書きしますか? (y/n): ")
        if response.lower() != 'y':
            print("インストールをキャンセルしました")
            return False
        shutil.rmtree(target_dir)
    
    # コピー
    shutil.copytree(source_dir, target_dir)
    
    print("✓ インストール完了")
    print("\nMayaで以下を実行してください:")
    print("  import layout_sync_maya")
    print("  layout_sync_maya.show()")
    
    return True


if __name__ == "__main__":
    install()