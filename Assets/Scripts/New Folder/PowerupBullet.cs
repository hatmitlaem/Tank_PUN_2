
using UnityEngine;
public class PowerupBullet : Powerup
{

    public override bool Apply(GameObject p)
    {
        PlayerManager m = p.GetComponent<PlayerManager>();
        if (m)
            m.UpdateShell();
        Hide();
        return true;
    }

}

