using System;
using System.Linq;
using System.Collections;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using UnityEngine;

public class TimerHelper : MonoBehaviour
{
    private List<Coroutine> m_Coroutine_Queue = new List<Coroutine>();
    public static ReadOnlyCollection<Coroutine> Queue { get { return instance.m_Coroutine_Queue.AsReadOnly(); } }

    private static TimerHelper instance;
    private static TimerHelper Instance
    {
        get
        {
            if (instance == null)
            {
                Type t = typeof(TimerHelper);
                instance = (TimerHelper)FindObjectOfType(t);
                if (instance == null) { instance = new GameObject(typeof(TimerHelper).Name).AddComponent<TimerHelper>(); }
            }
            return instance;
        }
    }
    private void Awake()
    {
        if (this != Instance)
        {
            Destroy(this);
            return;
        }
    }
    IEnumerator BaseTimer(float _time, Action _stopCallBack = null)
    {
        float tempTime = 0;
        while (tempTime < _time)
        {
            tempTime += Time.deltaTime;
            yield return null;
        }
        if (_stopCallBack != null) { _stopCallBack(); }
    }
    IEnumerator IntervalTimer(float _duration,float _intervalTime, Action<float> _intervalCallBack = null ,Action _stopCallBack = null)
    {
        float tempTime = 0;
        while (tempTime < _duration)
        {
            yield return new WaitForSeconds(_intervalTime);
            tempTime += _intervalTime;
            if(_intervalCallBack != null) { _intervalCallBack(tempTime); }
        }
        if (_stopCallBack != null) { _stopCallBack(); }
    }

    public static void StartTimer(float _time, Action _stopCallBack = null) 
    {
        if (_time == 0) { return; }
        _time = Mathf.Abs(_time);
        Coroutine timer = Instance.StartCoroutine(Instance.BaseTimer(_time,_stopCallBack));
    }
    public static void StartIntervalTimer(float _duration,float _time = 1, Action<float> _intervalCallBack = null,Action _stopCallBack = null) 
    {
        if (_duration < _time  || _time == 0) { return; }
        _duration = Mathf.Abs(_duration);
        _time = Mathf.Abs(_time);
        Coroutine timer = Instance.StartCoroutine(Instance.IntervalTimer(_duration,_time, _intervalCallBack,_stopCallBack));
    }
}
public class EqualsHelper
{
    public static bool IsEquals<T>(T[] _before, T[] _after)
    {
        var firstNotSecond = _before.Except(_after).ToList();
        var secondNotFirst = _after.Except(_before).ToList();
        return !firstNotSecond.Any() && !secondNotFirst.Any();
    }
    public static bool IsEquals<T>(List<T> _before, List<T> _after)
    {
        var firstNotSecond = _before.Except(_after).ToList();
        var secondNotFirst = _after.Except(_before).ToList();
        return !firstNotSecond.Any() && !secondNotFirst.Any();
    }
}
