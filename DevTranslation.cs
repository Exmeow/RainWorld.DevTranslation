using System.Reflection;
using BepInEx;
using DevInterface;
using MonoMod.Cil;
using MonoMod.RuntimeDetour;
using UnityEngine;

namespace DevTranslate;

[BepInPlugin("DevTranslation", "Devtools Translation", "2024.4.3")]
public class DevTranslation : BaseUnityPlugin
{
	private const string exc = "加载时发生了错误！请尝试上报给作者";

	public void OnEnable()
	{
		Logger.LogInfo("正在加载…");
		try
		{
			SearchAndModifyAll(typeof(DevUI).GetConstructor([typeof(RainWorldGame)]) ?? throw new NullReferenceException(exc),
				["Room Settings", "Objects", "Sound", "Map", "Triggers", "Dialog"], ["房间设置", "物体", "声音", "地图", "触发器", "对话"]);
			SearchAndModifyAll(typeof(RainWorldGame).GetConstructor([typeof(ProcessManager)]) ?? throw new NullReferenceException(exc), ["Dev tools active"],
				["开发者工具已激活"]);
			SearchAndModifyAll(typeof(RainWorldGame).GetMethod("Update") ?? throw new NullReferenceException(exc), [" : Dev tools active"], ["：开发者工具已激活"]);
			SearchAndModifyAll(
				typeof(Page).GetConstructor([typeof(DevUI), typeof(string), typeof(DevUINode), typeof(string)]) ?? throw new NullReferenceException(exc),
				["PAGES", "Save", "Export Sandbox", "Using Specific!", "Create Specific"], ["页面", "保存", "导出沙盒", "使用特定方案！", "创建特定方案"]);
			SearchAndModifyAll(
				typeof(ObjectsPage).GetConstructor([typeof(DevUI), typeof(string), typeof(DevUINode), typeof(string)]) ?? throw new NullReferenceException(exc),
				["OBJECTS: ", "Next Page", "Previous Page"], ["物体：", "下一页", "上一页"]);
			SearchAndModifyAll(typeof(ObjectsPage).GetMethod("RefreshObjectsPage") ?? throw new NullReferenceException(exc), ["OBJECTS: ...", "OBJECTS: "],
				["物体：…", "物体："]);
			SearchAndModifyAll(
				typeof(RoomSettingsPage).GetConstructor([typeof(DevUI), typeof(string), typeof(DevUINode), typeof(string)]) ?? throw new NullReferenceException(exc),
				["Next Page", "Previous Page"], ["下一页", "上一页"]);
			SearchAndModifyAll(
				typeof(SoundPage).GetConstructor([typeof(DevUI), typeof(string), typeof(DevUINode), typeof(string)]) ?? throw new NullReferenceException(exc),
				["Next Page", "Previous Page"], ["下一页", "上一页"]);
			SearchAndModifyAll(typeof(DevUI).GetMethod("SwitchPage") ?? throw new NullReferenceException(exc),
				["Room Settings", "Objects", "Sound", "Map", "Triggers", "Dialog"], ["房间设置", "物体", "声音", "地图", "触发器", "对话"]);
			SearchAndModifyAll(
				typeof(DangerTypeCycler).GetConstructor([typeof(DevUI), typeof(string), typeof(DevUINode), typeof(Vector2), typeof(float)]) ??
				throw new NullReferenceException(exc), ["G.O: "], ["威胁类型："]);
			SearchAndModifyAll(
				typeof(InheritFromTemplateMenu).GetConstructor([typeof(DevUI), typeof(string), typeof(DevUINode), typeof(Vector2), typeof(Vector2)]) ??
				throw new NullReferenceException(exc), ["Inherit from Template:"], ["自模板继承："]);
			SearchAndModifyAll(
				typeof(SaveAsTemplateMenu).GetConstructor([typeof(DevUI), typeof(string), typeof(DevUINode), typeof(Vector2), typeof(Vector2)]) ??
				throw new NullReferenceException(exc), ["Save as Template:"], ["保存为模板："]);
			Logger.LogInfo("加载成功！");
		}
		catch (Exception e)
		{
			Logger.LogFatal(e);
			throw;
		}
	}

	private static void SearchAndModifyAll(MethodBase methodBase, string[] str, string[] str2)
	{
		Modify(methodBase, il =>
			{
				var c = new ILCursor(il);
				for (var i = 0; i < str.Length; i++)
				{
					c.Index = 0;
					// ReSharper disable once AccessToModifiedClosure
					while (c.TryGotoNext(j => j.MatchLdstr(str[i])))
					{
						c.Next.Operand = str2[i];
					}
				}
			}
		);
	}

	private static void Modify(MethodBase methodBase, ILContext.Manipulator il)
	{
		new ILHook(methodBase, il).Apply();
	}
}