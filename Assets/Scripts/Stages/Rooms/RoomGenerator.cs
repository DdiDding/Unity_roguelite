using UnityEngine;

public class RoomGenerator
{
    // x: 최소, y: 최대
    public Vector2 WidthSizeRange; 
    public Vector2 HeightSizeRange;


    // 랜덤한 크기의 방 생성
    public Room GenerateRoom(int randomSeed)
    {
        var random = new System.Random(randomSeed);

        // 방의 Boundery 설정
        Room room;
        {
            float width = Random.Range(WidthSizeRange.x, WidthSizeRange.y);
            float height = Random.Range(HeightSizeRange.x, HeightSizeRange.y);
            room = new Room(width, height);
        }

        return room;
    }

}
