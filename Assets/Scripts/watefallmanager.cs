using UnityEngine;

public class WaterfallManager : MonoBehaviour
{
    public GameObject waterfallPrefab; 
    public float spawnOffset;      
    private Vector3 startPosition;
    public float verticalDistance = -10; 
    public float speed = 5f;           
    public float delayBetweenBlocks = 0.5f; 

    void Start()
    {
        startPosition=transform.position+ new Vector3(0,spawnOffset,0);
               
        StartCoroutine(SpawnWaterfall(0f));
        StartCoroutine(SpawnWaterfall(delayBetweenBlocks));
    }

    System.Collections.IEnumerator SpawnWaterfall(float delay)
    {
        yield return new WaitForSeconds(delay);

        GameObject block = Instantiate(waterfallPrefab, startPosition, Quaternion.identity);
        WaterfallBlock wb = block.GetComponent<WaterfallBlock>();
        wb.Init(startPosition, verticalDistance, speed);
    }
}
