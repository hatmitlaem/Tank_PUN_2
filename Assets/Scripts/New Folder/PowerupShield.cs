
using UnityEngine;

public class PowerupShield : Powerup
{
    public override bool Apply(GameObject p)
    {
        TankHealth h = p.GetComponent<TankHealth>();
        if (h)
            h.ActiveArmor();
        Hide();
        return true;
    }
}

