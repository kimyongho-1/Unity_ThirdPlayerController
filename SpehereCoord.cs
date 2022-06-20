using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpehereCoord : MonoBehaviour
{
    float _Radius ,_Azimuth , _Elevation;
    [SerializeField]float _mouseSensi;
    Vector2 _limit;
    Transform _Earth;
    private void Start()
    {
        _Earth = GameObject.Find("Earth").GetComponent<Transform>();
    }
    private void Update()
    {
       
        {
            //ToSphereCoordinate2D(_Earth.position);
            //ToCartesianCoordinate2D();
            ToSphereCoordinate3D(_Earth.position);
            ToCartesianCoordinate3D();
        }
        Debug.Log("R : "+_Radius+", Azimuth : "+_Azimuth+", Elevation : "+_Elevation);
    }

    void ToSphereCoordinate2D(Vector3 center)
    {
        // R , azimuth, elevation
        Vector2 tr = new Vector2(transform.position.x, transform.position.y);
        Vector2 point = new Vector2(center.x, center.y);
        Vector2 dir = (tr - point);

        _Radius = dir.magnitude;
        _Azimuth = Mathf.Atan2(dir.y, dir.x) 
            + Input.GetAxis("Mouse X")*_mouseSensi*Time.deltaTime;
    }
    void ToCartesianCoordinate2D()
    {
        // x y z
        float x = _Radius * Mathf.Cos(_Azimuth);
        float y = _Radius * Mathf.Sin(_Azimuth);
        transform.position = new Vector3(x,y,transform.position.z);
    }
    void ToSphereCoordinate3D(Vector3 center)
    {
        // R  ����(center)���� ���� ��ü������ �Ÿ�
        // azimuth ������, x���� +�������κ��� �������� ��ü���� ���� ������ ����
        // elevation �簢�� y���� +�������κ��� �������� ��ü���� ���� ������ ����
        
        Vector3 tr = transform.position;
        Vector3 dir = (tr - center);

        _Radius = dir.magnitude;
        _Azimuth = Mathf.Atan2(dir.z, dir.x)
            + Input.GetAxis("Horizontal") * _mouseSensi * Time.deltaTime;

        _Elevation = Mathf.Acos( dir.y / _Radius)
             + Input.GetAxis("Vertical") * _mouseSensi * Time.deltaTime; ;

        // ������ǥ���� ��ǥ���� ���� ������ ������ǥ�� ����ų�����־� ���Ѱ�����
        //_Azimuth = Mathf.Clamp(_Azimuth, 0, Mathf.PI * 2);
        _Elevation = Mathf.Clamp(_Elevation, 0, Mathf.PI );

    }
    void ToCartesianCoordinate3D()
    {
        // x y z
        float x = _Radius * Mathf.Cos(_Azimuth) * Mathf.Sin(_Elevation);
        float z = _Radius * Mathf.Sin(_Azimuth) * Mathf.Sin(_Elevation);
        float y = _Radius * Mathf.Cos(_Elevation);
        transform.position = new Vector3(x, y, z);
    }
}
