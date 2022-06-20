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
        // 이동,회전 "목적지"를 여기서 계산하지마시오
        // 업데이트 이후 실행되는 LateUpdate의 특성상
        // 대부분 카메라의 직접적인 이동회전만을 작성하십시요
        // 그렇지 않으면 원치않는 결과 + 지터링을 겪게됩니다
       transform.rotation = (_isLinearTr == true) ? Quaternion.Slerp
       (transform.rotation, Quaternion.Euler(rot), _rotSpeed * Time.deltaTime)
       : Quaternion.Euler(rot);
     transform.position = Vector3.Lerp(transform.position, pos, _camSpeed * Time.deltaTime);
        


    }

    void avoidObstacle()  // 레이캐스팅 + 장애물 피하기
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

    void zoomInOut() // 마우스휠로 돌리었을떄 줌인 줌아웃
    {
        if (Input.GetAxis("Mouse ScrollWheel") == 0) { return; } // 스크롤입력없음 빠짐

        float scroll = -Input.GetAxis("Mouse ScrollWheel") * _scrollSpeed;
        _cam.fieldOfView = Mathf.Clamp(_cam.fieldOfView + scroll, 33f, 90f);
    }
    void freeLook() // 휠클릭+마우스회전으로 자유시점
    {
        // 원의 특징은, 반지름길이(R)만큼의 (X: (Cosθ)×(R) ,Y : (Sinθ)×(R) ) 좌표를 지닌 점들로 이루어진 모양(구체모형)
        // R반지름은 캐릭터와 카메라사이의 거리
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
        // 직교좌표계공간을 구면좌표계로 전환하는 함수
        // 인자로 현재벡터값, 기준이나 원점이될 center
        // 혹시 반지름이나 역할이 따로있다면 radius파라미터 채우면됨, 아니면 냅두길
        // 구면좌표계 R θ Φ = 반지름,방위각,양각 (Radius,azimuth,elevation)
        // 직교좌표계에서 구면좌표계로 
        // (x y z) -> (R θ Φ)
        // 반지름 R : Square(x^2+y^2+z^2)
        // 방위각azimuth θ : Mathf.Acos(pos.z/radius) == Mathf.Atan2( Square(x^2+y^2) ,pos.z) 
        // == Mathf.Atan2(pos.z, pos.x) 
        // 양각elevation Φ : Mathf.Atan2(y,x) ==  Mathf.Asin(pos.y/radius)


        Vector3 dir = pos ;
        if (radius == 0) { radius = pos.magnitude; }

        float azimuth = Mathf.Atan2(pos.z, pos.x) + Input.GetAxis("Mouse X") * _MouseSensitivity.x;
        float elevation = Mathf.Asin(pos.y / radius) + Input.GetAxis("Mouse Y") * _MouseSensitivity.y;

        return new Vector3(radius, azimuth * Mathf.Deg2Rad, elevation * Mathf.Deg2Rad);
    }
    Vector3 ToCartesianCoordinate(float radius, float theta, float gamma)
    {
        // 구면좌표계공간을 직교좌표계로 전환하는 함수
        // 인자로 구면좌표계를 이루고있는 반지름, 방위각θ(azimuth)각도, 양각Φ(elevation)각도를 받음
        // x = R * sinθ * cosΦ
        // y = R * sinθ * sinΦ
        // z = R * cosθ

        float x = radius * Mathf.Cos(gamma) * Mathf.Cos(theta);
        float z = radius * Mathf.Sin(theta) * Mathf.Cos(gamma);
        float y = radius * Mathf.Sin(gamma);
        return new Vector3(x,y,z);
    }
   
    private void OnDrawGizmos()
    { 
        //Debug.DrawRay(transform.position, forwardDir * 7f, Color.red);
        // Debug.Log("Dir : " + dir + "forward : " + forwardDir);
        // to do : 캐릭터 달리기 + 점프, 캐릭터와 애니메이션 연동까지
        // 줌인줌아웃 + 카메라 프리룩(360도 회전)
     
    }
}
