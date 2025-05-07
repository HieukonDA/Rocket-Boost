using UnityEngine;

public interface ISkillManager
{
    void ActivateSkill(string skillTag, Vector3 position, Transform playerTransform); // Kích hoạt skill từ SkillCircle
    bool HasShield(); // Kiểm tra khiên
    bool HasPierce(); // Kiểm tra xuyên chướng ngại
    bool HasCoinMagnet(); // Kiểm tra hút coin
    float GetRemainingSkillTime(); // Lấy thời gian còn lại của skill
    void ResetSkills(); // Reset khi restart level
}