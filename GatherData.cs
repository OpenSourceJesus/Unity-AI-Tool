using System;
using System.IO;
using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

public class GatherData : MonoBehaviour
{
	const string FILES_PREFIX = "Here is my code made for Unity with C# separated by file names with their contents in triple-quoted strings:";
	const string ERROR_INDCTR = "): error ";
	const string ERRORS_PREFIX = "Let me know how to fix these errors: ";

	[MenuItem("Tools/Gather data %g")]
	static void Do ()
	{
		string data = FILES_PREFIX + '\n';
		foreach (string csFilePath in SystemExtensions.GetAllFilePathsInFolder(Application.dataPath, ".cs"))
			data += "File name: " + csFilePath.Substring(csFilePath.LastIndexOf('/') + 1) + "\nFile contents:'''" + File.ReadAllText(csFilePath) + "'''\n";
		data += ERRORS_PREFIX + '\n';
#if UNITY_EDITOR_WIN
		string logPath = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + "\Unity\Editor\Editor.log";
#elif UNITY_EDITOR_OSX
		string logPath = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile) + "/Library/Logs/Unity/Editor.log";
#else
		string logPath = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile) + "/.config/unity3d/Editor.log";
#endif
		string[] logLines = File.ReadAllLines(logPath);
		List<string> logLines_ = new List<string>(logLines);
		int lineIdx = 0;
		string dataFilePath = Path.GetTempPath() + "/AITool Data";
		if (File.Exists(dataFilePath))
			lineIdx = int.Parse(File.ReadAllText(dataFilePath));
		if (lineIdx < logLines.Length)
			logLines_.RemoveRange(0, lineIdx);
		for (int i = 0; i < logLines_.Count; i ++)
		{
			string logLine = logLines_[i];
			if (!logLine.Contains(ERROR_INDCTR))
			{
				logLines_.RemoveAt(i);
				i --;
			}
		}
		data += string.Join('\n', logLines_.ToArray());
		GUIUtility.systemCopyBuffer = data;
		lineIdx = logLines.Length;
		File.WriteAllText(dataFilePath, "" + lineIdx);
	}
}