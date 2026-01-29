try:
    import maya.cmds as cmds  # type: ignore
except Exception:  # pragma: no cover - Maya only
    cmds = None
import json
import re
from datetime import datetime


class LayoutExporter:
    """Maya シーンからレイアウトデータをJSON形式でエクスポート"""
    
    def __init__(self):
        self.version = "1.0.0"
    
    def export_selection(self, output_path, layout_name="Untitled"):
        """選択オブジェクトをエクスポート"""
        if cmds is None:
            raise RuntimeError("This module must be run inside Maya.")
        selected = cmds.ls(selection=True, long=True, transforms=True)
        
        if not selected:
            cmds.warning("オブジェクトが選択されていません")
            return 0
        
        layout_data = self._build_layout_data(selected, layout_name)
        self._write_json(layout_data, output_path)
        
        print(f"エクスポート完了: {len(selected)}個のオブジェクト → {output_path}")
        return len(selected)
    
    def export_all(self, output_path, layout_name="Untitled"):
        """シーン全体をエクスポート"""
        if cmds is None:
            raise RuntimeError("This module must be run inside Maya.")
        all_transforms = cmds.ls(type='transform', long=True)
        
        # カメラ・ライト等を除外
        valid_objects = [obj for obj in all_transforms 
                        if not self._is_system_object(obj)]
        
        layout_data = self._build_layout_data(valid_objects, layout_name)
        self._write_json(layout_data, output_path)
        
        print(f"エクスポート完了: {len(valid_objects)}個のオブジェクト → {output_path}")
        return len(valid_objects)
    
    def _build_layout_data(self, objects, layout_name):
        """レイアウトデータ構造を構築"""
        if cmds is None:
            raise RuntimeError("This module must be run inside Maya.")
        layout_objects = []
        
        for obj in objects:
            # トランスフォーム取得
            pos = cmds.xform(obj, query=True, worldSpace=True, translation=True)
            rot = cmds.xform(obj, query=True, worldSpace=True, rotation=True)
            scale = cmds.xform(obj, query=True, relative=True, scale=True)
            
            # ショートネーム取得
            short_name = obj.split('|')[-1]
            asset_name, sequence = self._split_asset_name(short_name)

            parent = cmds.listRelatives(obj, parent=True, fullPath=True)
            parent_path = parent[0] if parent else None
            
            # モデルパス推定 (リファレンスの場合)
            maya_path = self._get_reference_path(obj)
            if not maya_path:
                maya_path = f"model/{short_name}.ma"
            
            obj_data = {
                "id": self._generate_id(obj),
                "name": short_name,
                "assetName": asset_name,
                "sequence": sequence,
                "path": obj,
                "parentPath": parent_path,
                "modelPath": {
                    "maya": maya_path,
                    "unity": f"Assets/Prefabs/{short_name}.prefab"
                },
                "transform": {
                    "position": [round(pos[0], 4), round(pos[1], 4), round(pos[2], 4)],
                    "rotation": [round(rot[0], 4), round(rot[1], 4), round(rot[2], 4)],
                    "scale": [round(scale[0], 4), round(scale[1], 4), round(scale[2], 4)]
                }
            }
            
            layout_objects.append(obj_data)
        
        return {
            "version": self.version,
            "metadata": {
                "layoutName": layout_name,
                "exportDate": datetime.now().isoformat(),
                "sourceApp": "Maya",
                "coordinate": {
                    "upAxis": "Y",
                    "unit": "centimeter",
                    "handedness": "right"
                }
            },
            "objects": layout_objects
        }

    def _split_asset_name(self, short_name):
        """末尾の_数字を連番として分離"""
        match = re.match(r"^(.*)_([0-9]+)$", short_name)
        if match:
            return match.group(1), match.group(2)
        return short_name, None
    
    def _get_reference_path(self, obj):
        """リファレンスファイルのパスを取得"""
        if cmds is None:
            raise RuntimeError("This module must be run inside Maya.")
        if cmds.referenceQuery(obj, isNodeReferenced=True):
            ref_node = cmds.referenceQuery(obj, referenceNode=True)
            ref_file = cmds.referenceQuery(ref_node, filename=True)
            return ref_file
        return None
    
    def _is_system_object(self, obj):
        """システムオブジェクト(カメラ等)を判定"""
        if cmds is None:
            raise RuntimeError("This module must be run inside Maya.")
        shapes = cmds.listRelatives(obj, shapes=True, fullPath=True)
        if shapes:
            shape_type = cmds.nodeType(shapes[0])
            if shape_type in ['camera', 'light']:
                return True
        return False
    
    def _generate_id(self, obj):
        """一意のIDを生成"""
        short_name = obj.split('|')[-1]
        # UUIDの代わりに名前ベースのID生成
        return short_name.replace(':', '_').replace('|', '_')
    
    def _write_json(self, data, output_path):
        """JSONファイルに書き出し"""
        with open(output_path, 'w', encoding='utf-8') as f:
            json.dump(data, f, ensure_ascii=False, indent=2)