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
      
      // ���� CrossProduct
      // ����׷α׷� Ȯ�ν� �������, ������  ����, ����� ������
      Vector3 cross = Vector3.Cross(forwardDir, dir);


      // ���� ���溤��(Forward)�� �ٶ� ��ü(Direction)������ ���� FdotD�� cos�� �̴�
      float FdotD = Vector3.Dot(forwardDir, dir) / (Vector3.Magnitude(forwardDir) * Vector3.Magnitude(dir));
      float angle = Mathf.Acos(FdotD) * Mathf.Rad2Deg; // �ڽ��ΰ��� ���Ȱ����� ��ȯ���ִ� Acos, �ͼ��� ���������� �ٽ� ��ȯ�ϴ� Rad2Deg
      Debug.Log("�������� : " + cross + "ȸ���� ���� : " + angle);
      Quaternion Angle ;
      // ���� �� �����;� ���ؿ���, ���������� y���� ������ ��ü�� ���ʹ��⿡ �ִ�
      if (cross.y < 0) // ���溤�� ���� ����
      {
            Angle = player.rotation * Quaternion.Euler(0, -angle, 0);
      }
      else // ���溤�� ���� ������
      {
            Angle = player.rotation * Quaternion.Euler(0, angle, 0);
      }
        return Angle;
    }
    public Vector3 Rot_V3(Transform player, Vector3 target)
    {
        Vector3 forwardDir = (player.forward);
        Vector3 dir = (target - player.position);

        // ���� CrossProduct
        // ����׷α׷� Ȯ�ν� �������, ������  ����, ����� ������
        Vector3 cross = Vector3.Cross(forwardDir, dir);


        // ���� ���溤��(Forward)�� �ٶ� ��ü(Direction)������ ���� FdotD�� cos�� �̴�
        float FdotD = Vector3.Dot(forwardDir, dir) / (Vector3.Magnitude(forwardDir) * Vector3.Magnitude(dir));
        float angle = Mathf.Acos(FdotD) * Mathf.Rad2Deg; // �ڽ��ΰ��� ���Ȱ����� ��ȯ���ִ� Acos, �ͼ��� ���������� �ٽ� ��ȯ�ϴ� Rad2Deg
        Debug.Log("�������� : " + cross + "ȸ���� ���� : " + angle);
        Quaternion Angle;
        // ���� �� �����;� ���ؿ���, ���������� y���� ������ ��ü�� ���ʹ��⿡ �ִ�
        if (cross.y < 0) // ���溤�� ���� ����
        {
            Angle = player.rotation * Quaternion.Euler(0, -angle, 0);
        }
        else // ���溤�� ���� ������
        {
            Angle = player.rotation * Quaternion.Euler(0, angle, 0);
        }
        return Angle.eulerAngles;
    }
    
