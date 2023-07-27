using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeshGenerator : MonoBehaviour
{
    public SquareGrid squareGrid;
    public List<Vector3> vertices;
    public List<int> triangles;
    

    public void GenerateMesh(int[,] map, float squareSize){

        squareGrid = new SquareGrid(map, squareSize);
        vertices = new List<Vector3>();
        triangles = new List<int>();

        for(int x = 0; x < squareGrid.squares.GetLength(0); x++){
            for(int y = 0; y < squareGrid.squares.GetLength(1); y++){
                TriangulateSquare(squareGrid.squares[x,y]);
            }
        }

        Mesh mesh = new Mesh();
        GetComponent<MeshFilter>().mesh = mesh;
        mesh.vertices = vertices.ToArray();
        mesh.triangles = triangles.ToArray();
        mesh.RecalculateNormals();

    }

    public void TriangulateSquare(Square square){
        switch(square.configuration){
            case 0:
                break;

            // 1 point:

            case 1:
                MeshFromPoints(square.centerLeft, square.centerBottom, square.bottomLeft);
                break;
            case 2:
                MeshFromPoints(square.bottomRight, square.centerBottom, square.centerRight);
                break;
            case 4:
                MeshFromPoints(square.topRight, square.centerRight, square.centerTop);
                break;
            case 8:
                MeshFromPoints(square.topLeft, square.centerTop, square.centerLeft);
                break;

            // 2 points:
            case 3:
                MeshFromPoints(square.centerRight, square.bottomRight, square.bottomLeft, square.centerLeft);
                break;
            case 6:
                MeshFromPoints(square.centerTop, square.topRight, square.bottomRight, square.centerBottom);
                break;
            case 9:
                MeshFromPoints(square.topLeft, square.centerTop, square.centerBottom, square.bottomLeft);
                break;
            case 12:
                MeshFromPoints(square.topLeft, square.topRight, square.centerRight, square.centerLeft);
                break;
            case 5:
                MeshFromPoints(square.centerTop, square.topRight, square.centerRight, square.centerBottom, square.bottomLeft, square.centerLeft);
                break;
            case 10:
                MeshFromPoints(square.topLeft, square.centerTop, square.centerRight, square.bottomRight, square.centerBottom, square.centerLeft);
                break;

            // 3 point:
            case 7:
                MeshFromPoints(square.centerTop, square.topRight, square.bottomRight, square.bottomLeft, square.centerLeft);
                break;
            case 11:
                MeshFromPoints(square.topLeft, square.centerTop, square.centerRight, square.bottomRight, square.bottomLeft);
                break;
            case 13:
                MeshFromPoints(square.topLeft, square.topRight, square.centerRight, square.centerBottom, square.bottomLeft);
                break;
            case 14:
                MeshFromPoints(square.topLeft, square.topRight, square.bottomRight, square.centerBottom, square.centerLeft);
                break;

            // 4 point:
            case 15:
                MeshFromPoints(square.topLeft, square.topRight, square.bottomRight, square.bottomLeft);
                break;
        }
    }

    public void MeshFromPoints(params Node[] points){
        AssignVertices(points);

        if(points.Length >= 3)
            CreateTriangle(points[0], points[1], points[2]);
        if(points.Length >= 4)
            CreateTriangle(points[0], points[2], points[3]);
        if(points.Length >= 5)
            CreateTriangle(points[0], points[3], points[4]);
        if(points.Length >= 6)
            CreateTriangle(points[0], points[4], points[5]);
    }

    public void AssignVertices(Node[] points){
        for(int i = 0; i < points.Length; i++){
            if(points[i].vertexIndex == -1){
                points[i].vertexIndex = vertices.Count;
                vertices.Add(points[i].position);
            }
        }
    }

    public void CreateTriangle(Node a, Node b, Node c){
        triangles.Add(a.vertexIndex);
        triangles.Add(b.vertexIndex);
        triangles.Add(c.vertexIndex);
    }

    struct Triangle {
        public int vertexIndexA;
        public int vertexIndexB;
        public int vertexIndexC;

        int[] vertices;

        public Triangle(int a, int b, int c){
            vertexIndexA = a;
            vertexIndexB = b;
            vertexIndexC = c;
            vertices = new int[]{a, b, c};
        }

        public bool Contains(int vertexIndex){
            return vertexIndex == vertexIndexA || vertexIndex == vertexIndexB || vertexIndex == vertexIndexC;
        }

        public int this[int i]{
            get{ return vertices[i]; }
        }
    }

    public class SquareGrid{
        public Square[,] squares;

        public SquareGrid(int[,] map, float _squareSize){
            int nodeCountX = map.GetLength(0);
            int nodeCountY = map.GetLength(1);
            float mapWidth = nodeCountX * _squareSize;
            float mapHeight = nodeCountY * _squareSize;

            ControlNode[,] controlNodes = new ControlNode[nodeCountX,nodeCountY];

            for(int x = 0; x < nodeCountX; x++){
                for(int y = 0; y < nodeCountY; y++){
                    Vector3 position = new Vector3(x*_squareSize + _squareSize/2f - mapWidth/2f, 0, y*_squareSize + _squareSize/2 - mapHeight/2f);
                    controlNodes[x,y] = new ControlNode(position, map[x,y] == 1, _squareSize);
                }
            }

            squares = new Square[nodeCountX-1, nodeCountY-1];

            for(int x = 0; x < nodeCountX-1; x++){
                for(int y = 0; y < nodeCountY-1; y++){
                    squares[x,y] = new Square(controlNodes[x,y+1], controlNodes[x+1, y+1], controlNodes[x,y], controlNodes[x+1, y]);
                }
            }
        }
    }

    public class Square {
        public ControlNode topLeft, topRight, bottomLeft, bottomRight;
        public Node centerTop, centerRight, centerBottom, centerLeft;

        public int configuration;

        public Square(ControlNode _topLeft, ControlNode _topRight, ControlNode _bottomLeft, ControlNode _bottomRight){
            topLeft = _topLeft;
            topRight = _topRight;
            bottomLeft = _bottomLeft;
            bottomRight = _bottomRight;

            centerTop = topLeft.right;
            centerRight = bottomRight.above;
            centerBottom = bottomLeft.right;
            centerLeft = bottomLeft.above;

            if(_topLeft.isActive)
                configuration += 8;
            if(_topRight.isActive)
                configuration += 4;
            if(_bottomRight.isActive)
                configuration += 2;
            if(_bottomLeft.isActive)
                configuration += 1;
        }
    }

    public class ControlNode : Node {
        public bool isActive; 
        public Node above, right;

        public ControlNode(Vector3 _position, bool _isActive, float squareSize) : base(_position){
            isActive = _isActive;
            above = new Node(position + Vector3.forward * squareSize/2f);
            right = new Node(position + Vector3.right * squareSize/2f);
        }
    }

    public class Node {
        public Vector3 position;
        public int vertexIndex;

        public Node(Vector3 _position){
            position = _position;
            vertexIndex = -1;
        }
    }
}
