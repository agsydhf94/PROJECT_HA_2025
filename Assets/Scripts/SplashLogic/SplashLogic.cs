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
            // A의 Forward 벡터
            Vector3 forwardA = transform.forward;

            // A에서 B로 향하는 벡터
            Vector3 directionToB = (overlappedObjects[i].transform.position - transform.position).normalized;

            // 두 벡터 간의 각도 계산
            float angle = Vector3.Angle(forwardA, directionToB);

            // 각도가 60도 이내인지 확인
            if (angle <= splashAngle)
            {
                Debug.Log("B는 A의 Forward 기준으로 60도 이내에 있습니다.");
                detectedObjects.Add(overlappedObjects[i].gameObject);
            }
            else
            {
                Debug.Log("B는 A의 Forward 기준으로 60도 밖에 있습니다.");
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
