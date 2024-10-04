using UnityEngine;

public class CallShopCard : IUseEffect, IChoiseTarget
{
    public bool _effect => false;
    public bool storeEnd { get => Store.Instance._storeCanvas == null; }
    public bool ChoiseTarget(CharacterBase useCharacter, out CharacterBase target)
    {
        if (Store.Instance._storeCanvas == null) Store.Instance.StoreStart();
        target = null;
        return Store.Instance._storeCanvas == null;
    }

    public void Effect(CharacterBase useCharacter, CharacterBase target)
    {
        Store.Instance.StoreEnd();
    }

    public T GetEffectClass<T>() where T : IUseEffect
    {
        if (this is T effect)
        {
            return effect;
        }
        return default;
    }
}
