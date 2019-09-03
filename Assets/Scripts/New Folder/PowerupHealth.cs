using UnityEngine;
public class PowerupHealth : Powerup
{
    public float amount = 50;
    public override bool Apply(GameObject p)
    {
        TankHealth h = p.GetComponent<TankHealth>();
        if (h)
        {
            h.AddHealth(amount);
        }
        Hide();
        return true;
    }

}

