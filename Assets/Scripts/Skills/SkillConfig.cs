using UnityEngine;

public class SkillConfig : MonoBehaviour
{
    [SerializeField]
    private Skill NormalAtk;
    [SerializeField]
    private Skill SpecialAtk;

    
    public Skill NormalAttak => NormalAtk;
    public Skill SpecialAttak => SpecialAtk;
}
