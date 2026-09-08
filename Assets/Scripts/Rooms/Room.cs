using UnityEngine;

public class Room
{
    /* 
     * position 좌측 하단 모서리
     */
    public Rect Bounds;
    Room(float width, float height)
    {
        Bounds = new Rect(0, 0, width, height);
    }
}
