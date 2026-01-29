try:
    import maya.cmds as cmds  # type: ignore
except Exception:  # pragma: no cover - Maya only
    cmds = None

from . import exporter


class LayoutSyncUI:
    """Layout Sync Tool Maya UIウィンドウ"""
    
    WINDOW_NAME = "layoutSyncWindow"
    WINDOW_TITLE = "Layout Sync Tool"
    
    def __init__(self):
        self.exporter = exporter.LayoutExporter()
        self.output_path = ""
        self.layout_name = "Untitled"
    
    def create(self):
        """UIウィンドウを作成"""
        if cmds is None:
            raise RuntimeError("This module must be run inside Maya.")
        # 既存ウィンドウを削除
        if cmds.window(self.WINDOW_NAME, exists=True):
            cmds.deleteUI(self.WINDOW_NAME)
        
        # ウィンドウ作成
        window = cmds.window(
            self.WINDOW_NAME,
            title=self.WINDOW_TITLE,
            widthHeight=(400, 300),
            sizeable=True
        )
        
        # メインレイアウト
        main_layout = cmds.columnLayout(adjustableColumn=True, rowSpacing=5)
        
        cmds.separator(height=10, style='none')
        cmds.text(label="Layout Sync Tool for Maya", font="boldLabelFont")
        cmds.separator(height=10)
        
        # Unity → Maya セクション (Phase 2で実装)
        cmds.frameLayout(label="Unity → Maya (インポート)", collapsable=True, collapse=True)
        cmds.text(label="Phase 2で実装予定", align='center')
        cmds.setParent('..')
        
        cmds.separator(height=10)
        
        # Maya → Unity セクション
        cmds.frameLayout(label="Maya → Unity (エクスポート)", collapsable=True, collapse=False)
        
        form = cmds.formLayout(numberOfDivisions=100)
        
        # レイアウト名
        name_text = cmds.text(label="レイアウト名:", align='right')
        self.name_field = cmds.textField(text="Stage01")
        
        # 出力先
        path_text = cmds.text(label="出力先:", align='right')
        self.path_field = cmds.textField(editable=False, placeholderText="JSONファイルを選択...")
        browse_btn = cmds.button(label="参照...", command=self._browse_output_path)
        
        # エクスポート対象
        export_text = cmds.text(label="エクスポート対象:", align='right')
        self.export_radio = cmds.radioCollection()
        radio_layout = cmds.rowLayout(numberOfColumns=2)
        cmds.radioButton(label="選択オブジェクト", select=True)
        cmds.radioButton(label="全シーン")
        cmds.setParent('..')
        
        # エクスポートボタン
        export_btn = cmds.button(
            label="JSONエクスポート",
            height=35,
            backgroundColor=(0.3, 0.5, 0.8),
            command=self._export
        )
        
        # レイアウト配置
        cmds.formLayout(
            form, edit=True,
            attachForm=[
                (name_text, 'top', 10), (name_text, 'left', 10),
                (self.name_field, 'top', 10), (self.name_field, 'right', 10),
                (path_text, 'left', 10),
                (self.path_field, 'left', 100),
                (browse_btn, 'right', 10),
                (export_text, 'left', 10),
                (radio_layout, 'left', 100),
                (export_btn, 'left', 10), (export_btn, 'right', 10), (export_btn, 'bottom', 10)
            ],
            attachControl=[
                (self.name_field, 'left', 5, name_text),
                (path_text, 'top', 10, name_text),
                (self.path_field, 'top', 10, self.name_field),
                (self.path_field, 'right', 5, browse_btn),
                (browse_btn, 'top', 10, self.name_field),
                (export_text, 'top', 10, path_text),
                (radio_layout, 'top', 10, self.path_field),
                (export_btn, 'top', 15, radio_layout)
            ],
            attachPosition=[
                (name_text, 'right', 5, 25),
                (path_text, 'right', 5, 25),
                (self.path_field, 'left', 0, 25),
                (export_text, 'right', 5, 25),
                (radio_layout, 'left', 0, 25)
            ]
        )
        
        cmds.setParent('..')
        cmds.setParent('..')
        
        cmds.showWindow(window)
    
    def _browse_output_path(self, *args):
        """出力先ファイルを選択"""
        if cmds is None:
            raise RuntimeError("This module must be run inside Maya.")
        file_path = cmds.fileDialog2(
            fileMode=0,
            caption="JSONファイルを保存",
            fileFilter="JSON Files (*.json)",
            okCaption="保存"
        )
        
        if file_path:
            self.output_path = file_path[0]
            cmds.textField(self.path_field, edit=True, text=self.output_path)
    
    def _export(self, *args):
        """エクスポート実行"""
        if cmds is None:
            raise RuntimeError("This module must be run inside Maya.")
        if not self.output_path:
            cmds.confirmDialog(
                title="エラー",
                message="出力先を選択してください",
                button=["OK"]
            )
            return
        
        layout_name = cmds.textField(self.name_field, query=True, text=True)
        
        # ラジオボタンで選択判定
        selected_radio = cmds.radioCollection(self.export_radio, query=True, select=True)
        radio_label = cmds.radioButton(selected_radio, query=True, label=True)
        
        if radio_label == "選択オブジェクト":
            exported_count = self.exporter.export_selection(self.output_path, layout_name)
        else:
            exported_count = self.exporter.export_all(self.output_path, layout_name)
        
        if exported_count and exported_count > 0:
            cmds.confirmDialog(
                title="完了",
                message=f"エクスポートが完了しました\n{exported_count}個のオブジェクト\n{self.output_path}",
                button=["OK"]
            )


def show():
    """UIを表示"""
    ui = LayoutSyncUI()
    ui.create()