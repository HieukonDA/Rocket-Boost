using UnityEngine;

[CreateAssetMenu(fileName = "NewSkill", menuName = "Skills/SkillData", order = 1)]
public class SkillData : ScriptableObject
{
    [SerializeField] private string skillName; // Tên skill (VD: "Shield")
    [SerializeField] private float duration = 5f; // Thời gian hiệu lực
    [SerializeField] private bool isShield = false; // Có phải skill Shield không
    [SerializeField] private AudioClip pickupSound; // Âm thanh khi nhặt
    [SerializeField] private ParticleSystem effectPrefab; // Hiệu ứng particle khi nhặt
    [SerializeField] private GameObject skillModel; // Mô hình 3D của skill

    public string SkillName => skillName;
    public float Duration => duration;
    public bool IsShield => isShield;
    public AudioClip PickupSound => pickupSound;
    public ParticleSystem EffectPrefab => effectPrefab;
    public GameObject SkillModel => skillModel;
}