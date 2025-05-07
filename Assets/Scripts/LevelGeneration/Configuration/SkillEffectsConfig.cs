using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewSkill", menuName = "Skills/SkillEffectsConfig", order = 2)]
public class SkillEffectsConfig : ScriptableObject
{
    [SerializeField] private List<SkillEffectEntry> skillEffectPrefabs; // Prefab của hiệu ứng skill

   // Lớp phụ để lưu tag và prefab
    [System.Serializable]
    public class SkillEffectEntry
    {
        public string skillTag; // Tag để ánh xạ với SkillData
        public GameObject prefab; // Prefab có collider để bảo vệ
    }

    // Trả về prefab dựa trên tag
    public GameObject GetPrefabByTag(string skillTag)
    {
        SkillEffectEntry entry = skillEffectPrefabs.Find(e => e.skillTag == skillTag);
        return entry != null ? entry.prefab : null;
    }
}
