using System;
using System.Collections.Generic;
using System.Linq;

public enum Ingredient { Nasi, Daging, Sayur, Tahu, Bumbu, Krupuk, Cabe, Goboy }

[Serializable] public class MenuCardData {
    public string Name;
    public int BaseVP;
    public Dictionary<Ingredient,int> Req = new();
    public Ingredient? OptionalKey; 
    public int OptionalVP = 0;
    public int OptionalMax = 0;
}

[Serializable] public class CustomerCardData {
    public string Name;
    public string Effect;
    public int VP;  
}
