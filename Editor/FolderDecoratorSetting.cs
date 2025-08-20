using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;


namespace ZeroUltra.FolderDecorator
{
    public enum LabelStyle
    {
        Normal,
        Bold,
        Italic,
        BoldAndItalic,
    }
    [CreateAssetMenu(menuName = "FolderDecorateSetting", fileName = "FolderDecorateSetting", order = 1)]

    public class FolderDecoratorSetting : ScriptableObject
    {
        public FolderDecorators[] folderDecorators;

        [System.Serializable]
        public class FolderDecorators
        {
            [Tooltip("The folder to override the icon for.")]
            public Object Folder;
            [Tooltip("Built-in icon name")]
            public string BuiltinIconName; // Built-in icon name
            public Texture2D CustomIcon;
            public Rect IconOffset = Rect.zero;

            public LabelStyle LabelStyle = LabelStyle.Normal;
            public Color LabelColor = Color.white;
            public Color BackgroundColor = Color.white;
            [Tooltip("边框圆角")]
            public float BacgroundRadius = 0f;
            [Tooltip("边框的宽度。如果为0，则绘制完整的纹理")]
            public float BackgroundBorderWidth = 0f;
            public string Tooltip;
        }
    }

    [InitializeOnLoad]
    public class FolderIconOverrideDraw
    {
        static Dictionary<string, FolderDecoratorSetting.FolderDecorators> dictFolderOverridies = new();
        static FolderIconOverrideDraw()
        {
            EditorApplication.delayCall += () =>
            {
                dictFolderOverridies.Clear();
                var config = FindScriptableObject<FolderDecoratorSetting>();
                if (config != null)
                {
                    var foldericons = config.folderDecorators;
                    foreach (var item in foldericons)
                    {

                        var folderPath = AssetDatabase.GetAssetPath(item.Folder);
                        if (AssetDatabase.IsValidFolder(folderPath))
                        {
                            var guid = AssetDatabase.AssetPathToGUID(folderPath);
                            dictFolderOverridies.Add(guid, item);
                        }
                        else
                            Debug.LogError("Folder path is invalid: " + folderPath);
                    }
                    EditorApplication.projectWindowItemOnGUI += OnProjectWindowItemOnGUI;
                }
            };
        }
        private static void OnProjectWindowItemOnGUI(string guid, Rect selectionRect)
        {
            string assetPath = AssetDatabase.GUIDToAssetPath(guid);
            if (dictFolderOverridies.TryGetValue(guid, out var folderSetData))
            {
                //绘制背景
                int styleIndex = (int)folderSetData.LabelStyle;
                GUIStyle labelStyle = new GUIStyle(GUI.skin.label)
                {
                    fontStyle = styleIndex switch
                    {
                        0 => FontStyle.Normal,
                        1 => FontStyle.Bold,
                        2 => FontStyle.Italic,
                        3 => FontStyle.BoldAndItalic,
                        _ => FontStyle.Normal,
                    }
                };
                //如果透明度不为0，则设置文字颜色 为0则是默认颜色
                if (!Mathf.Approximately(folderSetData.LabelColor.a, 0))
                    labelStyle.normal.textColor = folderSetData.LabelColor;

                var tooltipContent = new GUIContent(System.IO.Path.GetFileName(assetPath), folderSetData.Tooltip);
                if (IsTreeView(selectionRect))
                {
                    var pos = selectionRect;
                    pos.x += 16.5f;
                    pos.height = 14f; //默认是16
                    pos.y += 1f;//下移一个单位
                    GUI.DrawTexture(pos, EditorGUIUtility.whiteTexture, ScaleMode.StretchToFill, false, 0, folderSetData.BackgroundColor, folderSetData.BackgroundBorderWidth, folderSetData.BacgroundRadius);

                    EditorGUI.LabelField(pos, tooltipContent, labelStyle);
                }
                else if (IsSmall(selectionRect))
                {
                    var pos = selectionRect;
                    pos.x += 19.5f;
                    pos.height = 14f;
                    pos.y += 1f;
                    GUI.DrawTexture(pos, EditorGUIUtility.whiteTexture, ScaleMode.StretchToFill, false, 0, folderSetData.BackgroundColor, folderSetData.BackgroundBorderWidth, folderSetData.BacgroundRadius);
                    EditorGUI.LabelField(pos, tooltipContent, labelStyle);
                }
                else //大图标
                {
                    var pos = selectionRect;
                    FitIconRect(ref pos);
                    var folderIcon = IsEmptyFolder(assetPath) ? EditorGUIUtility.IconContent("folderempty icon") : EditorGUIUtility.IconContent("folder icon");
                    GUI.DrawTexture(pos, folderIcon.image, ScaleMode.StretchToFill, false, 0, folderSetData.BackgroundColor, 0, 1);
                }

                //绘制图标
                Texture icon = null;
                //显示内置图标
                if (!string.IsNullOrEmpty(folderSetData.BuiltinIconName))
                {
                    icon = EditorGUIUtility.TrIconContent(folderSetData.BuiltinIconName).image;
                }
                //显示自定义图标
                else if (folderSetData.CustomIcon != null)
                {
                    icon = folderSetData.CustomIcon;
                }
                if (icon != null)
                {
                    DrawIconInLowerLeft(selectionRect, icon, folderSetData.BackgroundColor, offsetRect: folderSetData.IconOffset);
                }
            }
        }


