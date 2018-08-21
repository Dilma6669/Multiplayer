
public class PlayerData_00 : BasePlayerData
{

    public PlayerData_00()
    {
        name = "Kate";

        shipRoom = new int[,]
        { { 0, 0 }, { 0, 0 },
          { 0, 0 }, { 0, 0 },
          { 1, 0 }, { 1, 0 },
          { 0, 0 }, { 0, 0 },
          { 0, 0 }, { 0, 0 },
        };

        numUnits = 2;
    }
}
