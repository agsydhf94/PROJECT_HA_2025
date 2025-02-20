using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class SplashLogic : MonoBehaviour
{
    /*
    private void OnDrawGizmos()
    {
        for(int i = 0; i < detectedObjects.Count; i++)
        {
            Gizmos.DrawLine(transform.position, detectedObjects[i].transform.position);
        }
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, radius);
    }

    public List<GameObject> detectedObjects = new List<GameObject>();
    public float radius = 3f;
    public float splashAngle = 70f;
    public float distance = 10f;
    */
    



    /*
    public void DetectObjectsBySphere()
    {
        detectedObjects.Clear();
        Collider[] overlappedObjects = Physics.OverlapSphere(transform.position, radius);
        for(int i = 0; i < overlappedObjects.Length; i++)
        {
            Vector3 dir = transform.position - overlappedObjects[i].transform.position;
            Ray ray = new Ray(transform.position, dir.normalized);
            if(Physics.Raycast(ray, out RaycastHit hit, radius))
            {
                if(hit.transform == overlappedObjects[i].transform)
                {
                    detectedObjects.Add(overlappedObjects[i].gameObject);
                }
            }

            
        }
    }
    */

    /*
    [ContextMenu("Detect")]
    public void DetectObjectBySector()
    {
        detectedObjects.Clear();
        Collider[] overlappedObjects = Physics.OverlapSphere(transform.position, radius);
        for (int i = 0; i < overlappedObjects.Length; i++)
        {
            // A�� Forward ����
            Vector3 forwardA = transform.forward;

            // A���� B�� ���ϴ� ����
            Vector3 directionToB = (overlappedObjects[i].transform.position - transform.position).normalized;

            // �� ���� ���� ���� ���
            float angle = Vector3.Angle(forwardA, directionToB);

            // ������ 60�� �̳����� Ȯ��
            if (angle <= splashAngle)
            {
                Debug.Log("B�� A�� Forward �������� 60�� �̳��� �ֽ��ϴ�.");
                detectedObjects.Add(overlappedObjects[i].gameObject);
            }
            else
            {
                Debug.Log("B�� A�� Forward �������� 60�� �ۿ� �ֽ��ϴ�.");
            }
        }
    }
    */

    /*
    public void DetectObjectBySphereCasting()
    {
        detectedObjects.Clear();

        Ray ray = new Ray(transform.position, transform.forward);
        RaycastHit[] hitObjects = Physics.SphereCastAll(ray, radius, distance);
        if (hitObjects != null)
        {
            for (int i = 0; i < hitObjects.Length; i++)
            {
                detectedObjects.Add(hitObjects[i].transform.gameObject);
            }
        }
    }
    */

    /*
    [ContextMenu("Detect")]
    public void ExplosionPhysics()
    {
        detectedObjects.Clear();
        Collider[] overlappedObjects = Physics.OverlapSphere(transform.position, radius);
        for (int i = 0; i < overlappedObjects.Length; i++)
        {
            if (overlappedObjects[i].attachedRigidbody != null)
            {
                overlappedObjects[i].attachedRigidbody.AddExplosionForce(1000f, transform.position, radius);
            }

            detectedObjects.Add(overlappedObjects[i].gameObject);
        }
    }
    */




}
