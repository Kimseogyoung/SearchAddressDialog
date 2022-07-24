using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CamController : MonoBehaviour
{
    
    Transform playerTransform;

    public float cameraMoveSpeed;

    private int camMode;//0: 1��Ī 1: 3��Ī
    public float maxDistance;
    public float minDistance ;
    public  float cameraDistance =5.0f;//3��Ī ī�޶� �Ÿ�
    public float turnSpeed = 4.0f; // ���콺 ȸ�� �ӵ�    
    private float xRotate = 0.0f; // ���� ����� X�� ȸ������ ���� ���� ( ī�޶� �� �Ʒ� ���� )
    Vector3 cameraPosition;
    private void Start()
    {
        cameraPosition = new Vector3(0, cameraDistance, -5);
    }
    public void SetPlayer(Transform transform)
    {
        playerTransform = transform;
        ChangeMode(1);
    }
    public void ChangeMode(int mode)
    {
        camMode = mode;
        if (camMode == 0)
        {
            transform.position = playerTransform.Find("viewPoint").position;
        }
        else if(camMode == 1)
        {
            transform.position = playerTransform.position - playerTransform.forward + Vector3.up;
            transform.LookAt(playerTransform.position);
            transform.position = playerTransform.position - playerTransform.forward *cameraDistance;


        }
    }
    public int GetCamMode()
    {
        return camMode;
    }
    private void LateUpdate()
    {
        if (playerTransform != null)
        {
            if (camMode == 0)
            {
                float yRotateSize = Input.GetAxis("Mouse X") * turnSpeed;
                // ���� y�� ȸ������ ���� ���ο� ȸ������ ���
                float yRotate = transform.eulerAngles.y + yRotateSize;
                
                // ���Ʒ��� ������ ���콺�� �̵��� * �ӵ��� ���� ī�޶� ȸ���� �� ���(�ϴ�, �ٴ��� �ٶ󺸴� ����)
                float xRotateSize = -Input.GetAxis("Mouse Y") * turnSpeed;
                // ���Ʒ� ȸ������ ���������� -45�� ~ 80���� ���� (-45:�ϴù���, 80:�ٴڹ���)
                // Clamp �� ���� ������ �����ϴ� �Լ�
                xRotate = Mathf.Clamp(xRotate + xRotateSize, -45, 80);

                // ī�޶� ȸ������ ī�޶� �ݿ�(X, Y�ุ ȸ��)
                transform.eulerAngles = new Vector3(xRotate, yRotate, 0);
                transform.position = playerTransform.Find("viewPoint").position;

            }
            else if (camMode == 1)
            {
                float wheel = Input.GetAxis("Mouse ScrollWheel");
                cameraDistance -= wheel * 4f;
                cameraDistance = Mathf.Clamp(cameraDistance, minDistance, maxDistance);

                Vector2 mouseMove = Vector2.zero;
                if (Input.GetMouseButton(1))
                {
                    mouseMove.y = Input.GetAxis("Mouse X") * turnSpeed;
                    mouseMove.x = -Input.GetAxis("Mouse Y") * turnSpeed;

                    
                    //transform.rotation = Quaternion.Euler(xmove, ymove, 0);
                    //transform.eulerAngles = new Vector3(mouseMove.x, mouseMove.y, 0);
                    if (mouseMove.magnitude != 0)
                    {
                        Quaternion q = transform.rotation;
                        float newx =  Mathf.Clamp(q.eulerAngles.x + mouseMove.x, 0, 70);
                        q.eulerAngles = new Vector3(newx, q.eulerAngles.y + mouseMove.y , q.eulerAngles.z);
                        transform.rotation = q;

                    }
                    transform.position = playerTransform.position - transform.forward * cameraDistance;

                    /*
                    ���� ���� ���ؼ� 
                    �ش� ��ġ�� ��������� �����ִ� �ڵ�(�ʱ� ���)
                    //transform.eulerAngles = new Vector3(xRotate, yRotate, 0);
                    Vector3 tmp = Vector3.Normalize(transform.position - playerTransform.position);
                    tmp.y = 0;
                    Quaternion q = Quaternion.Euler(new Vector3(xRotate, yRotateSize, 0f));
                    mode1CamVec = q *  mode1CamVec ;

                    transform.position = Vector3.Lerp(transform.position, playerTransform.position + mode1CamVec,
                 Time.deltaTime * cameraMoveSpeed);

                    */

                }
                else
                {
                    transform.position = playerTransform.position - transform.forward * cameraDistance;
                }

            }
        }


    }
}
