using System;
using System.IO;
using System.Text;
using UnityEngine;

namespace Lang
{
    public static class Log
    {
        //public const bool Enable = false;
        public const bool IsWriteFile = false;
        public const bool Enable = true;

        static string Time = DateTime.Now.ToString("MMdd/HH:mm:ss");

        public static void D(string title, string content)
        {
            if (Enable)
            {
#if UNITY_EDITOR
                string s = string.Format("[<color=green>{0}</color>] {1} <color=#646464FF>{2}</color>", title, content, Time);
                Debug.Log(s);
#else
                string s = string.Format("[{0}] {1} {2}", title, content, Time);
                WriteFile(s);
#endif
            }
        }

        public static void E(string title, string content)
        {
            if (Enable)
            {
#if UNITY_EDITOR
                string s = string.Format("[<color=red>{0}</color>] {1} <color=#646464FF>{2}</color>", title, content, Time);
                Debug.LogError(s);
#else
                string s = string.Format("[{0}] {1} {2}", title, content, Time);
                WriteFile(s);
#endif
            }
        }

        public static void WriteFile(string s)
        {
#if UNITY_STANDALONE
            if (IsWriteFile)
            {
                try
                {
                    string path = Path.Combine(Environment.CurrentDirectory, "log.txt");
                    byte[] content = Encoding.UTF8.GetBytes(s + "\n");
                    using (FileStream logFile = new FileStream(path, FileMode.OpenOrCreate, FileAccess.Write, FileShare.Write))
                    {
                        logFile.Seek(0, SeekOrigin.End);
                        logFile.Write(content, 0, content.Length);
                    }
                }
                catch (Exception ex)
                {
                    Debug.LogError(ex);
                }
            }
#else
            Debug.Log(s);
#endif
        }
    }
}