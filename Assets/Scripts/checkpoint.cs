using UnityEngine;

public class checkpoint : MonoBehaviour
{
    public bool activated = false;  
    public static GameObject[] checkPointList;

    public static Vector3 defaultRespawnPoint = new Vector3(0f, 1f, 0f);    

    public static Vector3 GetActiveCheckPointPosition()
    {
        Vector3 activeCheckpointPosition = defaultRespawnPoint;

        if (checkPointList != null)
        {
            foreach (GameObject checkPoint in checkPointList)
            {
                if (checkPoint.GetComponent<checkpoint>().activated)
                {
                    activeCheckpointPosition = new Vector3(checkPoint.transform.position.x + 2f, checkPoint.transform.position.y - 0.2f, checkPoint.transform.position.z); 
                    break;
                }
            }
        }

        return activeCheckpointPosition;
    }

    private void ActivateCheckPoint() {
        foreach(GameObject checkPoint in checkPointList) {
            checkPoint.GetComponent<checkpoint>().activated = false;
            checkPoint.GetComponent<ParticleSystem>().Stop();
        }

        activated = true;
    }

    void Start() {
        checkPointList = GameObject.FindGameObjectsWithTag("CheckPoint");
    }

    void Update() {
        if(activated && !gameObject.GetComponent<ParticleSystem>().isPlaying) {
            gameObject.GetComponent<ParticleSystem>().Play();
        }
    }

    void OnTriggerEnter(Collider other) {
        if(other.gameObject.CompareTag("Player") && !activated) {
            ActivateCheckPoint();
        }
    }
}
