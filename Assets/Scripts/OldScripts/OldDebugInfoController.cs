/*using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TMPro;
using UnityEngine;

public class OldDebugInfoController : MonoBehaviour
{
    public static TextMeshProUGUI mazeTMP;
    void Start()
    {
        mazeTMP = GameObject.Find("MazeTMP").GetComponent<TextMeshProUGUI>();
       

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void Maze(List<List<StageCreator.Square>> grid,OldPlayerController controller)
    {
        string str = "\n";


        for (int y = 0; y < grid.Count; y++)
            {
                for (int x = 0; x < grid[y].Count; x++)
                {
                    if (grid[y][x] == StageCreator.Square.Floor) //床
                    {
                        str +=controller.playerTrack[y][x];
                    }
                    else if (grid[y][x] == StageCreator.Square.Wall) //壁
                    {
                        str += "■";
                    }
                    else if (grid[y][x] == StageCreator.Square.Player) //Player
                    {
                        str += "<color=blue>●</color>";
                    }
                    else if (grid[y][x] == StageCreator.Square.Enemy) //Enemy
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
*/
