using Unity.Netcode;

public class WeaponData : NetworkBehaviour
{
    public NetworkVariable<int> weaponIndex = new NetworkVariable<int>();
}
