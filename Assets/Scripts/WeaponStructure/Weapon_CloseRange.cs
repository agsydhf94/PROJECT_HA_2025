using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HA
{
    public class Weapon_CloseRange : Weapon
    {
        public IEnumerator SlashVFX()
        {
            weaponSO.swingVfx.SetActive(true);

            yield return new WaitForSeconds(0.5f);

            weaponSO.swingVfx.SetActive(false);
        }
    }
}
