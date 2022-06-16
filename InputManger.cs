using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class InputManger
{
    public Action KeyAction = null;
   
    private Transform _player ;
    public  Transform POS { get { return _player; } set { _player = value; } }

    public void KeyInput()
    {
        if (Input.anyKey == false) { return; }
        if (KeyAction != null) 
        { KeyAction.Invoke(); }
        
    }

  

    // 마우스 위치로 이동
    public IEnumerator Moving(Transform player ,Vector3 dest,float time )
    {
        float elapsedTime = 0;
        while (elapsedTime < time)
        {
            player.position = Vector3.Lerp(player.position, dest, (elapsedTime / time) );
            elapsedTime += Time.deltaTime;
            yield return null;
        }
    }
    // 마우스 위치로 회전
    public IEnumerator Rotating(Transform player, Quaternion degree, float time)
    {
        float elapsedTime = 0;
        while (elapsedTime < time)
        {
            player.rotation = Quaternion.Lerp(player.rotation, degree, (elapsedTime / time) );
            elapsedTime += Time.deltaTime;
            yield return null;
        }
    }
   

}