void CalculDir(Transform player,float degree , Quaternion dest)
    {
       
        player.rotation = Quaternion.Lerp(player.rotation, dest
            , Time.deltaTime *5f);

        // �������� 0.1���� �۴ٸ�, �������ε� Ȯ���� ���������� �̼������̴� ������ �����ֱ�
        if (dest.y - player.rotation.y <= Mathf.Epsilon) 
        {
            player.rotation = dest;
            _isSameDir = true;
        }
    } // ���ʹϾ����� ǥ��
    void CalculDir(Transform player,float degree) // vector3.eulerAngles ���ͷ� ǥ��
    {
        player.eulerAngles = Vector3.Lerp(player.eulerAngles,
            new Vector3(player.eulerAngles.x, degree,  player.eulerAngles.z) ,Time.deltaTime * 3f);
       
        // �������� 0.1���� �۴ٸ�, �������ε� Ȯ���� ���������� �̼������̴� ������ �����ֱ�
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
        // ���⺤�� �����, ���⺤�ʹ� ũ�Ⱑ 1�ΰ��� ����
        Vector3 forwardDir = (player.forward);
        Vector3 rightDir = player.right;
        Vector3 dir = (target - player.position);


        // ���� ����
        // Vector3.Dot(player.position, dir) ==
        // Vector3.Magnitude(player.position) * Vector3.Magnitude(dir) * Mathf.Cos(cos);
        // �ڻ��α��ϱ� ���� cos��ü�� mathf.cos()��
        float cos = Vector3.Dot(forwardDir, dir) / (
            Vector3.Magnitude(forwardDir) * Vector3.Magnitude(dir)  );
        // �ڻ��κ����� ���Լ� ����Ͽ� �������ϱ�
        float radian = Mathf.Acos(cos);   // Acos���� ���Ȱ��� ��ȯ��
        float degree = radian * Mathf.Rad2Deg;
        Debug.Log("cos : " +cos + " , radian :" + radian + ", degree : " + degree);

        // right���Ϳ� ������ ���� �ǹ��ϴ� RdotD ���� (RightVector.Dot.Direction)
        float RdotD = Vector3.Dot(rightDir, dir) / (
            Vector3.Magnitude(rightDir) * Vector3.Magnitude(dir) );  
        float Rcos = Mathf.Acos(RdotD) * Mathf.Rad2Deg;  // right���Ϳ� ������ �ڻ����� ���Լ��� ������ ��ȯ

        if (degree > 360.0f) { degree -= 360.0f; }

        // ���溤�Ϳ� ������ �������̺��� (cos�� ������ ����, ����� �ո�)
        if (cos < 0)
        {
            Debug.Log("��ü�� ���� �ڿ�����");
            Debug.Log((Rcos < 90) ? "�ڿ� ������ ����" : "�ڿ� ���� ����");
            Debug.Log("�����ʺ��Ϳ��� ������ �������� : " + Rcos + ", degree : " + degree);
            if (degree > 180.0f && Rcos > 90.0f)
            {  degree = degree - 360.0f; }
            else if (degree < 180.0f && Rcos > 90.0f)
            { degree = -degree; }
        }
        if (cos >= 0) 
        {
            Debug.Log("��ü�� ���� �տ�����");
            Debug.Log((Rcos <= 90) ? "������ ����" : "���� ����");

            //right���Ϳ� �ٽ� �����Ͽ� 90���� �Ѵ���Ȯ�� ( 90�̻��� ���� , ���ϴ� ������ ����)
            degree = 90.0f - Rcos;
          
            Debug.Log("�����ʺ��Ϳ��� ������ �������� : " + Rcos + ", degree : " + degree);
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
       
        Vector3 forwardDir = (player.forward);  // ����������� ������ 001 z����
        Vector3 dir = (target - player.position);

        // FdotD�� ����� ���� ������ ������� cos�� �� (�յ� �Ǻ��ϱ�����)
        // ������ �����(-1~1���̸� ���� cos����) ����Ͻ� ���⺤�Ͱ� �� �ո�, �����Ͻ� �� ����
        float FdotD = Vector3.Dot(forwardDir, dir) / (Vector3.Magnitude(forwardDir)*Vector3.Magnitude(dir));
        //Debug.Log(FdotD);
        // Atan2 ����
        // ���� ź��Ʈ�� ���, �����ﰢ������ ����/�غ��̵ȴ�
        // ����Ƽ 3d,2d�Ұ;��� ������ǥ��� �ٶ󺻴ٸ� 
        // �ٶ� ��ü�� x, y�� �غ� ���̷� ����Ѵ�
        // �� �̷� ź��Ʈ������ Atan2�� ���Ҽ��ִ�, ���ڼ����� y,x���̴�
        float degree = Mathf.Atan2(dir.z, dir.x) * Mathf.Rad2Deg ;
        float angle =0;
        Debug.Log("dir.z : "+dir.z+"dir.x : "+dir.x +" degree : " + degree);
        // �� �տ� ��ü ����
        if (degree > 0)
        {
            angle = (degree < 90.0f) ? (90.0f - degree) : (90.0f-degree);
           //if (degree < 90.0f) { angle = 90.0f - degree; } // �� ������
           //else { angle = -(degree - 90.0f); } // �� ����
        }
        // �� �ڿ� ����
        if (degree <= 0)
        {
            angle = (degree > -90.0f) ? (90.0f - degree) : (-270.0f - degree) ;
           //if (degree > -90.0f) { angle = 90.0f - degree; }
           //else { angle = -(270.0f + degree); }
        }
        
        // Quaternion angleAxis = Quaternion.AngleAxis(-degree, Vector3.up);
        // transform.rotation = Quaternion.Slerp(player.rotation, angleAxis, Time.deltaTime*2f);
        //z������ ȸ���� �Ѵٸ� ��� ����������, ���� ���� y������ ȸ���ϱ⿡, ������ ������Ų��
        // ������ ����Ƽ �ν����Ϳ��� ���� �˼��ִµ�
        // � ���������Ʈ�� �ΰ� y������ ȸ���غ��� ����� ������, ������ ���ʹ���
        // z������ ȸ���� ������ ������ ����� ���ʹ������� ȸ���ϴ°� ���Ժ����ִ�
        // ���� ������ ������ �������־���Ѵ�
        if (angle != 0 )
        {
            Debug.Log("ȸ�� ����");
            _isSameDir = false;
            RotCoru(player ,angle);
           
        }
        
    }
  
}
