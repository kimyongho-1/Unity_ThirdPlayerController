using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    [SerializeField] float _moveSpeed ,_rotSpeed;
    public Transform _localCamera;
    void Start()
    {
        Managers mng = Managers.INSTANCE;
        Managers._InputManger.POS = this.transform;
        Managers._InputManger.KeyAction -= OnKeyboad;
        Managers._InputManger.KeyAction += OnKeyboad;
    }
   
    public void OnKeyboad() // Ű���� �Է����� ����
    {
        float rotSpeed = _rotSpeed;
        // �Է�
        Vector2 input = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
        // ����
        Vector2 dir = input.normalized;
        
        //�ڷ� ���鼭 �¿�ȸ�����ҋ� ��� ȸ���� �׷��� ����
        if (input.y < 0) { rotSpeed *= 0.25f; }
       
        this.transform.SetPositionAndRotation( 
            this.transform.position +transform.TransformDirection(new Vector3(0,0,input.y))*dir.magnitude * _moveSpeed * Time.deltaTime
            , (input.x == 0) ? this.transform.rotation : this.transform.rotation *
            Quaternion.Euler(Vector3.up * Mathf.Atan2(dir.x, dir.y) * Mathf.Rad2Deg * rotSpeed * Time.deltaTime)
            );
    }
    public void OnMouse()
    { 
        

    }
    public void OnMousePos() // ����ĳ��Ʈ ���
    {
        if (!Input.GetMouseButton(0)) { return; }
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit))
        {
            if (hit.transform != null)
            {
                // ������
                Vector3 dest = new Vector3(hit.point.x, this.transform.position.y, hit.point.z);
                // ���Ⱒ��
                Quaternion degree = Managers._Rot.Rot_Q4(this.transform, hit.point);

                StopAllCoroutines();
                StartCoroutine(Managers._InputManger.Moving(this.transform, dest, 5f));
                StartCoroutine(Managers._InputManger.Rotating(this.transform, degree, 2f));
            }
        }
    }
    
}
