using UnityEngine;
using System.IO;

public class MapGenerator : MonoBehaviour
{

    [SerializeField]
    GameObject obstaclePrefab;
    
    [SerializeField]
    GameObject emptyPrefab;

    [SerializeField]
    GameObject playerPrefab;

    Vector2 playerSpawn = new Vector2(0, 0);

    // makes a map from a random file in the maps folder
    public void MakeMap()
    {
        string[] imageFiles = Directory.GetFiles("Assets/Maps", "*.jpg|*.png");

        int randomIndex = Mathf.RoundToInt(Random.Range(0, imageFiles.Length));

        MakeMap("Assets/Maps" + "/" + imageFiles[randomIndex]);
    }
    public void MakeMap(string filePath)
    {
        if (!File.Exists(filePath))
        {
            Debug.LogError("File not found: " + filePath);
            return;
        }

        byte[] fileData = File.ReadAllBytes(filePath);

        Texture2D texture = new Texture2D(2, 2, TextureFormat.RGBA32, false);

        if(!texture.LoadImage(fileData))
        {
            Debug.LogError("Failed to load image from file: " + filePath);
            return;
        }

        for(int x = 0; x<texture.width; x++)
        {
            for(int y = 0; y<texture.height; y++)
            {
                Color pixelColor = texture.GetPixel(x, y);
                
                // very red = obstacle 
                // very green = player spawn 
                // very white = empty 

                if(pixelColor.r > 0.9 && pixelColor.g < 0.1 && pixelColor.b < 0.1)
                {
                    GameObject.Instantiate(obstaclePrefab, new Vector3(x, y, 0), Quaternion.identity);
                }
                else if(pixelColor.g > 0.9 && pixelColor.r < 0.1 && pixelColor.b < 0.1)
                {
                    GameObject.Instantiate(emptyPrefab, new Vector3(x, y, 0), Quaternion.identity);
                    playerSpawn = new Vector2(x, y);
                }
                else if(pixelColor.r > 0.9 && pixelColor.g > 0.9 && pixelColor.b > 0.9)
                {
                    GameObject.Instantiate(emptyPrefab, new Vector3(x, y, 0), Quaternion.identity);
                }
            }
        }
    }

    public GameObject SpawnPlayer()
    {
        return GameObject.Instantiate(playerPrefab, new Vector3(playerSpawn.x, playerSpawn.y, 0), Quaternion.identity);
    }
}
