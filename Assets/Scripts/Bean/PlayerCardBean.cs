using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCardBean
{
    public int cid;
    public int proficiency;

    public PlayerCardBean(int Cid,int Pro)
    {
        this.cid = Cid;
        this.proficiency = Pro;
    }

    public PlayerCardBean() { }
}
