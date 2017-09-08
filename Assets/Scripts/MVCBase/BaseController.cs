using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseController<M> where M : BaseModel
{
    protected M Model;

    public virtual void Setup(M model)
    {
        Model = model;
    }
}
