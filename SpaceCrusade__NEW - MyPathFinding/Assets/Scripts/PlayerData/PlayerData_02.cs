
public class PlayerData_02 : BasePlayerData {

    public PlayerData_02()
    {
        name = "Fred";

        shipRoom = new int[,]
        { { 0, 0 }, { 0, 0 },
          { 0, 0 }, { 0, 0 },
          { 1, 0 }, { 1, 0 },
          { 0, 0 }, { 0, 0 },
          { 0, 0 }, { 0, 0 },
        };

        numUnits = 3;
    }
}