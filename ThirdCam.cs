using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThirdCam : MonoBehaviour
{
    public GameObject DD;
    public Player _chr; // ĳ��������, ī�޶� �ٶ� ���
    Vector3 pos, _obs; // �̵��� ������ & ���̴��
    Vector3 rot;  // ȸ���� ������
    bool _isLinearTr; // ��������, ��������
    float camDistance; Camera _cam; // ���� ī�޶�Ÿ� & ī�޶������Ʈ

    // ī�޶� ȸ��,�̵��ӵ� , �������� �ִ�Ÿ� , ��ũ�Ѽӵ�, ���콺 ȸ���ӵ�
    [SerializeField] float _rotSpeed, _camSpeed, _maxDistance, _scrollSpeed, _mouseSensi;

    // ������������ ����� ������(camDistance)�� ������(������ �Ӱ�)
    float _radius, _azimuth, _elevation;
    [SerializeField] Vector2 _elevation_Range;
    #region GetSetter
    public float Radius
    {
        get { return _radius; }
        set { _radius = value; }
    }
    public float Azimuth
    {
        get { return _azimuth; }
        set
        {
            _azimuth = Mathf.Clamp(value
          , -360f * Mathf.Deg2Rad, 360f * Mathf.Deg2Rad);
        }
    }
    public float Elevation
    {
        get { return _elevation; }
        set
        {
            _elevation = Mathf.Clamp(value
          , _elevation_Range.x * Mathf.Deg2Rad, _elevation_Range.y * Mathf.Deg2Rad);
        }
    }
    #endregion
    private void Start()
    {
        _cam = transform.GetComponent<Camera>();
        _maxDistance = 7f;
        if (_chr == null)
        { _chr = GameObject.Find("@Player").GetComponent<Player>(); }
        camDistance = _maxDistance;
        pos = _chr.transform.position - transform.forward * camDistance;

        GetSphereCoord(pos);

    }



    void Update()
    {

        zoomInOut();
        avoidObstacle();

        if (Input.GetMouseButtonDown(1))
        {
            GetSphereCoord(pos);
            Elevation = transform.eulerAngles.x * Mathf.Deg2Rad;
        }

        if (Input.GetMouseButton(1))
        {
            _isLinearTr = false;
            SphereCoord();

        }

        else
        {
            _isLinearTr = true;
            pos = _chr.transform.position - transform.forward * camDistance;
            rot = new Vector3(transform.eulerAngles.x,
                _chr.transform.rotation.eulerAngles.y, 0);
        }
        pos.y += 1f;

        transform.rotation = (_isLinearTr == true) ? Quaternion.Slerp
     (transform.rotation, Quaternion.Euler(rot), _rotSpeed * Time.deltaTime)
     : Quaternion.Euler(rot);
        transform.position = Vector3.Lerp(transform.position, pos, _camSpeed * Time.deltaTime);
    }


    private void LateUpdate()
    {
        // �̵�,ȸ�� "������"�� ���⼭ ����������ÿ�
        // ������Ʈ ���� ����Ǵ� LateUpdate�� Ư����
        // ��κ� ī�޶��� �������� �̵�ȸ������ �ۼ��Ͻʽÿ�
        // �׷��� ������ ��ġ�ʴ� ��� + ���͸��� �ްԵ˴ϴ�




    }

    void avoidObstacle()  // ����ĳ���� + ��ֹ� ���ϱ�
    {
        RaycastHit hit;
        Vector3 dir = _chr.transform.position - transform.position;

        // ī�޶� �տ� ��ֹ� �ִ� ������
        if (Physics.Raycast(transform.position, dir.normalized, out hit, dir.magnitude, LayerMask.GetMask("Wall")))
        {
            Vector3 dist = hit.point - _chr.transform.position;
            _obs = hit.point;                                      //0.95������ ��¦ ƴ�� �����, ���ٽ� �ν��������
            camDistance = Mathf.Clamp(camDistance + (_camSpeed * Time.deltaTime), 1f, dist.magnitude * 0.95f);
            return;
        }

        if ((_obs - transform.position).magnitude < 1f)
        {
            return;
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
    void freeLook() // SphereCoord()+GetCartesianCoord+GetSphereCoord �� ��ü��
    {
        // ����
        // float horMove, vertMove;
        // start�� �ۼ�
        // horMove = -90f* Mathf.Deg2Rad;
        // Update �� �ۼ� 
        // if => freeLook();
        // else => vertMove = 15f; 
        // else => horMove = -90f * Mathf.Deg2Rad;


        // ��Ŭ��+���콺ȸ������ ��������
        // ���� Ư¡��, ����������(R)��ŭ�� (X: (Cos��)��(R) ,Y : (Sin��)��(R) ) ��ǥ�� ���� ����� �̷���� ���(��ü����)
        // R�������� ĳ���Ϳ� ī�޶������ �Ÿ�
        Vector3 center = _chr.transform.position;
        Vector2 input = new Vector3(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y"));
        // horMove += input.x * 100f* _mouseSensi * Time.deltaTime * Mathf.Deg2Rad ;
        // vertMove += input.y * 100f* _mouseSensi * Time.deltaTime ;
        // vertMove = Mathf.Clamp(vertMove, 0f, 90f);
        //   float angle = (90.0f - vertMove) * Mathf.Deg2Rad;

        //   center.y += camDistance * Mathf.Cos(angle);
        //   center.x += camDistance * Mathf.Cos(horMove) * Mathf.Sin(angle);
        //   center.z += camDistance * Mathf.Sin(horMove) * Mathf.Sin(angle);

        //   pos = center;
        //   rot = new Vector3(90f -angle*Mathf.Rad2Deg  , -horMove * Mathf.Rad2Deg - 90f, 0);

    }

    public void SphereCoord()
    {
        Azimuth += Input.GetAxisRaw("Mouse X") * Time.deltaTime * _mouseSensi;
        Elevation += Input.GetAxisRaw("Mouse Y") * Time.deltaTime * _mouseSensi;
        if (Mathf.Abs(Azimuth) >= Mathf.PI * 2)
        { Azimuth = (Azimuth >= 0) ? -360f : +360f; }
        rot = new Vector3(Elevation * Mathf.Rad2Deg,
              -Azimuth * Mathf.Rad2Deg - 90f, 0);
        pos = GetCartesianCoord() + _chr.transform.position;
    }
    public void GetSphereCoord(Vector3 cartesianCoord)
    {
        Vector3 dir = cartesianCoord;

        Radius = dir.magnitude; // �������� => dir.magnitude;
        Azimuth = Mathf.Atan2(dir.z, dir.x);
        Elevation = Mathf.Asin(dir.y / dir.magnitude);  // �������� => Mathf.Asin(dir.y / radius);

    }

    Vector3 GetCartesianCoord()
    {
        //float t = Radius * Mathf.Cos(Elevation);
        return new Vector3
            (
             camDistance * Mathf.Cos(Elevation) * Mathf.Cos(Azimuth), // x
             camDistance * Mathf.Sin(Elevation),  // y
             camDistance * Mathf.Cos(Elevation) * Mathf.Sin(Azimuth)//z
            );
    }
    private void OnDrawGizmos()
    {
        //Debug.DrawRay(transform.position, forwardDir * 7f, Color.red);
        // Debug.Log("Dir : " + dir + "forward : " + forwardDir);
        // to do : ĳ���� �޸��� + ����, ĳ���Ϳ� �ִϸ��̼� ��������
        // �����ܾƿ� + ī�޶� ������(360�� ȸ��)

    }
}
