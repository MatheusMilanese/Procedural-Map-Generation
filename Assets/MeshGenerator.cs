using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeshGenerator : MonoBehaviour
{
    public SquareGrid squareGrid;

    public void GenerateMesh(int[,] map, float squareSize){
        squareGrid = new SquareGrid(map, squareSize);
    }

    public void OnDrawGizmos(){
        if(squareGrid != null){
            for(int x = 0; x < squareGrid.squares.GetLength(0); x++){
                for(int y = 0; y < squareGrid.squares.GetLength(1); y++){
                    Gizmos.color = squareGrid.squares[x,y].topLeft.isActive ? Color.black : Color.white;
                    Gizmos.DrawCube(squareGrid.squares[x,y].topLeft.position, Vector3.one * .4f);

                    Gizmos.color = squareGrid.squares[x,y].topRight.isActive ? Color.black : Color.white;
                    Gizmos.DrawCube(squareGrid.squares[x,y].topRight.position, Vector3.one * .4f);
                    
                    Gizmos.color = squareGrid.squares[x,y].bottomLeft.isActive ? Color.black : Color.white;
                    Gizmos.DrawCube(squareGrid.squares[x,y].bottomLeft.position, Vector3.one * .4f);
                    
                    Gizmos.color = squareGrid.squares[x,y].bottomRight.isActive ? Color.black : Color.white;
                    Gizmos.DrawCube(squareGrid.squares[x,y].bottomRight.position, Vector3.one * .4f);
                    
                    Gizmos.color = Color.gray;
                    Gizmos.DrawCube(squareGrid.squares[x,y].centerTop.position, Vector3.one * .15f);
                    Gizmos.DrawCube(squareGrid.squares[x,y].centerRight.position, Vector3.one * .15f);
                    Gizmos.DrawCube(squareGrid.squares[x,y].centerBottom.position, Vector3.one * .15f);
                    Gizmos.DrawCube(squareGrid.squares[x,y].centerLeft.position, Vector3.one * .15f);

                }
            }
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

        public Square(ControlNode _topLeft, ControlNode _topRight, ControlNode _bottomLeft, ControlNode _bottomRight){
            topLeft = _topLeft;
            topRight = _topRight;
            bottomLeft = _bottomLeft;
            bottomRight = _bottomRight;

            centerTop = topLeft.right;
            centerRight = bottomRight.above;
            centerBottom = bottomLeft.right;
            centerLeft = bottomLeft.above;
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

        public Node(Vector3 _position){
            position = _position;
        }
    }
}
