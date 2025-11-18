using System;
using System.Collections.Generic;

public enum Ingredient { Nasi, Daging, Sayur, Tahu, Bumbu, Krupuk, Cabe, Goboy }
public enum CharacterType { None, BuPrasojo, GengBajoel, JengSastro, CakLondho, PakBas }

[Serializable] public class MenuCardData {
    public string Name;
    public int BaseVP;
    public Dictionary<Ingredient, int> Req = new Dictionary<Ingredient, int>();
    public Ingredient? OptionalKey;
    public int OptionalVP;
    public int OptionalMax;
}

[Serializable] public class CustomerCardData {
    public string Name;
    public string Effect;
    public int VP;
}