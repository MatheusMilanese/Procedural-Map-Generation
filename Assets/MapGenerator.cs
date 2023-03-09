using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapGenerator : MonoBehaviour
{
    [SerializeField] private int _width, _height;

    [Range(0,100)]
    [SerializeField] private int _randomFillPercent;

    [SerializeField] private int _smoothCount;

    [SerializeField] private string _seed;
    [SerializeField] private bool _useRandomSeed;

    private int[,] map;
    // Start is called before the first frame update
    void Start()
    {
        GenerateMap();
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Space))
            GenerateMap();
    }

    // private void OnDrawGizmos() {
    //     if(map != null){
    //         for(int x = 0; x < _width; x++){
    //             for(int y = 0; y < _height; y++){
    //                 Gizmos.color = map[x,y] == 1 ? Color.black : Color.white;
    //                 Vector3 pos = new Vector3(x - _width/2 + .5f, 0, y - _height/2 + .5f);
    //                 Gizmos.DrawCube(pos, Vector3.one);
    //             }
    //         }
    //     }
    // }

    private void GenerateMap(){
        map = new int[_width, _height];

        RandomFillMap();

        for(int i = 0; i < _smoothCount; i++){
            SmoothMap();
        }

        MeshGenerator meshGenerator = GetComponent<MeshGenerator>();
        meshGenerator.GenerateMesh(map, 1);
    }

    private void RandomFillMap(){
        if(_useRandomSeed){
            _seed = Time.time.ToString();
        }

        System.Random pseudoRandom = new System.Random(_seed.GetHashCode());

        for(int x = 0; x < _width; x++){
            for(int y = 0; y < _height; y++){
                if(x == 0 || x == _width-1 || y == 0 || y == _height-1)
                    map[x,y] = 1;
                else
                    map[x,y] = pseudoRandom.Next(0, 100) < _randomFillPercent ? 1 : 0;
            }
        }
    }

    private void SmoothMap(){
        int[,] newMap = (int[,]) map.Clone();

        for(int x = 0; x < _width; x++){
            for(int y = 0; y < _height; y++){
                int wallCount = GetNeighbourWallsCount(x, y);

                if(wallCount > 4)
                    newMap[x,y] = 1;
                else if(wallCount < 4)
                    newMap[x,y] = 0;
            }
        }

        map = (int[,]) newMap.Clone();
    }

    private int GetNeighbourWallsCount(int x, int y){
        int wallCount = 0;

        for(int neighbourX = x-1; neighbourX <= x+1; neighbourX++){
            for(int neighbourY = y-1; neighbourY <= y+1; neighbourY++){
                if(neighbourX == x && neighbourY == y)
                    continue;
                if(neighbourX < 0 || neighbourX >= _width || neighbourY < 0 || neighbourY >= _height)
                    wallCount++;
                else if(map[neighbourX, neighbourY] == 1)
                    wallCount++;
            }
        }

        return wallCount;
    }
}
