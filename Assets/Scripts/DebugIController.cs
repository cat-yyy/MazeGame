using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem.XR;

public class DebugInfoController : MonoBehaviour
{
    public static TextMeshProUGUI mazeTMP;
    [SerializeField] private LANGenerator lanGenerator;

    public void InitializeDebugInfo()
    {
        mazeTMP = GameObject.Find("MazeTMP").GetComponent<TextMeshProUGUI>();
        //playerController = GameObject.FindWithTag("Player").GetComponent<PlayerController>();
    }

    public void Maze(GridManager gridManager)
    {
        string str = "\n";


        for (int z = 0; z < gridManager.height; z++)
        {
            for (int x = 0; x < gridManager.width; x++)
            {
                GridPoint current=new GridPoint(x,z);
                if (gridManager.GetSquare(current) == Square.Floor) //床
                {
                    str += lanGenerator.playerTrack[z][x];
                    //str += "□";
                }
                else if (gridManager.GetSquare(current) == Square.Wall) //壁
                {
                    str += "■";
                }
                else if (gridManager.GetSquare(current)== Square.Player) //Player
                {
                    str += "<color=blue>●</color>";
                }
                else if (gridManager.GetSquare(current) == Square.Enemy) //Enemy
                {
                    str += "<color=yellow>●</color>";
                }
                else
                {
                    str += "?";

                }

            }
            str += "\n";
        }
        mazeTMP.text = str;


    }

}