        /// <summary>
        /// 画图标到右下角
        /// </summary>
        /// <param name="rect">位置尺寸</param>
        /// <param name="icon">图标</param>
        /// <param name="color">颜色</param>
        /// <param name="offsetRect">偏移尺寸</param>
        /// <param name="borderWidth">图标边框宽度</param>
        /// <param name="borderRadius"></param>
        private static void DrawIconInLowerLeft(Rect rect, Texture icon, Color color = default, Rect offsetRect = default, float borderRadius = 10f)
        {
            if (color == default)
                color = Color.white;
            bool isSmall = FitIconRect(ref rect);
            bool isTreeView = IsTreeView(rect);
            if (isSmall && !isTreeView)   //小图标且不在TreeView
            {
                rect.width = rect.height = 10f;
                rect.x += 8;
                rect.y += 4;
            }
            else if (isTreeView)  //在TreeView
            {
                rect.width = rect.height = 10f;
                rect.x += 6f;
                rect.y += 4.5f;
            }
            else //大图标
            {
                rect.width = rect.height * 0.5f;
                rect.y += rect.height * 0.068f;
                rect.x += rect.width * 0.8f;
            }
            if (offsetRect != default)
            {
                rect.x += offsetRect.x;
                rect.y += offsetRect.y;
                rect.width += offsetRect.width;
                rect.height += offsetRect.height;
            }
            GUI.DrawTexture(rect, icon, ScaleMode.ScaleToFit); //后面的两个参数 图标边框宽度 图标圆角
        }

        private static T FindScriptableObject<T>(string[] searchInFolders = null) where T : ScriptableObject
        {
            var assetGUID = AssetDatabase.FindAssets("t:ScriptableObject", searchInFolders);
            foreach (var item in assetGUID)
            {
                var assetPath = AssetDatabase.GUIDToAssetPath(item);
                var obj = AssetDatabase.LoadMainAssetAtPath(assetPath);
                //找到目标
                if (obj is T t)
                {
                    return t;
                }
            }
            return default(T);
        }

        private static bool IsSmall(Rect rect)
        {
            return rect.width > rect.height;
        }
        /// <summary>
        /// 判断是否是TreeView 也就是文件夹旁边是否有小三角
        /// </summary>
        private static bool IsTreeView(Rect rect)
        {
            return (rect.x - 16) % 14 == 0;
        }
        /// <summary>
        /// 根据视图缩放处理rectz正确的宽度和高度
        /// </summary>
        /// <param name="rect"></param>
        /// <returns>是否是小图标</returns>
        public static bool FitIconRect(ref Rect rect)
        {
            //最小是16
            var isSmall = rect.width > rect.height;

            if (isSmall)
                rect.width = rect.height;
            else
                rect.height = rect.width;
            return isSmall;
        }

        private static bool IsEmptyFolder(string folder)
        {
            DirectoryInfo directoryInfo = new DirectoryInfo(folder);
            // Check for sub - folders
            if (directoryInfo.GetDirectories().Length > 0)
            {
                return false;
            }
            // Check for files
            if (directoryInfo.GetFiles().Length > 0)
            {
                return false;
            }
            return true;
        }
    }

    [CustomEditor(typeof(FolderDecoratorSetting))]
    public class FolderIconOverrideEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            if (GUILayout.Button("Apply"))
            {
                AssetDatabase.SaveAssetIfDirty(target);
                EditorUtility.RequestScriptReload();
            }
            GUILayout.Label("please click apply if it is not displayed");
        }
    }
}