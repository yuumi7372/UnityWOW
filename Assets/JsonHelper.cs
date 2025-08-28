using System;
using UnityEngine;

public static class JsonHelper
{
    [Serializable]
    private class Wrapper<T>
    {
        public T[] words;
    }

    public static T[] FromJson<T>(string json)
    {
        string wrappedJson = "{ \"words\": " + json + "}";
        Wrapper<T> wrapper = JsonUtility.FromJson<Wrapper<T>>(wrappedJson);
        return wrapper.words;
    }
}
