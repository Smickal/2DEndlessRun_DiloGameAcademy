using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TerrainGeneratorController : MonoBehaviour
{
    [Header("Templates")]
    public List<TerrainTemplateController> terrainTemplates;
    public float TerrainTemplateWidth;

    [Header("Generator Area")]
    public Camera gameCamera;
    public float areaStartOffset;
    public float areaEndOffset;

    private List<GameObject> spawnedTerrain;

    private const float debugLineHeight = 10.0f;

    private float lastGeneratedPositionX;
    private float lastRemovedPositionX;

    private Dictionary<string, List<GameObject>> pool;



    private void Start()
    {
        pool = new Dictionary<string, List<GameObject>>();

        spawnedTerrain = new List<GameObject>();

        lastGeneratedPositionX = GetHorizontalPositionStart();
        GenerateFirstTerrain(lastGeneratedPositionX);
        lastGeneratedPositionX += TerrainTemplateWidth;

        lastRemovedPositionX = lastGeneratedPositionX - TerrainTemplateWidth;


    }

    private GameObject GenerateFromPool(GameObject item, Transform parent)
    {
        if (pool.ContainsKey(item.name))
        {
            if(pool[item.name].Count > 0)
            {
                GameObject newItemFromPool = pool[item.name][0];
                pool[item.name].Remove(newItemFromPool);
                newItemFromPool.SetActive(true);
                return newItemFromPool;
            }
        }
        else
        {
            pool.Add(item.name, new List<GameObject>());
        }

        GameObject newItem = Instantiate(item, parent);
        newItem.name = item.name;
        return newItem;
    }

    private void ReturnToPool(GameObject item)
    {
        if (!pool.ContainsKey(item.name))
        {
            Debug.LogError("Invalid Pool item!!");
        }

        pool[item.name].Add(item);
        item.SetActive(false);
    }


    private void Update()
    {
        while (lastGeneratedPositionX < GetHorizontalPositionEnd())
        {
            GenerateTerrain(lastGeneratedPositionX);
            lastGeneratedPositionX += TerrainTemplateWidth;
        }

        while(lastRemovedPositionX + TerrainTemplateWidth < GetHorizontalPositionStart())
        {
            lastRemovedPositionX += TerrainTemplateWidth;
            RemoveTerrain(lastRemovedPositionX);
        }
    }

    private void RemoveTerrain(float posX)
    {
        GameObject terrainToRemove = null;
        //cari terrain
        foreach(GameObject item in spawnedTerrain)
        {
            if(item.transform.position.x == posX)
            {
                terrainToRemove = item;
                break;
            }
        }
        //setelah ketemu
        if(terrainToRemove != null)
        {
            spawnedTerrain.Remove(terrainToRemove);
            Destroy(terrainToRemove);
        }

    }

    private float GetHorizontalPositionStart()
    {
        return gameCamera.ViewportToWorldPoint(new Vector2(0f, 0f)).x + areaStartOffset;
    }

    private float GetHorizontalPositionEnd()
    {
        return gameCamera.ViewportToWorldPoint(new Vector2(1f, 0f)).x + areaEndOffset;
    }


    private void GenerateTerrain(float posX)
    {
        GameObject newTerrain = Instantiate(terrainTemplates[Random.Range(0, terrainTemplates.Count)].gameObject, transform);

        newTerrain.transform.position = new Vector2(posX, -4.5f);

        spawnedTerrain.Add(newTerrain);
    }

    private void GenerateFirstTerrain(float posX)
    {
        GameObject newTerrain = Instantiate(terrainTemplates[0].gameObject, transform);

        newTerrain.transform.position = new Vector2(posX, -4.5f);

        spawnedTerrain.Add(newTerrain);
    }

    //debug
    private void OnDrawGizmos()
    {
        Vector3 areaStartPosition = transform.position;
        Vector3 areaEndPosition = transform.position;

        areaStartPosition.x = GetHorizontalPositionStart();
        areaEndPosition.x = GetHorizontalPositionEnd();

        Debug.DrawLine(areaStartPosition + Vector3.up * debugLineHeight / 2, areaStartPosition + Vector3.down * debugLineHeight / 2, Color.red);
        Debug.DrawLine(areaEndPosition + Vector3.up * debugLineHeight / 2, areaEndPosition + Vector3.down * debugLineHeight / 2, Color.red);
    }
}
