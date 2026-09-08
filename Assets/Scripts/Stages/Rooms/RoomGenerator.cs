using UnityEngine;

public class RoomGenerator
{
    readonly public (int Min, int Max) WidthSizeRange;
    readonly public (int Min, int Max) HeightSizeRange;

    public RoomGenerator((int Min, int Max) widthSizeRange, (int Min, int Max) heightSizeRange)
    {
        WidthSizeRange = widthSizeRange;
        HeightSizeRange = heightSizeRange;
    }

    // 랜덤한 크기의 방 생성
    // randomGenerator: System.Random 객체
    public Room GenerateRoom(System.Random randomGenerator)
    {
        Debug.Assert(randomGenerator != null);

        // 방의 Boundery 설정
        Room room;
        {
            float width = randomGenerator.Next(WidthSizeRange.Min, WidthSizeRange.Max);
            float height = randomGenerator.Next(HeightSizeRange.Min, HeightSizeRange.Max);
            room = new Room(width, height);
        }

        return room;
    }

}
