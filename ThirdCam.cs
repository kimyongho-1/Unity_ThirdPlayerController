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

    void avoidObstacle()  // 레이캐스팅 + 장애물 피하기
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

    void zoomInOut() // 마우스휠로 돌리었을떄 줌인 줌아웃
    {
        if (Input.GetAxis("Mouse ScrollWheel") == 0) { return; } // 스크롤입력없음 빠짐

        float scroll = Input.GetAxis("Mouse ScrollWheel") * _scrollSpeed;
        _cam.fieldOfView = Mathf.Clamp(_cam.fieldOfView + scroll, 33f, 90f);
    }
    void freeLook() // 휠클릭+마우스회전으로 자유시점
    {
        // 원의 특징은, 반지름길이(R)만큼의 (X: (Cosθ)×(R) ,Y : (Sinθ)×(R) ) 좌표를 지닌 점들로 이루어진 모양(구체모형)
        // R반지름은 캐릭터와 카메라사이의 거리

        Vector3 center = _chr.transform.position; // 원형태로 돌떄, 원의 중심점은 캐릭터가될것이다(캐릭터중심으로 회전할거기에)

        Vector3 input = new Vector3(Input.GetAxis("Mouse X"), 0, 0); // 마우스 좌우회전
        input = input.normalized;

        horMove += (input.x * Time.deltaTime * 5f); // 원형태로 돌며, 싸코싸인 함수의 인자가 각도라서 각도 변수 만들어사용
                                                    //if (angle > 360f) { angle -= 360f; } // 360도는 0도와 같다, 초과시 360만큼 뺸 각도와 동일


        center.x += camDistance * Mathf.Cos(horMove);
        center.y = 4;
        center.z += camDistance * Mathf.Sin(horMove);

        // 원형태로 카메라가 위치할 위치 완성!
        pos = center;

        Vector3 forwardDir = -(_chr.transform.forward);
        Vector3 dir = (transform.position - _chr.transform.position);
        dir.y = 0; dir = dir.normalized;
        float FdotD = Vector3.Dot(forwardDir, dir) / (Vector3.Magnitude(forwardDir) * Vector3.Magnitude(dir));
        if (FdotD == 1) 
        { rot = transform.rotation.eulerAngles; return; }
        float degree = Mathf.Atan2(-dir.x,-dir.z) * Mathf.Rad2Deg;
        // To do : mathfAtan2말고 외적이나 다른 방식으로 각도를 구해야겠음
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

        // to do : 캐릭터 달리기 + 점프, 캐릭터와 애니메이션 연동까지
        // 줌인줌아웃 + 카메라 프리룩(360도 회전)

    }
}
