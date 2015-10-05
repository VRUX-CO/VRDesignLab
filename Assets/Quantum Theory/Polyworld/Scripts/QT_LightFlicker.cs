using UnityEngine;
using System.Collections;

public class QT_LightFlicker : MonoBehaviour
{

   	public float minFlickerSpeed  = 0.01f;
    public float maxFlickerSpeed = 0.1f;
    public float minLightIntensity = 0.7f;
	public float maxLightIntensity =1;

    void Start()
    {
        StartCoroutine(Flicker());
    }

    IEnumerator Flicker()
    {
        while(true)
        {       
          GetComponent<Light>().intensity = Random.Range(minLightIntensity, maxLightIntensity);

          yield return new WaitForSeconds(Random.Range(minFlickerSpeed, maxFlickerSpeed));

        }
    }
}



