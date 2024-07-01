using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FocusCam : MonoBehaviour
{
    //Biến lưu trữ người chơi
    public List<GameObject> players;
    //Biến lưu trữ vị trí của người chơi hiện tại
    private int currentPlayerindex = 0;

    //Hàm này sẽ được gọi khi lượt của một người chơi kết thúc
    public void EndTurn()
    {
        //Kiểm tra xem lượt của nhân vật hiện tại đã kết thúc chưa và chuyển sang nhân vật tiếp theo trong danh sách
        currentPlayerindex = (currentPlayerindex + 1) % players.Count;

        //Lấy vị trí của nhân vật tiếp theo
        Vector3 targetPos = players[currentPlayerindex].transform.position;

        //Chuyển cam tới vị trí người chơi tiếp theo
        MoveCamToPos(targetPos);
    }

    public void MoveCamToPos(Vector3 targetPos)
    {
        //Đặt cam tới vị trí mới
        Vector3 newPos = new Vector3(targetPos.x, targetPos.y, Camera.main.transform.position.z);
        Camera.main.transform.position = newPos;
    }
}
