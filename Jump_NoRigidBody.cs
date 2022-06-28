using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Jump_NoRigidBody : MonoBehaviour
{
    float t = 0; // �ð�����
    public float _jumpPower = 3.5f; // ������������ ������ ����������� ����
    int state = 0;  // ��������
    public float _accel, _gravity = 3.5f; // �׼� �ö󰥋� �߰����ӵ�, �߷� �������� ���ӵ�
    public float _feetPos; // ĳ���Ͱ� ������ y��ġ ĳ�̺���
    const float Accell = 0.02f; // ���� ���ӵ�, Ÿ�ӵ�Ÿ�� ������ ���� �ƴ϶� ������·� ������
    private void Start()
    {
        _feetPos = transform.position.y ;
    }
    // y = -x^2 + ax  =  -x(x - a)
    // ���� ���� = �ð����� + ������X�ð�
    // TO DO : �������Ʒ��� ����, �ߵ��� ���� �Ӹ� �ε��� ���մ��� �ۼ�
    void CheckJump()
    {
        // �����������ҋ� �Ӹ����� ���̽��� ��ֹ� �Ǵ�
        // ���� �ϰ��ҋ� ��Ʈ�Ʒ� ���̽��� ��ֹ� �Ǵ�
        // �����Լ� üũ ����!!
        float yy = transform.position.y;  // ���� ĳ���� ���̰����� �߰� ���� ��������� 

        if (state == 0 && Input.GetKeyDown(KeyCode.Space))
        {
            state = 1;
        }
        myJump(ref yy);  //�ʱⰪ�� ������ ref ������ out���� 
        transform.position = new Vector3(transform.position.x, yy, transform.position.z);
    }

    void myJump(ref float yy)
    {
        switch (state)
        {
            case 0:  // Stay
                break;

            case 1:  // Jump Up
                t += Accell * _accel;
                yy = -t * (t - _jumpPower);
                if (t >= _jumpPower * 0.5f) { Debug.Log(yy); state = 2; }
                break;
            case 2:  // Jump Down
                t += Accell * _gravity;
                yy = -t * (t - _jumpPower);
                if (yy <= _feetPos) { t = state = 0;  yy = _feetPos; }
                break;
        }
    }
}
