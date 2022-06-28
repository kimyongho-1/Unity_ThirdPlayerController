using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThirdCam : MonoBehaviour
{
    public GameObject DD;
    public Player _chr; // 캐릭터이자, 카메라가 바라볼 대상
    Vector3 pos, _obs; // 이동할 목적지 & 레이대상
    Vector3 rot;  // 회전할 목적지
    bool _isLinearTr; // 보간할지, 대입할지
    float camDistance; Camera _cam; // 현재 카메라거리 & 카메라오브젝트

    // 카메라 회전,이동속도 , 대상까지의 최대거리 , 스크롤속도, 마우스 회전속도
    [SerializeField] float _rotSpeed, _camSpeed, _maxDistance, _scrollSpeed, _mouseSensi;

    // 구형공간에서 사용할 반지름(camDistance)와 각도축(방위각 앙각)
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
        // 이동,회전 "목적지"를 여기서 계산하지마시오
        // 업데이트 이후 실행되는 LateUpdate의 특성상
        // 대부분 카메라의 직접적인 이동회전만을 작성하십시요
        // 그렇지 않으면 원치않는 결과 + 지터링을 겪게됩니다




    }

    void avoidObstacle()  // 레이캐스팅 + 장애물 피하기
    {
        RaycastHit hit;
        Vector3 dir = _chr.transform.position - transform.position;

        // 카메라 앞에 장애물 있다 판정시
        if (Physics.Raycast(transform.position, dir.normalized, out hit, dir.magnitude, LayerMask.GetMask("Wall")))
        {
            Vector3 dist = hit.point - _chr.transform.position;
            _obs = hit.point;                                      //0.95곱으로 살짝 틈을 줘야함, 안줄시 인식힘들어짐
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

    void zoomInOut() // 마우스휠로 돌리었을떄 줌인 줌아웃
    {
        if (Input.GetAxis("Mouse ScrollWheel") == 0) { return; } // 스크롤입력없음 빠짐

        float scroll = -Input.GetAxis("Mouse ScrollWheel") * _scrollSpeed;
        _cam.fieldOfView = Mathf.Clamp(_cam.fieldOfView + scroll, 33f, 90f);
    }
    void freeLook() // SphereCoord()+GetCartesianCoord+GetSphereCoord 로 대체됨
    {
        // 변수
        // float horMove, vertMove;
        // start에 작성
        // horMove = -90f* Mathf.Deg2Rad;
        // Update 에 작성 
        // if => freeLook();
        // else => vertMove = 15f; 
        // else => horMove = -90f * Mathf.Deg2Rad;


        // 휠클릭+마우스회전으로 자유시점
        // 원의 특징은, 반지름길이(R)만큼의 (X: (Cosθ)×(R) ,Y : (Sinθ)×(R) ) 좌표를 지닌 점들로 이루어진 모양(구체모형)
        // R반지름은 캐릭터와 카메라사이의 거리
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

        Radius = dir.magnitude; // 오리지날 => dir.magnitude;
        Azimuth = Mathf.Atan2(dir.z, dir.x);
        Elevation = Mathf.Asin(dir.y / dir.magnitude);  // 오리지날 => Mathf.Asin(dir.y / radius);

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
        // to do : 캐릭터 달리기 + 점프, 캐릭터와 애니메이션 연동까지
        // 줌인줌아웃 + 카메라 프리룩(360도 회전)

    }
}
