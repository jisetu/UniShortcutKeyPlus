using System;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace UniShortcutKeyPlus
{
	internal static class ToggleDebugMode
	{
		private const string ITEM_NAME = "Tools/UniShortcutKeyPlus/Toggle Inspector Debug &k";

        [MenuItem(ITEM_NAME)]
        private static void Toggle()
        {
            EditorApplication.delayCall += ToggleInternal;
        }

        private static void ToggleInternal()
        {
            // 1. すべての InspectorWindow を取得
            var inspectorType = typeof(Editor).Assembly.GetType("UnityEditor.InspectorWindow");

            var inspectors = Resources.FindObjectsOfTypeAll(inspectorType);
            const BindingFlags flags = BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public;

            foreach (EditorWindow w in inspectors)
            {
                if (w == null) continue;

                // 2. InspectorWindow 固有の ActiveEditorTracker を取得
                // 3. リフレクションを使って、"m_Tracker" というフィールド(変数)を探す
                var trackerField = inspectorType.GetField(
                    "m_Tracker",
                    BindingFlags.NonPublic | BindingFlags.Instance
                );
                if (trackerField == null) continue;
                var tracker = trackerField.GetValue(w) as ActiveEditorTracker;
                if (tracker == null) continue;

                var isNormal = tracker.inspectorMode == InspectorMode.Normal;
                var newMode = isNormal ? InspectorMode.Debug : InspectorMode.Normal;

                // 4. リフレクションを使って、"inspectorMode" プロパティにアクセスし、値を反転させる
                var modeProperty = inspectorType.GetProperty("inspectorMode", flags);
                if (modeProperty != null && modeProperty.CanWrite)
                {
                    modeProperty.SetValue(w, newMode);
                    Debug.Log("方法A: inspectorMode プロパティで設定");
                }

                //  5. 再構築を遅延実行
                EditorApplication.delayCall += () =>
                {
                    try
                    {
                        tracker.ForceRebuild();
                        w.Repaint();
                    }
                    catch (System.Exception e)
                    {
                        Debug.LogError($"再構築エラー: {e.Message}");
                    }
                };
            }
        }
	}
}