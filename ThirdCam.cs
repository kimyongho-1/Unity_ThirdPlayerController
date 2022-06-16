using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThirdCam : MonoBehaviour
{
    public Player _chr;
    Vector3 pos;
    Vector3 rot; float horMove = 0;
    [SerializeField] float _obstacleDist = 0.9f;
    float camDistance; Camera _cam;
    [SerializeField] float _rotSpeed, _camSpeed, _maxDistance, _scrollSpeed;
    private void Start()
    {
        _cam = transform.GetComponent<Camera>();
        _maxDistance = 7f;
        if (_chr == null)
        { _chr = GameObject.Find("@Player").GetComponent<Player>(); }
        camDistance = _maxDistance;
        pos = _chr.transform.position - transform.forward * camDistance;
    }

    private void Update()
    {
        
        zoomInOut();
        avoidObstacle();

        if (Input.GetMouseButton(2))
        { freeLook();  }
        else
        {
            pos = _chr.transform.position - transform.forward * camDistance;
            rot = new Vector3(this.transform.rotation.eulerAngles.x, _chr.transform.rotation.eulerAngles.y, 0);
            pos.y += 1f;
        }

    }
    private void LateUpdate()
    {
        
        this.transform.rotation = Quaternion.Slerp(this.transform.rotation,
        Quaternion.Euler(rot), _rotSpeed * Time.deltaTime);
      

        transform.position = Vector3.Lerp(transform.position, pos, _camSpeed * Time.deltaTime);
    }

    void avoidObstacle()  // ����ĳ���� + ��ֹ� ���ϱ�
    {
        RaycastHit hit;
        Vector3 dir = transform.position - _chr.transform.position;
        Debug.DrawRay(_chr.transform.position, dir.normalized * dir.magnitude, Color.red);
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

        float scroll = Input.GetAxis("Mouse ScrollWheel") * _scrollSpeed;
        _cam.fieldOfView = Mathf.Clamp(_cam.fieldOfView + scroll, 33f, 90f);
    }
    void freeLook() // ��Ŭ��+���콺ȸ������ ��������
    {
        // ���� Ư¡��, ����������(R)��ŭ�� (X: (Cos��)��(R) ,Y : (Sin��)��(R) ) ��ǥ�� ���� ����� �̷���� ���(��ü����)
        // R�������� ĳ���Ϳ� ī�޶������ �Ÿ�

        Vector3 center = _chr.transform.position; // �����·� ����, ���� �߽����� ĳ���Ͱ��ɰ��̴�(ĳ�����߽����� ȸ���Ұű⿡)

        Vector3 input = new Vector3(Input.GetAxis("Mouse X"), 0, 0); // ���콺 �¿�ȸ��
        input = input.normalized;

        horMove += (input.x * Time.deltaTime * 5f); // �����·� ����, ���ڽ��� �Լ��� ���ڰ� ������ ���� ���� �������
                                                    //if (angle > 360f) { angle -= 360f; } // 360���� 0���� ����, �ʰ��� 360��ŭ �A ������ ����


        center.x += camDistance * Mathf.Cos(horMove);
        center.y = 4;
        center.z += camDistance * Mathf.Sin(horMove);

        // �����·� ī�޶� ��ġ�� ��ġ �ϼ�!
        pos = center;

        Vector3 forwardDir = -(_chr.transform.forward);
        Vector3 dir = (transform.position - _chr.transform.position);
        dir.y = 0; dir = dir.normalized;
        float FdotD = Vector3.Dot(forwardDir, dir) / (Vector3.Magnitude(forwardDir) * Vector3.Magnitude(dir));
        if (FdotD == 1) 
        { rot = transform.rotation.eulerAngles; return; }
        float degree = Mathf.Atan2(-dir.x,-dir.z) * Mathf.Rad2Deg;
        // To do : mathfAtan2���� �����̳� �ٸ� ������� ������ ���ؾ߰���
        float angle = 0;
        Debug.Log(degree);
        angle = (degree < 90.0f) ? (90.0f - degree) : (90.0f - degree);
        
        if (Mathf.Abs(angle) > 1)
        {
            rot = new Vector3(transform.rotation.eulerAngles.x, angle, 0);
        }
        else
        { rot = transform.rotation.eulerAngles; }
        //Vector3 angle = Managers._Rot.Rot_V3(transform, _chr.transform.position);
        //if (Mathf.Abs(this.transform.rotation.eulerAngles.y - angle.y) < 0.1) { return; }
        //angle.x = this.transform.rotation.eulerAngles.x;
        //angle.z = 0;
        //rot = angle;

        // to do : ĳ���� �޸��� + ����, ĳ���Ϳ� �ִϸ��̼� ��������
        // �����ܾƿ� + ī�޶� ������(360�� ȸ��)

    }
}
