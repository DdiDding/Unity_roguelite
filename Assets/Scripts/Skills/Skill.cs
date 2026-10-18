using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct SkillData
{
    public AnimationClip AnimationClip;
    public float Cooltime;
    public float Damage;
}

[CreateAssetMenu(fileName = "Skill", menuName = "Scriptable Objects/Skill")]
public class Skill : ScriptableObject
{
    [SerializeField]
    private List<SkillData> skills = new List<SkillData>();

    public int Count => skills.Count;

    public SkillData GetSkill(int index) => skills[index];
}
