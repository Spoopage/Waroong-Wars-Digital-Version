using System;
using System.Collections.Generic;
using UnityEngine;

public enum Ingredient { Nasi, Daging, Sayur, Tahu, Bumbu, Krupuk, Cabe, Goboy }
public enum CharacterType { None, BuPrasojo, GengBajoel, JengSastro, CakLondho, PakBas }

[Serializable]
public struct OptionalBonusData
{
    public Ingredient Ingredient;
    public int VP;
}

[Serializable]
public class MenuCardData
{
    public string Name;
    public int BaseVP;
    public Dictionary<Ingredient, int> Req = new Dictionary<Ingredient, int>();

    public List<OptionalBonusData> Optionals = new List<OptionalBonusData>();

    public Sprite Icon;
}

[Serializable]
public class CustomerCardData
{
    public string Name;
    public string Effect;
    public int VP;
    public Sprite Icon;
}