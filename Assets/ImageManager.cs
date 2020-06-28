using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ImageManager : MonoBehaviour
{

    public static ImageManager instance;

    public SpriteSource[] images;

    void Awake()
    {
        if (instance != null)
        {
            Destroy(gameObject);
        }
        else
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
    }

    public Sprite Get(string name)
    {
        SpriteSource s = Array.Find(images, item => item.name == name);
        if (s == null)
        {
            Debug.LogWarning("Image: " + name + " not found!");
        }
        return s.sprite;
    }
}

[Serializable]
public class SpriteSource
{
    public string name;
    public Sprite sprite;
}
