using UnityEngine;
using UnityEngine.Rendering;
using System.Collections.Generic;

public class GeneratorStage
{
    public RoomGenerator RoomGenerator;
    readonly public (int Min, int Max) RoomCountRange;

    public GeneratorStage((int Min, int Max) roomCountRange)
    {
        RoomGenerator = new RoomGenerator((1,2),(2,4));

        // 직접 설정
        RoomCountRange = roomCountRange;
    }

    public List<Room> GenerateRooms(System.Random randomGenerator)
    {
        Debug.Assert(randomGenerator != null);

        List<Room> result = new List<Room>();
        int roomCount = randomGenerator.Next(RoomCountRange.Min, RoomCountRange.Max + 1);
        for (int i = 0; i < roomCount; i++)
        {
            result.Add(RoomGenerator.GenerateRoom(randomGenerator));
        }

        return result;
    }

}
