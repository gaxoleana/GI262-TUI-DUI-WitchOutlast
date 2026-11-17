using UnityEngine;

public class UpgradeManager : MonoBehaviour
{
    public bool UpgradeWeapon(Weapon weapon)
    {
        if (weapon == null)
        {
            return false;
        }

        return weapon.Upgrade();
    }
}
