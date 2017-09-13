using System;
using UnityEngine;

public delegate void LogTraceReceiver(string content);

public enum LogLevel
{
    No,
    Error,
    Trace,
    Warning,
    Info
}

public static class Log
{
    public static LogLevel LogLevel = LogLevel.Info;
    public static LogTraceReceiver TraceReceiver;

    private static object _format(object msg, params object[] args)
    {
        string str = msg as string;
        if ((args.Length != 0) && !string.IsNullOrEmpty(str))
        {
            return string.Format(str, args);
        }
        return msg;
    }

    public static void Error(object msg, params object[] args)
    {
        if (LogLevel >= LogLevel.Error)
        {
            Debug.LogError(_format(msg, args));
            
        }
    }

    public static void Warning(object msg, params object[] args)
    {
        if (LogLevel >= LogLevel.Warning)
        {
            Debug.LogWarning(_format(msg, args));
        }
    }

    public static void Info(object msg, params object[] args)
    {
        if (LogLevel >= LogLevel.Info)
        {
            Debug.Log(_format(msg, args));
        }
    }

    public static void Info(object msg, UnityEngine.Object context)
    {
        if (LogLevel >= LogLevel.Info)
        {
            Debug.Log(msg, context);
        }
    }

    public static void Trace(object msg, params object[] args)
    {
        if (LogLevel >= LogLevel.Trace)
        {
            object message = _format(string.Format("Trace: {0}", msg), args);
            if (Application.isEditor)
            {
                Debug.Log(message);
            }
            else if ((TraceReceiver != null) && (message != null))
            {
                TraceReceiver(message.ToString());
            }
        }
    }

    public static void Assert(bool condition, object msg, params object[] args)
    {
        if (!condition)
        {
            Debug.LogError(_format(msg, args));
        }
    }

    public static void Exception(System.Exception ex)
    {
        if (LogLevel >= LogLevel.Error)
        {
            Debug.LogException(ex);
        }
    }
}
