
public class PlayerData_01 : BasePlayerData {

    public PlayerData_01()
    {
        name = "Boris";

        shipRoom = new int[,]
        { { 0, 0 }, { 0, 0 },
          { 0, 0 }, { 0, 0 },
          { 1, 0 }, { 1, 0 },
          { 0, 0 }, { 0, 0 },
          { 0, 0 }, { 0, 0 },
         };

        numUnits = 5;
    }
}
