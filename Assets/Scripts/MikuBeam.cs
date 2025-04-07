using UnityEngine;

public class MikuBeam : MonoBehaviour
{
    [SerializeField] private float beamTime = 1f;
    private bool hasHitPlayer = false;
    // Update is called once per frame
    void Update()
    {
        beamTime -= Time.deltaTime;

        if (beamTime < 0)
        {
            Destroy(this.gameObject);
        }
    }

    void OnTriggerEnter2D(Collider2D collider)
    {
        if (collider.transform.tag == "Player" && !hasHitPlayer)
        {
            Debug.Log("Get miku beamed");
            collider.gameObject.GetComponent<FuelManager>().fossilFuelLevel -= 30;
            hasHitPlayer = true;
        }
    }
}
