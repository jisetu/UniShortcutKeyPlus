using System;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace UniShortcutKeyPlus
{
	internal static class LockInspector
	{
		private const string ITEM_NAME = "Edit/UniShortcutKeyPlus/Lock Inspector &l";

		[MenuItem( ITEM_NAME )]
		private static void Lock()
		{
			Type inspectorType = typeof(Editor).Assembly.GetType("UnityEditor.InspectorWindow");
			if ( inspectorType == null ) return;

			// 1. フォーカスしているウィンドウを取得、なければマウスオーバーしているウィンドウを取得
			var window = EditorWindow.focusedWindow;
			if ( window == null || !inspectorType.IsInstanceOfType( window ) )
			{
				window = EditorWindow.mouseOverWindow;
			}

			// 2. 対象がなければ何もしない
			if ( window == null || !inspectorType.IsInstanceOfType( window ) ) return;

			// 3. リフレクションを使って isLocked プロパティにアクセスし、値を反転させる
			var type = window.GetType();
			var property = type.GetProperty( "isLocked", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic );

			if ( property != null )
			{
				bool value = (bool)property.GetValue( window );
				property.SetValue( window, !value );
				
				// ロック状態のアイコン表示を更新するために再描画
				window.Repaint();
			}
		}
	}
}