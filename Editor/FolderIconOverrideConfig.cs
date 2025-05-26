using System.Collections;
using UnityEditor;
using UnityEngine;
/// <summary>
/// description:      
/// </summary>
namespace ZeroUltra.FolderIcon
{
    [CreateAssetMenu(menuName = "ScriptableObject/FolderIconOverrideConfig", fileName = "FolderIconOverrideConfig", order = 1)]

    public class FolderIconOverrideConfig : ScriptableObject
    {
        public FolderIconOverride[] folderIconOverrides;

        [System.Serializable]
        public class FolderIconOverride
        {
            [Tooltip("The folder to override the icon for.")]
            public Object Folder;
            [Tooltip("Built-in icon name")]
            public string IconName; // Built-in icon name
            public Texture2D CustomIcon;
            public Color Color = Color.white;
            public Rect offsetRect = Rect.zero;
        }
    }

    [InitializeOnLoad]
    public class FolderIconOverrideDraw
    {
        readonly static Color32 backgroundColor = new Color32(55, 55, 55, 255);
        static FolderIconOverrideConfig config;
        static FolderIconOverrideDraw()
        {
            EditorApplication.delayCall += () =>
            {
                config = FindScriptableObject<FolderIconOverrideConfig>();
                if (config != null)
                {
                    EditorApplication.projectWindowItemOnGUI += OnProjectWindowItemOnGUI;
                }
            };
        }
        private static void OnProjectWindowItemOnGUI(string guid, Rect selectionRect)
        {
            string assetPath = AssetDatabase.GUIDToAssetPath(guid);
            var foldericons = config.folderIconOverrides;
            foreach (var item in foldericons)
            {
                if (item.Folder != null)
                {
                    var folderPath = AssetDatabase.GetAssetPath(item.Folder);
                    //判断是否是文件夹且guid相同
                    if (AssetDatabase.IsValidFolder(folderPath) && AssetDatabase.GUIDFromAssetPath(folderPath).ToString() == guid)
                    {
                        //默认是文件夹图标
                        Texture icon = null;
                        //显示内置图标
                        if (!string.IsNullOrEmpty(item.IconName))
                        {
                            icon = EditorGUIUtility.TrIconContent(item.IconName).image;
                        }
                        //显示自定义图标
                        else if (item.CustomIcon != null)
                        {
                            icon = item.CustomIcon;
                        }
                        else
                        {
                            icon = Texture2D.whiteTexture;
                        }
                        if (icon != null)
                        {
                            DrawIconInLowerLeft(selectionRect, icon, item.Color, offsetRect: item.offsetRect);
                        }
                    }
                }
            }
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

        /// <summary>
        /// 画图标到右下角
        /// </summary>
        /// <param name="rect">位置尺寸</param>
        /// <param name="icon">图标</param>
        /// <param name="color">颜色</param>
        /// <param name="offsetRect">偏移尺寸</param>
        /// <param name="borderWidth">图标边框宽度</param>
        /// <param name="borderRadius"></param>
        private static void DrawIconInLowerLeft(Rect rect, Texture icon, Color color = default, Rect offsetRect = default, bool drawBackground = false, float borderRadius = 10f)
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
            else if (isTreeView)
            {
                rect.width = rect.height = 10f;
                rect.x += 6f;
                rect.y += 4.5f;
            }
            else
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
            //画一个背景
            if (drawBackground)
            {
                GUI.DrawTexture(rect, Texture2D.whiteTexture, ScaleMode.ScaleToFit, true, 0, backgroundColor, 0, borderRadius);
            }
            GUI.DrawTexture(rect, icon, ScaleMode.ScaleToFit, true, 0, color, 0, 1); //后面的两个参数 图标边框宽度 图标圆角
        }

        /// <summary>
        /// 判断是否是小图标
        /// </summary>
        /// <param name="rect"></param>
        /// <returns></returns>
        private static bool IsSmallIcon(Rect rect)
        {
            return rect.height == 16;
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
    }

    [CustomEditor(typeof(FolderIconOverrideConfig))]
    public class FolderIconOverrideEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            if (GUILayout.Button("Apply"))
            {
                EditorUtility.RequestScriptReload();
            }
            GUILayout.Label("please click apply if it is not displayed");
        }
    }
}