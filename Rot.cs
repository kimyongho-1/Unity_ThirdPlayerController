using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rot 
{
    
    IEnumerator _Co;
    bool _isSameDir = false;
    
    Quaternion _destiny;
    private void Start()
    {
        
        // target = _aim.transform.position;
    }

    public Quaternion Rot_Q4(Transform player, Vector3 target)
    {
      Vector3 forwardDir = (player.forward);
      Vector3 dir = (target - player.position);
      
      // 외적 CrossProduct
      // 디버그로그로 확인시 전방기준, 음수는  왼쪽, 양수는 오른쪽
      Vector3 cross = Vector3.Cross(forwardDir, dir);


      // 나의 전방벡터(Forward)와 바라볼 물체(Direction)사이의 내적 FdotD는 cosθ 이다
      float FdotD = Vector3.Dot(forwardDir, dir) / (Vector3.Magnitude(forwardDir) * Vector3.Magnitude(dir));
      float angle = Mathf.Acos(FdotD) * Mathf.Rad2Deg; // 코싸인값을 라디안값으로 변환해주는 Acos, 익숙한 각도법으로 다시 변환하는 Rad2Deg
      Debug.Log("외적벡터 : " + cross + "회전할 각도 : " + angle);
      Quaternion Angle ;
      // 현재 내 에디터씬 기준에서, 외적벡터의 y값이 음수면 물체가 왼쪽방향에 있다
      if (cross.y < 0) // 전방벡터 기준 왼쪽
      {
            Angle = player.rotation * Quaternion.Euler(0, -angle, 0);
      }
      else // 전방벡터 기준 오른쪽
      {
            Angle = player.rotation * Quaternion.Euler(0, angle, 0);
      }
        return Angle;
    }
    public Vector3 Rot_V3(Transform player, Vector3 target)
    {
        Vector3 forwardDir = (player.forward);
        Vector3 dir = (target - player.position);

        // 외적 CrossProduct
        // 디버그로그로 확인시 전방기준, 음수는  왼쪽, 양수는 오른쪽
        Vector3 cross = Vector3.Cross(forwardDir, dir);


        // 나의 전방벡터(Forward)와 바라볼 물체(Direction)사이의 내적 FdotD는 cosθ 이다
        float FdotD = Vector3.Dot(forwardDir, dir) / (Vector3.Magnitude(forwardDir) * Vector3.Magnitude(dir));
        float angle = Mathf.Acos(FdotD) * Mathf.Rad2Deg; // 코싸인값을 라디안값으로 변환해주는 Acos, 익숙한 각도법으로 다시 변환하는 Rad2Deg
        Debug.Log("외적벡터 : " + cross + "회전할 각도 : " + angle);
        Quaternion Angle;
        // 현재 내 에디터씬 기준에서, 외적벡터의 y값이 음수면 물체가 왼쪽방향에 있다
        if (cross.y < 0) // 전방벡터 기준 왼쪽
        {
            Angle = player.rotation * Quaternion.Euler(0, -angle, 0);
        }
        else // 전방벡터 기준 오른쪽
        {
            Angle = player.rotation * Quaternion.Euler(0, angle, 0);
        }
        return Angle.eulerAngles;
    }
    
void CalculDir(Transform player,float degree , Quaternion dest)
    {
       
        player.rotation = Quaternion.Lerp(player.rotation, dest
            , Time.deltaTime *5f);

        // 각도차가 0.1보다 작다면, 육안으로도 확인이 힘들정도니 미세한차이는 강제로 맞춰주기
        if (dest.y - player.rotation.y <= Mathf.Epsilon) 
        {
            player.rotation = dest;
            _isSameDir = true;
        }
    } // 쿼터니언값으로 표현
    void CalculDir(Transform player,float degree) // vector3.eulerAngles 벡터로 표현
    {
        player.eulerAngles = Vector3.Lerp(player.eulerAngles,
            new Vector3(player.eulerAngles.x, degree,  player.eulerAngles.z) ,Time.deltaTime * 3f);
       
        // 각도차가 0.1보다 작다면, 육안으로도 확인이 힘들정도니 미세한차이는 강제로 맞춰주기
        if (degree - player.eulerAngles.y <= 0.1)
        {
            player.eulerAngles = new Vector3(0, degree,0);
            _isSameDir = true;
        }
    }
    IEnumerator RotCoru(Transform player,float degree)
    {

        while (_isSameDir != true)
        {
            CalculDir(player, degree);
            yield return new WaitForSeconds(0.001f);

        }
        Debug.Log("_isSameDir :" + _isSameDir);
        _Co = null; yield break;
    }
    IEnumerator RotCoru(Transform player,float degree, Quaternion dest)
    {
        
        while (_isSameDir != true)
        {
            CalculDir(player ,degree, dest);
            yield return new WaitForSeconds(0.001f);
            
        }
        Debug.Log("_isSameDir :" + _isSameDir);
         _Co = null; yield break;
    }

    void RotThroughCos(Transform player, Vector3 target)
    {
        // 방향벡터 만들기, 방향벡터는 크기가 1인것이 규정
        Vector3 forwardDir = (player.forward);
        Vector3 rightDir = player.right;
        Vector3 dir = (target - player.position);


        // 내적 공식
        // Vector3.Dot(player.position, dir) ==
        // Vector3.Magnitude(player.position) * Vector3.Magnitude(dir) * Mathf.Cos(cos);
        // 코사인구하기 변수 cos자체가 mathf.cos()임
        float cos = Vector3.Dot(forwardDir, dir) / (
            Vector3.Magnitude(forwardDir) * Vector3.Magnitude(dir)  );
        // 코사인비율을 역함수 사용하여 각도구하기
        float radian = Mathf.Acos(cos);   // Acos사용시 라디안값을 반환함
        float degree = radian * Mathf.Rad2Deg;
        Debug.Log("cos : " +cos + " , radian :" + radian + ", degree : " + degree);

        // right벡터와 내적한 값을 의미하는 RdotD 변수 (RightVector.Dot.Direction)
        float RdotD = Vector3.Dot(rightDir, dir) / (
            Vector3.Magnitude(rightDir) * Vector3.Magnitude(dir) );  
        float Rcos = Mathf.Acos(RdotD) * Mathf.Rad2Deg;  // right벡터와 내적한 코사인을 역함수로 각도로 변환

        if (degree > 360.0f) { degree -= 360.0f; }

        // 전방벡터와 내적후 각도차이보기 (cos가 음수면 뒷편, 양수면 앞면)
        if (cos < 0)
        {
            Debug.Log("물체는 현재 뒤에있음");
            Debug.Log((Rcos < 90) ? "뒤와 오른쪽 방향" : "뒤와 왼쪽 방향");
            Debug.Log("오른쪽벡터와의 내적후 각도차이 : " + Rcos + ", degree : " + degree);
            if (degree > 180.0f && Rcos > 90.0f)
            {  degree = degree - 360.0f; }
            else if (degree < 180.0f && Rcos > 90.0f)
            { degree = -degree; }
        }
        if (cos >= 0) 
        {
            Debug.Log("물체는 현재 앞에있음");
            Debug.Log((Rcos <= 90) ? "오른쪽 방향" : "왼쪽 방향");

            //right벡터와 다시 내적하여 90도를 넘는지확인 ( 90이상은 왼쪽 , 이하는 오른쪽 방향)
            degree = 90.0f - Rcos;
          
            Debug.Log("오른쪽벡터와의 내적후 각도차이 : " + Rcos + ", degree : " + degree);
        }  

       
        Quaternion a = player.rotation*Quaternion.AngleAxis(degree, Vector3.up);
        
        if (player.rotation.y != a.y )
        {
            _isSameDir = false;
            RotCoru(player,degree, a);
            
        }


    }
    void RotThroughAtan2(Transform player, Vector3 target)
    {
       
        Vector3 forwardDir = (player.forward);  // 월드공간에선 언제나 001 z방향
        Vector3 dir = (target - player.position);

        // FdotD는 전방과 방향 사이의 내적결과 cos이 됨 (앞뒤 판별하기위해)
        // 내적의 결과가(-1~1사이를 도는 cos값이) 양수일시 방향벡터가 내 앞면, 음수일시 내 뒷편
        float FdotD = Vector3.Dot(forwardDir, dir) / (Vector3.Magnitude(forwardDir)*Vector3.Magnitude(dir));
        //Debug.Log(FdotD);
        // Atan2 공식
        // 먼저 탄젠트의 경우, 직각삼각형에서 높이/밑변이된다
        // 유니티 3d,2d할것없이 직교좌표계로 바라본다면 
        // 바라볼 물체의 x, y가 밑변 높이로 비례한다
        // 또 이런 탄젠트각도는 Atan2로 구할수있다, 인자순서는 y,x순이다
        float degree = Mathf.Atan2(dir.z, dir.x) * Mathf.Rad2Deg ;
        float angle =0;
        Debug.Log("dir.z : "+dir.z+"dir.x : "+dir.x +" degree : " + degree);
        // 내 앞에 물체 존재
        if (degree > 0)
        {
            angle = (degree < 90.0f) ? (90.0f - degree) : (90.0f-degree);
           //if (degree < 90.0f) { angle = 90.0f - degree; } // 내 오른쪽
           //else { angle = -(degree - 90.0f); } // 내 왼쪽
        }
        // 내 뒤에 존재
        if (degree <= 0)
        {
            angle = (degree > -90.0f) ? (90.0f - degree) : (-270.0f - degree) ;
           //if (degree > -90.0f) { angle = 90.0f - degree; }
           //else { angle = -(270.0f + degree); }
        }
        
        // Quaternion angleAxis = Quaternion.AngleAxis(-degree, Vector3.up);
        // transform.rotation = Quaternion.Slerp(player.rotation, angleAxis, Time.deltaTime*2f);
        //z축으로 회전을 한다면 양수 각도이지만, 현재 나는 y축으로 회전하기에, 음수로 반전시킨다
        // 이유는 유니티 인스펙터에서 쉽게 알수있는데
        // 어떤 월드오브젝트를 두고 y축으로 회전해보면 양수가 오른쪽, 음수가 왼쪽방향
        // z축으로 회전시 음수가 오른쪽 양수가 왼쪽방향으로 회전하는걸 쉽게볼수있다
        // 따라서 각도를 음수로 전개해주어야한다
        if (angle != 0 )
        {
            Debug.Log("회전 시작");
            _isSameDir = false;
            RotCoru(player ,angle);
           
        }
        
    }
  
}
