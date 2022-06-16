using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Managers : MonoBehaviour
{
    
    static Managers _Manager;
    InputManger IM = new InputManger();
    Rot rot = new Rot();
    private void Start()
    {
        Application.targetFrameRate = 60;
        Init();
    }
    static void Init()
    {
        if (_Manager == null)
        {
            GameObject go = GameObject.Find("@Managers");
            if (go == null)
            {
                go = new GameObject() { name = "@Managers" };
                go.AddComponent<Managers>();
            }
            DontDestroyOnLoad(go);
            _Manager = go.gameObject.GetComponent<Managers>();
        }
    }

    public static Managers INSTANCE {
        get {
            if (_Manager == null)
            { Managers.Init(); }
            return _Manager;  }
    }
    
    // Manager
    public static InputManger _InputManger
    { get {return INSTANCE.IM; } }

    // Include
    public static Rot _Rot { get { return INSTANCE.rot; } }

    private void Update()
    {
        _InputManger.KeyInput();
    }
 
}
