using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ObjectData
{
    public string tag;
    public string name;
    public Vector3 Position;
    public Vector3 Rotation;
    public Vector3 Scale;

    //Constructor empty
    public ObjectData()
    {
        tag = "";
        name = "";
        Position = Vector3.zero;
        Rotation = Vector3.zero;
        Scale = Vector3.zero;
    }
    //Constructor
    public ObjectData(string _tag, string _name, Vector3 _pos, Vector3 _rot, Vector3 _scale)
    {
        tag = _tag;
        name = _name;
        Position = _pos;
        Rotation = _rot;
        Scale = _scale;
    }
}

//This class is needed for write arrays in JSON
public static class JsonHelper//Json Class helper
{
    public static T[] FromJson<T>(string json)
    {
        Wrapper<T> wrapper = JsonUtility.FromJson<Wrapper<T>>(json);
        return wrapper.Items;
    }
    public static string ToJson<T>(T[] array)
    {
        Wrapper<T> wrapper = new Wrapper<T>();
        wrapper.Items = array;
        return JsonUtility.ToJson(wrapper);
    }
    public static string ToJson<T>(T[] array, bool prettyPrint)
    {
        Wrapper<T> wrapper = new Wrapper<T>();
        wrapper.Items = array;
        return JsonUtility.ToJson(wrapper, prettyPrint);
    }
    [System.Serializable]
    private class Wrapper<T>
    {
        public T[] Items;
    }
}
