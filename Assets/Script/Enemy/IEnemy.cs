public interface IEnemy
{
    // Hàm này cho phép tất cả quái thực hiện nhận sát thương
    void TakeDamage(int damage, string damageType, int attackDirection);
}
