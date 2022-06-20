using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThirdCam : MonoBehaviour
{
    public GameObject DD;
    public Player _chr;
    Vector3 pos;
    Vector3 rot; float horMove, vertMove; bool _isLinearTr;
     [SerializeField] float _obstacleDist = 0.9f;
    float camDistance; Camera _cam;
    [SerializeField] float _rotSpeed, _camSpeed, _maxDistance, _scrollSpeed;
    [SerializeField] Vector2 _MouseSensitivity;
    private void Start()
    {
        horMove = -90f* Mathf.Deg2Rad;
        _cam = transform.GetComponent<Camera>();
        _maxDistance = 7f;
        if (_chr == null)
        { _chr = GameObject.Find("@Player").GetComponent<Player>(); }
        camDistance = _maxDistance;
        pos = _chr.transform.position - transform.forward * camDistance;
      //  StartCoroutine(draw());
    }

    IEnumerator draw()
    {
        while (DD.transform.childCount > 0)
        {
            DD.transform.GetChild(0).position = 
                new Vector3(0, transform.position.y, transform.position.z);
           DD.transform.GetChild(0).SetParent(_chr.transform);
           yield return new WaitForSeconds(0.35f);
        }
        yield break;
    }

    private void Update()
    {
        zoomInOut();
        avoidObstacle();
       
        if (Input.GetMouseButton(1) )
        {
            freeLook(); _isLinearTr = false; 
           
        }
        else
        {
            vertMove = 15f; 
         horMove = -90f * Mathf.Deg2Rad;
         _isLinearTr = true;
         pos = _chr.transform.position - transform.forward * camDistance;
         rot = new Vector3(15f, 
             _chr.transform.rotation.eulerAngles.y, 0);
         pos.y += 1f;
        }
       
    }
   
    private void LateUpdate()
    {
        // �̵�,ȸ�� "������"�� ���⼭ ����������ÿ�
        // ������Ʈ ���� ����Ǵ� LateUpdate�� Ư����
        // ��κ� ī�޶��� �������� �̵�ȸ������ �ۼ��Ͻʽÿ�
        // �׷��� ������ ��ġ�ʴ� ��� + ���͸��� �ްԵ˴ϴ�
       transform.rotation = (_isLinearTr == true) ? Quaternion.Slerp
       (transform.rotation, Quaternion.Euler(rot), _rotSpeed * Time.deltaTime)
       : Quaternion.Euler(rot);
     transform.position = Vector3.Lerp(transform.position, pos, _camSpeed * Time.deltaTime);
        


    }

    void avoidObstacle()  // ����ĳ���� + ��ֹ� ���ϱ�
    {
        RaycastHit hit;
        Vector3 dir = transform.position - _chr.transform.position;
       //Debug.DrawRay(_chr.transform.position, dir.normalized * dir.magnitude, Color.red);
        if (Physics.Raycast(_chr.transform.position, dir.normalized, out hit, dir.magnitude, LayerMask.GetMask("Wall")))
        {
            Debug.Log(hit.transform.name);
            Vector3 dist = hit.point - _chr.transform.position;
            camDistance = (dist.magnitude * _obstacleDist);
        }
        else
        {
            camDistance = Mathf.Clamp(camDistance + (_camSpeed * Time.deltaTime), 1f, _maxDistance);
        }
    }

    void zoomInOut() // ���콺�ٷ� ���������� ���� �ܾƿ�
    {
        if (Input.GetAxis("Mouse ScrollWheel") == 0) { return; } // ��ũ���Է¾��� ����

        float scroll = -Input.GetAxis("Mouse ScrollWheel") * _scrollSpeed;
        _cam.fieldOfView = Mathf.Clamp(_cam.fieldOfView + scroll, 33f, 90f);
    }
    void freeLook() // ��Ŭ��+���콺ȸ������ ��������
    {
        // ���� Ư¡��, ����������(R)��ŭ�� (X: (Cos��)��(R) ,Y : (Sin��)��(R) ) ��ǥ�� ���� ����� �̷���� ���(��ü����)
        // R�������� ĳ���Ϳ� ī�޶������ �Ÿ�
        Vector3 center = _chr.transform.position; 
        Vector2 input = new Vector3(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y"));
        horMove += input.x * 100f* _MouseSensitivity.x * Time.deltaTime * Mathf.Deg2Rad ;
        vertMove += input.y * 100f* _MouseSensitivity.y * Time.deltaTime ;
        vertMove = Mathf.Clamp(vertMove, 0f, 90f);
        float angle = (90.0f - vertMove) * Mathf.Deg2Rad;
       
        center.y += camDistance * Mathf.Cos(angle);
        center.x += camDistance * Mathf.Cos(horMove) * Mathf.Sin(angle);
        center.z += camDistance * Mathf.Sin(horMove) * Mathf.Sin(angle);
    
        pos = center;
        rot = new Vector3(90f -angle*Mathf.Rad2Deg  , -horMove * Mathf.Rad2Deg - 90f, 0);
    
    }
    Vector3 ToSphereCoordinate(Vector3 pos, Vector3 center, float radius = 0)
    {
        // ������ǥ������� ������ǥ��� ��ȯ�ϴ� �Լ�
        // ���ڷ� ���纤�Ͱ�, �����̳� �����̵� center
        // Ȥ�� �������̳� ������ �����ִٸ� radius�Ķ���� ä����, �ƴϸ� ���α�
        // ������ǥ�� R �� �� = ������,������,�簢 (Radius,azimuth,elevation)
        // ������ǥ�迡�� ������ǥ��� 
        // (x y z) -> (R �� ��)
        // ������ R : Square(x^2+y^2+z^2)
        // ������azimuth �� : Mathf.Acos(pos.z/radius) == Mathf.Atan2( Square(x^2+y^2) ,pos.z) 
        // == Mathf.Atan2(pos.z, pos.x) 
        // �簢elevation �� : Mathf.Atan2(y,x) ==  Mathf.Asin(pos.y/radius)


        Vector3 dir = pos ;
        if (radius == 0) { radius = pos.magnitude; }

        float azimuth = Mathf.Atan2(pos.z, pos.x) + Input.GetAxis("Mouse X") * _MouseSensitivity.x;
        float elevation = Mathf.Asin(pos.y / radius) + Input.GetAxis("Mouse Y") * _MouseSensitivity.y;

        return new Vector3(radius, azimuth * Mathf.Deg2Rad, elevation * Mathf.Deg2Rad);
    }
    Vector3 ToCartesianCoordinate(float radius, float theta, float gamma)
    {
        // ������ǥ������� ������ǥ��� ��ȯ�ϴ� �Լ�
        // ���ڷ� ������ǥ�踦 �̷���ִ� ������, ��������(azimuth)����, �簢��(elevation)������ ����
        // x = R * sin�� * cos��
        // y = R * sin�� * sin��
        // z = R * cos��

        float x = radius * Mathf.Cos(gamma) * Mathf.Cos(theta);
        float z = radius * Mathf.Sin(theta) * Mathf.Cos(gamma);
        float y = radius * Mathf.Sin(gamma);
        return new Vector3(x,y,z);
    }
   
    private void OnDrawGizmos()
    { 
        //Debug.DrawRay(transform.position, forwardDir * 7f, Color.red);
        // Debug.Log("Dir : " + dir + "forward : " + forwardDir);
        // to do : ĳ���� �޸��� + ����, ĳ���Ϳ� �ִϸ��̼� ��������
        // �����ܾƿ� + ī�޶� ������(360�� ȸ��)
     
    }
}
